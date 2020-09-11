using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Crmf;

namespace DataSystemsConversionTool.Models
{
    public partial class DataSystemsConversionModel
    {
        public String BatchFileName { get; set; }
        public String SourceInvoiceDataDtsx { get; set; }
        public String SourceInvoiceDetailDataDtsx { get; set; }
        public String InvoiceRequestTemp { get; set; }
        public String InvoiceRequestSample { get; set; }
        public String DestinationFileName { get; set; }
        public String ERPFileName { get; set; }
    }
}
