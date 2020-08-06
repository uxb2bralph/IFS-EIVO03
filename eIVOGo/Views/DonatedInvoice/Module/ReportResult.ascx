<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc6" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Module.Base" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%@ Import Namespace="eIVOGo.Resource.Views.DonatedInvoice.Module" %>

<% Html.RenderPartial("Module/InquireDonation", Model); %>
<% if (!models.InquiryHasError)
    {  %>
<uc6:FunctionTitleBar ID="resultTitle" runat="server" ItemName="查詢結果" />
<div class="border_gray">
    <% 
        Html.RenderPartial("~/Views/Module/DonatedInvoiceReport.ascx", models);
    %>
    <!--按鈕-->
</div>
<table id="tblAction" width="100%" border="0" cellspacing="0" cellpadding="0">
    <tr>
        <td class="Bargain_btn">
            <% Html.RenderPartial("~/Views/Module/PrintData.ascx"); %>
            <input type="button" value="<%=ReportResult.CSV下載 %>" name="btnCsv" class="btn" onclick="$('form').prop('action', '<%= Url.Action("DownloadCSV") %>').submit();" />
        </td>
    </tr>
</table>
<% } %>
<script runat="server">
    ModelSource<InvoiceItem> models;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        models.DataSourcePath = VirtualPathUtility.ToAbsolute("~/DonatedInvoice/ReportGridPage");

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        resultTitle.ItemName = ReportResult.查詢結果;
    }

</script>
