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
    <link href="~/App_Themes/Ver2016/eivo.css?1.0" rel="stylesheet" type="text/css" />
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />
</head>
<body style="font-family:KaiTi">
    <form id="theForm" runat="server">
        <%  var profile = Context.GetUser();
            DocumentPrintQueue item;
            while((item = _model.FirstOrDefault())!=null)
            {
                Html.RenderPartial("~/Views/DataView/Module/InvoiceView2016.cshtml", item.CDS_Document.InvoiceItem);
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

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (IQueryable<DocumentPrintQueue>)this.Model;
    }


</script>