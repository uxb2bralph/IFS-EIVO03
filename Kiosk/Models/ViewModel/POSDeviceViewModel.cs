using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Kiosk.Models.ViewModel
{
    public class POSDeviceViewModel
    {
        public String company_id { get; set; }
        public int? quantity { get; set; }

    }

    public class POSInvoiceViewModel
    {
        public String sn { get; set; }
        public decimal? amount { get; set; }
        public DateTime? time { get; set; }
        public String buyer { get; set; }
        public string carrier { get; set; }
        public string carrier_type { get; set; }
        public int printed { get; set; }
    }

}