using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;


namespace Test_PixlPark.Helpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html,
            Models.PageInfo pageInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            TagBuilder ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");
            //------------------- previous
            TagBuilder previous = new TagBuilder("li");
            previous.AddCssClass("page-item");
            TagBuilder previousa = new TagBuilder("a");
            previousa.InnerHtml = "Previous";
            previousa.AddCssClass("page-link");
            if (pageInfo.PageNumber == 1)
            {

                previous.AddCssClass("disabled");
            }
            else
            {
                previousa.MergeAttribute("href", pageUrl(pageInfo.PageNumber - 1));
            }
            
            previous.InnerHtml += previousa.ToString(TagRenderMode.Normal);
            ul.InnerHtml += previous.ToString(TagRenderMode.Normal);
            //-------------first
            TagBuilder firstl = new TagBuilder("li");
            previous.AddCssClass("page-item");
            TagBuilder firsta = new TagBuilder("a");
            firsta.InnerHtml = "First";
            firsta.AddCssClass("page-link");
            if (pageInfo.PageNumber == 1)
            {

                firstl.AddCssClass("disabled");
            }
            else
            {
                firsta.MergeAttribute("href", pageUrl(1));
            }

            firstl.InnerHtml += firsta.ToString(TagRenderMode.Normal);
            ul.InnerHtml += firstl.ToString(TagRenderMode.Normal);
            

            int first = pageInfo.PageNumber > 5 ? pageInfo.PageNumber - 5 : 1;
            int last = pageInfo.TotalPages - 5 > pageInfo.PageNumber ? first + 10 : pageInfo.TotalPages;
            for (int i = first; i <= last; i++)
            {
                TagBuilder li = new TagBuilder("li");
                li.AddCssClass("page-item");
                if (i == pageInfo.PageNumber)
                {
                   
                    li.AddCssClass("active");
                }
                TagBuilder a = new TagBuilder("a");
                a.MergeAttribute("href", pageUrl(i));
                a.InnerHtml = i.ToString();
                a.AddCssClass("page-link");
                li.InnerHtml += a.ToString(TagRenderMode.Normal);
                ul.InnerHtml += li.ToString(TagRenderMode.Normal);
            }
            //-------------last
            TagBuilder lastl = new TagBuilder("li");
            previous.AddCssClass("page-item");
            TagBuilder lasta = new TagBuilder("a");
            lasta.InnerHtml = "Last";
            lasta.AddCssClass("page-link");
            if (pageInfo.PageNumber == pageInfo.TotalPages)
            {

                lastl.AddCssClass("disabled");
            }
            else
            {
                lasta.MergeAttribute("href", pageUrl(pageInfo.TotalPages));
            }

            lastl.InnerHtml += lasta.ToString(TagRenderMode.Normal);
            ul.InnerHtml += lastl.ToString(TagRenderMode.Normal);
            //-------------next
            TagBuilder nextl = new TagBuilder("li");
            previous.AddCssClass("page-item");
            TagBuilder nexta = new TagBuilder("a");
            nexta.InnerHtml = "Next";
            nexta.AddCssClass("page-link");
            if (pageInfo.PageNumber == pageInfo.TotalPages)
            {

                nextl.AddCssClass("disabled");
            }
            else
            {
                nexta.MergeAttribute("href", pageUrl(pageInfo.PageNumber + 1));
            }

            nextl.InnerHtml += nexta.ToString(TagRenderMode.Normal);
            ul.InnerHtml += nextl.ToString(TagRenderMode.Normal);
            result.Append(ul.ToString());
            return MvcHtmlString.Create(result.ToString());
        }
    }
}