<%@ Page Title="" Language="C#" MasterPageFile="~/template/MvcMainPage.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<asp:Content ID="header" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="formContent" runat="server">
    <%--<%  Html.RenderAction("ApplyPOSDevice", "InvoiceBusiness", new { id = 2364 }); %>--%>
        <%--<%  Html.RenderPartial("~/Views/Forms/SimpleInvoice.ascx"); %>--%>
</asp:Content>
<script runat="server">

    ModelSource<InvoiceItem> models;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        doTask();
    }

    void doTask()
    {
        switch(Request["task"])
        {
            case "issuingNotice":
                var items = models.GetTable<InvoiceItem>().Where(i => i.SellerID == 2359 && i.InvoiceDate >= DateTime.Today)
                        .Select(i => i.InvoiceID).ToArray();
                items.SendIssuingNotification(true);
                break;
        }
    }

</script>