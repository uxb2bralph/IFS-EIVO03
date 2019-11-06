<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="eIVOGo.Module.Common" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Business.Helper" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<div>
    <a href="<%= Url.Action(_viewModel.ProcessType == Naming.InvoiceProcessType.B0401
            ? "PrintB0401AsPDF"
            : "PrintD0401AsPDF","DataView") %>" target="_blank">列印資料下載</a>
    <br />
    <a href="<%= Url.Action(_viewModel.ProcessType == Naming.InvoiceProcessType.B0401
            ? "PrintB0401"
            : "PrintD0401","DataView") %>" target="_blank">網頁列印</a>
</div>


<script runat="server">

    RenderStyleViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _viewModel = (RenderStyleViewModel)ViewBag.ViewModel;
    }

</script>
