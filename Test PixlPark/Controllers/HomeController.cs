using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft;
using System.IO;
using System.Web.Script.Serialization;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Test_PixlPark.Models;

namespace Test_PixlPark.Controllers
{
    public class HomeController : Controller
    {
        static string Hash(string input)
        {
            using (SHA1 sha1Hash = SHA1.Create())
            {
                //From String to byte array
                byte[] sourceBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
                string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

                return hash;
            }
        }
        private string GetRequestToken()
        {
            // Адрес ресурса, к которому выполняется запрос
            string url = "http://api.pixlpark.com/oauth/requesttoken";


            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "GET";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //ответ от сервера
                var response = streamReader.ReadToEnd();
                bool success = Convert.ToBoolean(JObject.Parse(response)["Success"].ToString());
                if (success)
                {
                    return JObject.Parse(response)["RequestToken"].ToString();
                }


            }
            return "";
        }
        private bool Autorization()
        {

            string requestToken = GetRequestToken();
            if (requestToken != "")
            {
                Session["requestToken"] = requestToken;
                string accessToken = GetAccessToken(requestToken);
                if (accessToken != "")
                {
                    Session["accessToken"] = accessToken;
                    Session["LogIn"] = true;
                    return true;
                }
                else
                {
                    Session["requestToken"] = "";
                    Session["LogIn"] = false;
                    return false;
                }
            }
            else
            {
                Session["LogIn"] = false;
                return false;
            }


        }
        private string GetAccessToken(string oauth_token)
        {
            // Адрес ресурса, к которому выполняется запрос
            string url = "http://api.pixlpark.com/oauth/accesstoken?";
            string password = Hash(oauth_token + "8e49ff607b1f46e1a5e8f6ad5d312a80");
            string param = "oauth_token=" + oauth_token + "&grant_type=api&username=38cd79b5f2b2486d86f562e3c43034f8&password=" + password;


            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + param);

            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "GET";
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                //ответ от сервера
                var response = streamReader.ReadToEnd();
                bool success = Convert.ToBoolean(JObject.Parse(response)["Success"].ToString());
                if (success)
                {
                    return JObject.Parse(response)["AccessToken"].ToString();
                }


            }
            return "";
        }
        private List<Order> GetOrders(string oauth_token,string action,int count = 10,int skip = 0)
        {
            // Адрес ресурса, к которому выполняется запрос
            string url = "http://api.pixlpark.com/orders?";
            string param = "oauth_token=" + oauth_token+ "&count="+count+"&skip=" + skip ;


            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + param);

            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "GET";
            List<Order> orders = new List<Order>();
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //ответ от сервера
                    var response = streamReader.ReadToEnd();
                    var result = JObject.Parse(response)["Result"].ToString();
                    orders = JsonConvert.DeserializeObject<List<Order>>(result);
                    return orders;
                }
            }
            catch(WebException ex)
            {
                // получаем статус исключения
                WebExceptionStatus status = ex.Status;

                if (status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    if ((int)httpResponse.StatusCode == 401)
                    {
                        Autorization();
                        Response.Redirect(Request.RawUrl);
                    }
                }
                return orders ;
                
            }
            

        }
        private int GetOrdersCount(string oauth_token)
        {
            // Адрес ресурса, к которому выполняется запрос
            string url = "http://api.pixlpark.com/orders/count?";
            string param = "oauth_token=" + oauth_token;


            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url + param);

            httpWebRequest.ContentType = "text/json";
            httpWebRequest.Method = "GET";
            try
            {
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    //ответ от сервера
                    var response = streamReader.ReadToEnd();
                    int result = Convert.ToInt32(JObject.Parse(response)["Result"][0]["count"].ToString());
                    return result;
                }
            }
            catch (WebException ex)
            {
                // получаем статус исключения
                WebExceptionStatus status = ex.Status;

                if (status == WebExceptionStatus.ProtocolError)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)ex.Response;
                    if ((int)httpResponse.StatusCode == 401)
                    {
                        Autorization();
                        Response.Redirect(Request.RawUrl);
                    }
                }
                return 0;

            }


        }
        public ActionResult Index()
        {

            if (Session.IsNewSession)
            {
                if (Autorization())
                {
                    return View();
                }
                else
                {
                    return View("Error");
                }

            }
            else
            {
                if (Convert.ToBoolean(Session["LogIn"]))
                    return View();
                else
                    if (Autorization())
                {
                    return View();
                }
                else
                {
                    return View("Error");
                }

            }


        }

        public ActionResult Orders(int page = 1)
        {
            int pageSize = 10;
            ViewBag.Message = "Тут будут все заказы";
            if (Convert.ToBoolean(Session["LogIn"]))
            {
                int skip = (page - 1) * pageSize;
                List<Order> orders = GetOrders(Session["accessToken"].ToString(),"Orders",pageSize,skip);
                PageInfo pageInfo = new PageInfo { PageNumber = page, PageSize = pageSize, TotalItems = GetOrdersCount(Session["accessToken"].ToString()) };
                IndexViewModel ivm = new IndexViewModel { PageInfo = pageInfo, Orders = orders };
                return View(ivm);
            }
            else
                return RedirectToAction("Index");
        }


    }
}