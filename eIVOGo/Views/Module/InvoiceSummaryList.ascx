<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Src="~/Module/JsGrid/JsGridInitialization.ascx" TagPrefix="uc1" TagName="JsGridInitialization" %>
<%@ Register Src="~/Module/JsGrid/DataField/InvoiceNo.ascx" TagPrefix="uc1" TagName="InvoiceNo" %>
<%@ Register Src="~/Module/JsGrid/DataField/Attachment.ascx" TagPrefix="uc1" TagName="Attachment" %>
<%@ Register Src="~/Module/JsGrid/DataField/CheckBox.ascx" TagPrefix="uc1" TagName="CheckBox" %>
<%@ Register Src="~/Module/JsGrid/DataField/JsGridField.ascx" TagPrefix="uc1" TagName="JsGridField" %>

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
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Resource.Views.Module" %>

<div id="fieldsContainer"></div>
<div id="jsGrid"></div>
<uc1:JsGridInitialization runat="server" ID="gridInit" FieldName="fields" JsGridSelector="#jsGrid" FieldsContainerSelector="#fieldsContainer" />
<asp:PlaceHolder ID="phFields" runat="server">
    <script>
        var fields = [];
    </script>
    <uc1:JsGridField runat="server" ID="SellerName" FieldVariable="fields" name="SellerName" title="開立發票營業人" width="240" align="left" />
    <uc1:JsGridField runat="server" ID="SellerReceiptNo" FieldVariable="fields" name="SellerReceiptNo" title="統編" width="240" align="center" footerTemplate="function () { return '總筆數：'; }" />
    <script>
        <%--fields[fields.length] = {
            "name": "LiveDate",
            "type": "text",
            "title": "上線日期",
            "width": "200",
            "align": "center"
        };--%>
        fields[fields.length] = {
            "name": "RecordCount",
            "type": "text",
            "title": "<%#InvoiceSummaryList.資料筆數%>",
            "width": "200",
            "align": "right",
            footerTemplate: function () { return "<%= String.Format("{0:##,###,###,##0.##}", models.Items.Count()) %>"; }
        };
    </script>
</asp:PlaceHolder>

<script runat="server">

    ModelSource<InvoiceItem> models;

    protected void Page_Load(object sender, EventArgs e)
    {
        SellerName.title = InvoiceSummaryList.開立發票營業人;
        SellerReceiptNo.title = InvoiceSummaryList.統編;
        var count = InvoiceSummaryList.總筆數_;
        SellerReceiptNo.footerTemplate="function () { return '" +count +"'}";        
    }

    protected override void OnInit(EventArgs e)
    {        
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        models.DataSourcePath = VirtualPathUtility.ToAbsolute("~/InvoiceQuery/InvoiceSummaryGridPage");

        gridInit.DataSourceUrl =   models.DataSourcePath.ToString();
        gridInit.GetRecordCount = () =>
        {
            return models.Items           
                .GroupBy(i => i.SellerID)
                .Count();
        };
        gridInit.AllowPaging = models.ResultModel == Naming.DataResultMode.Display;
        gridInit.PrintMode = models.ResultModel == Naming.DataResultMode.Print;
    }

    public override void Dispose()
    {
        if (models != null)
            models.Dispose();

        base.Dispose();
    }

</script>
