using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model.Models.ViewModel
{
    public class AuthTokenViewModel
    {
        public String Seed { get; set; }
        public String Authorization { get; set; }
        public String SellerID { get; set; }
        public bool? SampleQuery { get; set; }
    }
}