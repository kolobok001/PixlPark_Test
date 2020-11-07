using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Test_PixlPark.Models
{
    public class DeliveryAddress
    {
        public string ZipCode { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Description { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
    }
}