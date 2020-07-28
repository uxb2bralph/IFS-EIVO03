using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.DataEntity;
using Model.Locale;
using static Model.Locale.Naming;

namespace Model.Models.ViewModel
{

    public partial class InquireInvoiceViewModel : CommonQueryViewModel
    {
        //public List<InquireInvoiceItem> Results { get; set; }//amy
        public IQueryable<InvoiceItem> Results { get; set; }//amy
        public int? CompanyID { get; set; }
        public int? AgentID { get; set; }
        public String DataNo { get; set; }
        public String Consumption { get; set; }
        public String BuyerReceiptNo { get; set; }
        public String SellerReceiptNo { get; set; } //[Amy](開立人)統編
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
        public DateTime? InvoiceDate { get; set; }
        public Naming.DocumentTypeDefinition? DocType { get; set; }

        public bool? SelectAll { get; set; }
        public String SelectorIndication { get; set; }
        public String SelectorIndicationValue { get; set; }
        public String JS_OnSelect { get; set; }
        public int? BuyerID { get; set; } //[Amy]-1090611
        public String ReMark { get; set; }//[Amy]-備註欄位
        public String BuyerAddr { get; set; }  //[Amy]-買受人地址
        public String BuyerMail { get; set; }  //[Amy]-買受人Mail
        public decimal? TotalAmount { get; set; }//[Amy]-含稅金額
        public decimal? SalesAmount { get; set; }//[Amy]-未稅金額
        public decimal? TaxAmount { get; set; }//[Amy]-稅額
        public String WinningState { get; set; } //[Amy]-是否中獎
        public String OrderNO { get; set; } //[Amy]-序號

    }

    /// <summary>
    /// Amy-1090609
    /// 統計報表>發票明細查詢欄位
    /// </summary>
    public class InquireInvoiceItem
    {
        public int CompanyID;
        public int AgentID;
        public String DataNo;
        public String Consumption;
        public String BuyerReceiptNo;
        public String SellerReceiptNo;
        public String BuyerName;
        public String SellerName;
        public DateTime DateFrom;
        public DateTime DateTo;
        public bool IsWinning;
        public int Winning;
        public bool Cancelled;
        public bool IsAttached;
        public bool QueryAtStart;
        public int SellerID;
        public bool IsNoticed;
        public String CarrierType;
        public String CarrierNo;
        public String PrintMark;
        public int InvoiceID;
        public String InvoiceNo;
        public String AgencyCode;
        public String CustomerID;
        public System.Nullable<byte> ProcessType;
        public String ActionTitle;
        public int Attachment;
        public DateTime InvoiceDateFrom;
        public DateTime InvoiceDateTo;
        public DateTime InvoiceDate;
        public String ReMark;       //[Amy]-備註欄位
        public String BuyerAddr;     //[Amy]-買受人地址
        public String BuyerMail;     //[Amy]-買受人Mail        
        public decimal SalesAmount;  //[Amy]-未稅金額
        public decimal TaxAmount;    //[Amy]-稅額
        public decimal TotalAmount;  //[Amy]-含稅金額
        public String WinningState;  //[Amy]-是否中獎
        public String OrderNo;       //[Amy]-序號
        public System.Nullable<byte> DocType;
    }

    public partial class InquireNoIntervalViewModel
    {
        public int? Year { get; set; }
        public int? PeriodNo { get; set; }
        public int? SellerID { get; set; }
        public String SelectIndication { get; set; }
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
        public String Password1 { get; set; }
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

        public List<InvoiceTrackCodeItem> Results { get; set; }
    }

    public class InvoiceTrackCodeItem
    {
        public int TrackID;

        public string TrackCode;

        public short Year;

        public short PeriodNo;

        public System.Nullable<byte> InvoiceType;
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

    /// <summary>
    /// Amy-1090526
    /// 取得角色維護資料
    /// </summary>
    public partial class UserRoleDefinitionViewModel : CommonQueryViewModel
    {
        public int RoleID { get; set; }
        public String Role { get; set; }
        public String SiteMenu { get; set; }
        public List<UserRoleDefinitionItem> Results { get; set; }
    }
    /// <summary>
    /// Amy-1090526
    /// 使用者角色維護-欄位
    /// </summary>
    public class UserRoleDefinitionItem
    {
        public int RoleID;
        public string SiteMenu;
        public string Role;

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

    public class EnumSelectorViewModel
    {
        public string FieldName { get; set; }
        public string SelectorIndication { get; set; }
        public string SelectorIndicationValue { get; set; }
        public Dictionary<string, string> OptionItems { get; set; }
        public string DefaultValue { get; set; }

        public string FieldClass { get; set; }
    }
}