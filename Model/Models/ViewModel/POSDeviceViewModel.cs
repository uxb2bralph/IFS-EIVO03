using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model.Models.ViewModel
{
    public class POSDeviceViewModel
    {
        public String company_id { get; set; }
        public int? quantity { get; set; }
        public String SellerID
        {
            get => company_id;
            set => company_id = value;
        }
        public int? Booklet { get; set; }
        public String Seed { get; set; }
        public String Authorization { get; set; }
    }
}