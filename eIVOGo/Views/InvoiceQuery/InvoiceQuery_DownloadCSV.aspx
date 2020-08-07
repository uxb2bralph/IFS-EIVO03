<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility" %>

<script runat="server">

    ModelSource<InvoiceItem> models;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        IQueryable<InvoiceItem> items = (IQueryable<InvoiceItem>)Model;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;

        Response.CsvDownload(GetCsvResult(items), null, "text/csv");
    }

    public IEnumerable<object> GetCsvResult(IQueryable<InvoiceItem> items)
    {        
        string systemName = System.Globalization.CultureInfo.CurrentCulture.Name; // en-US 
        if (systemName == "en-US")
        {
            return items.Select(d => new
            {
                InvoiceNumber = d.TrackCode + d.No,
                InvoiceDate = String.Format("{0:yyyy/MM/dd}", d.InvoiceDate),
                CustomerID = d.InvoiceBuyer.CustomerID,
                SerialNumber = d.InvoicePurchaseOrder != null ? d.InvoicePurchaseOrder.OrderNo : null,
                InvoiceIssuer = d.InvoiceSeller.CustomerName,
                UniformNumberOfIssuer = d.InvoiceSeller.ReceiptNo,
                UntaxedAmount = d.InvoiceAmountType.SalesAmount,
                Tax = d.InvoiceAmountType.TaxAmount,
                TaxIncludedAmount = d.InvoiceAmountType.TotalAmount,
                WhetherToWin = d.InvoiceWinningNumber != null ? d.InvoiceWinningNumber.UniformInvoiceWinningNumber.PrizeType : "N/A",
                BuyerUniformNumber = d.InvoiceBuyer.ReceiptNo != null ? d.InvoiceBuyer.ReceiptNo : "",
                Remarks = d.Remark != null ? d.Remark : "",
                VehicleNumber = d.InvoiceCarrier != null ? d.InvoiceCarrier.CarrierNo : "",
            }); 
        }
        else
        {
            return items.Select(d => new
            {
                發票號碼 = d.TrackCode + d.No,
                發票日期 = String.Format("{0:yyyy/MM/dd}", d.InvoiceDate),
                客戶ID = d.InvoiceBuyer.CustomerID,
                序號 = d.InvoicePurchaseOrder != null ? d.InvoicePurchaseOrder.OrderNo : null,
                發票開立人 = d.InvoiceSeller.CustomerName,
                開立人統編 = d.InvoiceSeller.ReceiptNo,
                未稅金額 = d.InvoiceAmountType.SalesAmount,
                稅額 = d.InvoiceAmountType.TaxAmount,
                含稅金額 = d.InvoiceAmountType.TotalAmount,
                是否中獎 = d.InvoiceWinningNumber != null ? d.InvoiceWinningNumber.UniformInvoiceWinningNumber.PrizeType : "N/A",
                買受人統編 = d.InvoiceBuyer.ReceiptNo != null ? d.InvoiceBuyer.ReceiptNo : "",
                備註 = d.Remark != null ? d.Remark : "",
                載具號碼 = d.InvoiceCarrier != null ? d.InvoiceCarrier.CarrierNo : "",
            });             
        }
    }

    public override void Dispose()
    {
        if (models != null)
            models.Dispose();

        base.Dispose();
    }

</script>
