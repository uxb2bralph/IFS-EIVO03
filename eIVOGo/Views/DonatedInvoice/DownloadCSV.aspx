<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Web.Script.Serialization" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="eIVOGo.Resource.Views.DonatedInvoice" %>

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
                LoveCode = d.InvoiceDonation != null ? d.InvoiceDonation.AgencyCode : "",
                InvoicingBusinessPerson = d.InvoiceSeller.CustomerName,
                InvoiceNumber = d.TrackCode + d.No,
                WhetherToWin = d.InvoiceWinningNumber != null ? DownloadCSV.是 : DownloadCSV.否
            });
        }else
        {
            return items.Select(d => new
            {
                愛心碼 = d.InvoiceDonation != null ? d.InvoiceDonation.AgencyCode : "",
                開立發票營業人 = d.InvoiceSeller.CustomerName,
                發票號碼 = d.TrackCode + d.No,
                是否中獎 = d.InvoiceWinningNumber != null ? DownloadCSV.是 : DownloadCSV.否
            });
        }

        return obj;
    }

    public override void Dispose()
    {
        if (models != null)
            models.Dispose();

        base.Dispose();
    }

</script>