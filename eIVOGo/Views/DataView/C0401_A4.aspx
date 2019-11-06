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
    <style type="text/css">
        div.fspace
        {
            height: 9.0cm;
        }
        div.bspace
        {
            height: 9.0cm;
        }
        
        body, td, th
        {
            font-family: Verdana, Arial, Helvetica, sans-serif, "細明體" , "新細明體";
        }      

    </style>
    <title>電子發票系統</title>
        <link href="~/App_Themes/NewPrint/New_eivo.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />    
</head>
<body>
    <form id="theForm" runat="server">
        <%  String invoicePrintView;
            if (_viewModel?.UseCustomView == true && (invoicePrintView = _model.Organization.OrganizationStatus.InvoicePrintView.GetEfficientString()) != null)
            {
                Html.RenderPartial(invoicePrintView, _model);
            }
            else
            {
                if(_viewModel.PrintCuttingLine==true)
                {
                    Html.RenderPartial("~/Views/DataView/Module/C0401_A4_Print.ascx", _model);
                }
                else
                {
                    Html.RenderPartial("~/Views/DataView/Module/C0401_A4.ascx", _model);
                }
            }
            %>
    </form>
</body>
</html>
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

        if (_viewModel == null)
        {
            _viewModel = this.TempData["viewModel"] as RenderStyleViewModel;
            if(_viewModel!=null)
            {
                ViewBag.ViewModel = _viewModel;
            }
        }
    }


</script>