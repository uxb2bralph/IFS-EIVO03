using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Model.Models.ViewModel
{
    public class POSDeviceViewModel : AuthTokenViewModel
    {
        [JsonIgnore]
        public String company_id
        {
            get => SellerID;
            set => SellerID = value;
        }

        public int? quantity { get; set; }

        public int? Booklet { get; set; }
    }
}