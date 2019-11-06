using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Locale;

namespace Model.Models.ViewModel
{
    public class OrganizationViewModel : QueryViewModel
    {
        public String ContactName { get; set; }
        public String Fax { get; set; }
        public String LogoURL { get; set; }
        public String CompanyName { get; set; }
        public int? CompanyID { get; set; }
        public String ReceiptNo { get; set; }
        public String Phone { get; set; }
        public String ContactFax { get; set; }
        public String ContactPhone { get; set; }
        public String ContactMobilePhone { get; set; }
        public String RegAddr { get; set; }
        public String UndertakerName { get; set; }
        public String Addr { get; set; }
        public String EnglishName { get; set; }
        public String EnglishAddr { get; set; }
        public String EnglishRegAddr { get; set; }
        public String ContactEmail { get; set; }
        public String UndertakerPhone { get; set; }
        public String UndertakerFax { get; set; }
        public String UndertakerMobilePhone { get; set; }
        public String InvoiceSignature { get; set; }
        public String UndertakerID { get; set; }
        public String ContactTitle { get; set; }
        public int? CategoryID { get; set; }
        public int? CurrentLevel { get; set; }
        public DateTime? LastTimeToAcknowledge { get; set; }
        public int? RequestPeriodicalinterval { get; set; }
        public bool? SetToPrintInvoice { get; set; }
        public string InvoicePrintView { get; set; }
        public bool? IronSteelIndustry { get; set; }
        public bool? Entrusting { get; set; }
        public string AuthorizationNo { get; set; }
        public Guid TokenID { get; set; }
        public bool? SetToOutsourcingCS { get; set; }
        public string AllowancePrintView { get; set; }
        public bool? SetToNotifyCounterpartBySMS { get; set; }
        public bool? DownloadDataNumber { get; set; }
        public bool? DownloadDispatch { get; set; }
        public bool? UploadBranchTrackBlank { get; set; }
        public bool? PrintAll { get; set; }
        public Naming.InvoiceTypeDefinition? SettingInvoiceType { get; set; }
        public bool? SubscribeB2BInvoicePDF { get; set; }
        public bool? UseB2BStandalone { get; set; }
        public bool? DisableIssuingNotice { get; set; }
        public bool? DisableWinningNotice { get; set; }
        public bool? EntrustToPrint { get; set; }
        public bool? EnableTrackCodeInvoiceNoValidation { get; set; }
        public Naming.InvoiceNoticeStatus? NoticeSetting { get; set; }
        public int[] NoticeStatus { get; set; }
        public bool? IgnoreDuplicatedDataNumber { get; set; }
        public Naming.InvoiceProcessType? DefaultProcessType { get; set; }

    }

}