using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class InvoicePDFWatcherForZipModel
    {
        public string Date { get; set; }
        public string FileName { get; set; }
        public string OrderNo { get; set; }
        public int Status { get; set; }
    }
}
