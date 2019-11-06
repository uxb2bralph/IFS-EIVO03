<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Web.Optimization" %>
<%@ Import Namespace="Business.Helper" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.Security.MembershipManagement" %>

<!DOCTYPE html>
<html>
<head>
    <%= Styles.Render("~/App_Themes/POSPrint") %>
    <style type="text/css">
        div.fspace
        {
            height: 8.8cm;
        }
        div.bspace
        {
            height: 8.9cm;
        }
        
        body, td, th
        {
            font-family: Verdana, Arial, Helvetica, sans-serif, "細明體" , "新細明體";
        }

        body {
            margin: 0px;
            margin-left: 0cm;
            margin-right: 0cm;
        }      

    </style>
    <title>電子發票系統</title>
    <meta http-equiv="content-type" content="text/html; charset=UTF-8" />    
</head>
<body>
    <script src="<%= VirtualPathUtility.ToAbsolute("~/Scripts/css3-multi-column.js") %>" type="text/javascript"></script>
    <%  Html.RenderPartial("~/Views/Module/InvoicePOSPrintView.ascx"); %>
</body>
</html>
<%  if (_model != null)
    { 
        models.MarkPrintedLog(_model, _profile);    %>
<script>

    alert('發票已開立，準備列印!!\r\n***請選取雙面列印模式***');
    window.parent.document.getElementsByTagName('form')[1].reset();

    (function () {

        function printFrame() {
            if (document.queryCommandSupported('print')) {
                document.execCommand('print', false, null);
            }
            else {
                //window.parent.<framename>.focus();
                //window.print();
                self.focus();
                self.print();
            }
        }

        var beforePrint = function () {
        };

        var afterPrint = function () {
            window.close();
        };

        if (window.matchMedia) {
            var mediaQueryList = window.matchMedia('print');
            mediaQueryList.addListener(function (mql) {
                if (mql.matches) {
                    beforePrint();
                } else {
                    afterPrint();
                }
            });
        }

        window.onbeforeprint = beforePrint;
        window.onafterprint = afterPrint;

        window.onload = function () {
            printFrame();
            //window.close();
        };
    })();
</script>
<%  } %>
<script runat="server">

    InvoiceViewModel _viewModel;
    ModelSource<InvoiceItem> models;
    InvoiceItem _model;
    UserProfileMember _profile;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _viewModel = (InvoiceViewModel)ViewBag.ViewModel;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;

        _model = this.Model as InvoiceItem;
        _profile = Context.GetUser();
        
    }
</script>