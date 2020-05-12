using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Locale;
using static Model.Locale.Naming;

namespace Model.Models.ViewModel
{
    public partial class InquireInvoiceViewModel : CommonQueryViewModel
    {
        public int? CompanyID { get; set; }
        public int? AgentID { get; set; }
        public String DataNo { get; set; }
        public String Consumption { get; set; }
        public String BuyerReceiptNo { get; set; }
        public String BuyerName { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? IsWinning { get; set; }
        public int? Winning { get; set; }
        public bool? Cancelled { get; set; }
        public bool? IsAttached { get; set; }
        public bool? QueryAtStart { get; set; }
        public int? SellerID { get; set; }
        public bool? IsNoticed { get; set; }
        public String CarrierType { get; set; }
        public String CarrierNo { get; set; }
        public String PrintMark { get; set; }
        public int? InvoiceID { get; set; }
        public String InvoiceNo { get; set; }
        public String AgencyCode { get; set; }
        public String CustomerID { get; set; }
        public Naming.InvoiceProcessType? ProcessType { get; set; }
        public String ActionTitle { get; set; }
        public int? Attachment { get; set; }
        public DateTime? InvoiceDateFrom { get; set; }
        public DateTime? InvoiceDateTo { get; set; }
        public Naming.DocumentTypeDefinition? DocType { get; set; }
    }

    public partial class InquireNoIntervalViewModel
    {
        public int? Year { get; set; }
        public int? PeriodNo { get; set; }
        public int? SellerID { get; set; }
        public String SelectIndication { get; set; }
        public bool? BranchRelation { get; set; }
    }

    public partial class BusinessRelationshipViewModel
    {
        public int? CompanyID { get; set; }
        public String ReceiptNo { get; set; }
        public String CompanyName { get; set; }
        public String ContactEmail { get; set; }
        public String Phone { get; set; }
        public String CustomerNo { get; set; }
        public String Addr { get; set; }
        public int? CompanyStatus { get; set; }
        public int? BusinessType { get; set; }
        public bool? Entrusting { get; set; }
        public bool? EntrustToPrint { get; set; }
        public int? MasterID
        {
            get => CompanyID;
            set => CompanyID = value;
        }
    }

    public partial class UserProfileViewModel : QueryViewModel
    {
        public int? SellerID { get; set; }
        public int? UID { get; set; }
        public String PID { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String Password1 {get;set;}
        public String EMail { get; set; }
        public String Address { get; set; }
        public String Phone { get; set; }
        public String MobilePhone { get; set; }
        public String Phone2 { get; set; }
        public bool? WaitForCheck { get; set; }
        public Naming.RoleID? DefaultRoleID { get; set; }
        public Guid? ResetID { get; set; }
        public bool? ResetPass { get; set; }
    }

    public partial class InvoiceNoIntervalViewModel : CommonQueryViewModel
    {
        public int? IntervalID { get; set; }
        public int? TrackID { get; set; }
        public int? SellerID { get; set; }
        public int? StartNo { get; set; }
        public int? EndNo { get; set; }
        public int? Parts { get; set; }
    }

    public partial class CommonQueryViewModel : QueryViewModel
    {
        public String ResultAction { get; set; }
        public String QueryAction { get; set; }
        public String Title { get; set; }
        public String FieldName { get; set; }
        public String CommitAction { get; set; }
    }

    public partial class UserAccountQueryViewModel : CommonQueryViewModel
    {
        public int? SellerID { get; set; }
        public String PID { get; set; }
        public String UserName { get; set; }
        public int? RoleID { get; set; }
        public int? LevelID { get; set; }
    }

    public partial class TrackCodeQueryViewModel : CommonQueryViewModel
    {
        public int? Year { get; set; }
        public int? PeriodNo { get; set; }
    }

    public partial class TrackCodeViewModel
    {
        public int? TrackID { get; set; }
        public int? id
        {
            get => TrackID;
            set => TrackID = value;
        }
        public short? Year { get; set; }
        public short? PeriodNo { get; set; }
        public String TrackCode { get; set; }
        public Naming.InvoiceTypeDefinition? InvoiceType { get; set; }

    }

    public partial class WinningNumberViewModel
    {
        public int? WinningID { get; set; }
        public int? Year { get; set; }
        public int? Period { get; set; }
        public int? Rank { get; set; }
        public String WinningNo { get; set; }
    }

    public partial class MailTrackingViewModel
    {
        public String StartNo { get; set; }
        public String EndNo { get; set; }
        public int? DeliveryStatus { get; set; }
        public Naming.ChannelIDType? ChannelID { get; set; }
        public int? Attachment { get; set; }

    }

    public class QueryViewModel
    {
        public int? PageSize { get; set; } = Uxnet.Web.Properties.Settings.Default.PageSize;
        public int? PageIndex { get; set; }
        public int? PageOffset { get; set; } = 0;
        public string[] SortName { get; set; }
        public int?[] SortType { get; set; }
        public bool? Paging { get; set; }
        public String KeyID { get; set; }
        public String FileDownloadToken { get; set; }
        public int?[] Sort { get; set; }
        public String DialogID { get; set; }
        public DataResultMode? ResultMode { get; set; }
        public int? RecordCount { get; set; }
        public String OnPageCallScript { get; set; }
        public bool? ForTest { get; set; }
        public String QueryForm { get; set; }
        public bool? StartQuery { get; set; }
        public String ResultView { get; set; }
        public bool? Encrypt { get; set; }
        public String QuickSearch { get; set; }
        public Naming.FieldDisplayType? DisplayType { get; set; }

    }

    public class BusinessRelationshipQueryViewModel : CommonQueryViewModel
    {
        public int? CompanyID { get; set; }
        public String ReceiptNo { get; set; }
        public String CompanyName { get; set; }
        public int? CompanyStatus { get; set; }
        public bool? EntrustToPrint { get; set; }
        public bool? Entrusting { get; set; }
        public int? BusinessType { get; set; }
        public String BranchNo { get; set; }
    }

    public class DocumentQueryViewModel : QueryViewModel
    {
        public int? id
        {
            get
            {
                return DocID;
            }

            set
            {
                DocID = value;
            }
        }
        public int? DocID { get; set; }
        public String Reason { get; set; }
        public bool? NameOnly { get; set; }
        public bool? AppendAttachment { get; set; }
        public int[] ChkItem { get; set; }
    }

    public class ExceptionLogQueryViewModel : QueryViewModel
    {
        public int? CompanyID { get; set; }
        public int? MaxLogID { get; set; }
    }

    public class RenderStyleViewModel : DocumentQueryViewModel
    {
        public bool? PrintBack { get; set; }
        public bool? PrintCuttingLine { get; set; }
        public String PaperStyle { get; set; }
        public Naming.InvoiceProcessType? ProcessType { get; set; }
        public bool? PrintBuyerAddr { get; set; }
        public bool? UseCustomView { get; set; }
        public bool? CreateNew { get; set; }
    }

    public class InvoiceRequestViewModel : QueryViewModel
    {
        public int? AgentID { get; set; }
        public int? Sender { get; set; }
        public int? TaskID { get; set; }
        public String Comment { get; set; }
        public InvoiceProcessType? ProcessType { get; set; }
    }

    public class ActionResultViewModel
    {
        public bool? Result { get; set; }
        public String Message { get; set; }
        public String[] ErrorCode { get; set; }
    }

    public class ProcessRequestQueryViewModel : QueryViewModel
    {
        public int? TaskID { get; set; }
        public DateTime? SubmitDateFrom { get; set; }
        public DateTime? SubmitDateTo { get; set; }
        public DateTime? ProcessStartFrom { get; set; }
        public DateTime? ProcessStartTo { get; set; }
        public DateTime? ProcessCompleteFrom { get; set; }
        public DateTime? ProcessCompleteTo { get; set; }
    }

    public class ProductCatalogQueryViewModel : QueryViewModel
    {
        public int? ProductID { get; set; }
        public string Barcode { get; set; }
        public string ProductName { get; set; }
        public string Spec { get; set; }
        public string PieceUnit { get; set; }
        public string Remark { get; set; }
        public decimal? SalePrice { get; set; }
        public int? SupplierID { get; set; }
    }


}