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
<%@ Import Namespace="Business.Helper" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>電子發票系統</title>
    <link href="~/App_Themes/NewPrint/New_eivo.css" rel="stylesheet" type="text/css" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
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
</head>
<body>
    <form id="theForm" runat="server">
        <%  var profile = Context.GetUser();
            DocumentPrintQueue item;
            var items = _model.OrderBy(d => d.CDS_Document.InvoiceItem.No);
            while((item = items.FirstOrDefault())!=null)
            {
                if (_viewModel?.PrintCuttingLine == true)
                {
                    Html.RenderPartial("~/Views/DataView/Module/C0401_A4_Print.ascx", item.CDS_Document.InvoiceItem);
                }
                else
                {
                    Html.RenderPartial("~/Views/DataView/Module/C0401_A4.ascx", item.CDS_Document.InvoiceItem);
                }
                models.MarkPrintedLog(item.CDS_Document.InvoiceItem, profile);
            }   %>
    </form>
</body>
</html>

<script>
    window.onload = function () {
        window.print();
    };
</script>
<script runat="server">

    ModelSource<InvoiceItem> models;
    IQueryable<DocumentPrintQueue> _model;
    RenderStyleViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (IQueryable<DocumentPrintQueue>)this.Model;
        _viewModel = ViewBag.ViewModel as RenderStyleViewModel;
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