using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test_PixlPark.Models
{
    public class Order
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string TrackingUrl { get; set; }
        public object TrackingNumber { get; set; }
        public string Status { get; set; }
        public DeliveryAddress DeliveryAddress { get; set; }
        public double Price { get; set; }
        public double DeliveryPrice { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateCreated { get; set; }

    }
}