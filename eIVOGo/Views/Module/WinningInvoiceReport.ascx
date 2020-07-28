<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/JsGrid/JsGridInitialization.ascx" TagPrefix="uc1" TagName="JsGridInitialization" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Module.Base" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%@ Import Namespace="eIVOGo.Resource.Views.Module" %>

<div id="fieldsContainer"></div>
<div id="jsGrid"></div>
<uc1:JsGridInitialization runat="server" ID="gridInit" FieldName="fields" JsGridSelector="#jsGrid" FieldsContainerSelector="#fieldsContainer" />
<script>

            var fields = [

                {
                    "name": "SellerReceiptNo",
                    "type": "text",
                    "title": "<%=WinningInvoiceReport.統編%>",
                    "width": "80",
                    "align": "center"
                },
                {
                    "name": "SellerName",
                    "type": "text",
                    "title": "<%=WinningInvoiceReport.開立發票營業人%>",
                    "width": "160",
                    "align": "left"
                },
                {
                    "name": "Addr",
                    "type": "text",
                    "title": "<%=WinningInvoiceReport.營業人地址%>",
                    "width": "360",
                    "align": "left"
                },
            {
                "name": "WinningCount",
                "type": "text",
                "title": "<%=WinningInvoiceReport.中獎張數%>",
                "width": "80",
                "align": "left"
            },
            {
                "name": "DonationCount",
                "type": "text",
                "title": "<%=WinningInvoiceReport.捐贈張數%>",
                "width": "80",
                "align": "left"
            }
    ];

        </script>

<script runat="server">
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        gridInit.DataSourceUrl = ((ModelSource<InvoiceItem>)Model).DataSourcePath;
        gridInit.GetRecordCount = () =>
            {
                return ((ModelSource<InvoiceItem>)Model).Items
                    .GroupBy(i => i.SellerID)
                    .Count();
            };
        gridInit.AllowPaging = ((ModelSource<InvoiceItem>)Model).ResultModel == Naming.DataResultMode.Display;
        gridInit.PrintMode = ((ModelSource<InvoiceItem>)Model).ResultModel == Naming.DataResultMode.Print;
    }
</script>
