using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model.Locale;

namespace Model.DataEntity
{
    public partial class DataDefinition
    {
    }

    public partial class InvoiceDeliveryTracking
    {
        public int? DuplicateCount { get; set; }
        public bool MergedItem { get; set; }
    }

    public partial class InvoiceItem
    {
        public int? PackageID { get; set; }
    }


    public class NotifyToProcessID
    {
        public int? MailToID { get; set; }
        public Organization Seller { get; set; }
        public String itemNO { get; set; }
        public int? DocID { get; set; }
        public String Subject { get; set; }
    }

    public class NotifyMailInfo
    {
        public bool? isMail { get; set; }
        public InvoiceItem InvoiceItem { get; set; }
    }

    public class InvoiceEntity
    {
        public InvoiceItem MainItem { get; set; }
        public List<InvoiceProduct> ItemDetails { get; set; }
        public Naming.UploadStatusDefinition? Status { get; set; }
        public String Reason { get; set; }
    }
}
