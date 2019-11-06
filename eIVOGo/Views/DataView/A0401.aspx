<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
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
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>電子發票系統</title>
    <link href="~/App_Themes/Ver2016/eivo.css?1.0" rel="stylesheet" type="text/css" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body style="font-family:KaiTi">
    <form id="theForm" runat="server">
        <%  String invoicePrintView;
            if (_viewModel?.UseCustomView == true && (invoicePrintView = _model.Organization.OrganizationStatus.InvoicePrintView.GetEfficientString()) != null)
            {
                Html.RenderPartial(invoicePrintView, _model);
            }
            else
            {
                Html.RenderPartial("~/Views/DataView/Module/InvoiceView2016.cshtml", _model);
            }
        %>
    </form>
</body>
</html>
<%  if (_viewModel?.ResultMode == Naming.DataResultMode.Print)
    {%>
<script>
    window.onload = function () {
        window.print();
    };
</script>
<%  } %>
<script runat="server">

    ModelSource<InvoiceItem> models;
    InvoiceItem _model;
    RenderStyleViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceItem)this.Model;
        _viewModel = (RenderStyleViewModel)ViewBag.ViewModel;
    }


</script>