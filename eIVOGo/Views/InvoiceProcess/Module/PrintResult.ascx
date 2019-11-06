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
    <a href="<%= Url.Action(_viewModel.ProcessType == Naming.InvoiceProcessType.A0401
            ? "PrintA0401AsPDF"
            : "PrintC0401AsPDF","DataView",_viewModel) %>" target="_blank">列印資料下載(PDF合併)</a>
    <%
        var viewModel = new
        {
            _viewModel.CreateNew,
            _viewModel.KeyID,
            _viewModel.NameOnly,
            _viewModel.PaperStyle,
            _viewModel.PrintBack,
            _viewModel.PrintBuyerAddr,
            _viewModel.PrintCuttingLine,
            _viewModel.ProcessType,
        };%>
    <a href='javascript:$("").launchDownload("<%= Url.Action("ZipInvoicePDF","DataView") %>", <%= JsonConvert.SerializeObject(viewModel) %>, "report", true);'>列印資料下載(ZIP)</a>
    <br />
    <a href="<%= Url.Action(_viewModel.ProcessType == Naming.InvoiceProcessType.A0401
            ? "PrintA0401"
            : "PrintC0401","DataView",viewModel) %>" target="_blank">網頁列印</a>
    <%      if(_viewModel.PrintBack == true)
            { %>
    <br />
    <span style="color: blue;">***列印時請記得選取雙面列印模式***</span>
    <%      }   %>
</div>


<script runat="server">

    RenderStyleViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _viewModel = (RenderStyleViewModel)ViewBag.ViewModel;
        _viewModel.PrintCuttingLine = true;
        _viewModel.CreateNew = true;
    }

</script>
