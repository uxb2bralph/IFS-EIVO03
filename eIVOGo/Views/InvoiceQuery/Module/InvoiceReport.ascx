<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc5" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc6" %>





<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<!--交易畫面標題-->
<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "發票報表匯出"); %>
<div class="border_gray">
    <!--表格 開始-->
    <table width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <th colspan="2" class="Head_style_a">查詢條件
            </th>
        </tr>
        <% 
            ((CommonInquiry<InvoiceItem>)this.Model).Render(Html);
        %>
        <%
            Html.RenderPartial("~/Views/InquireInvoice/ByInvoiceNo.cshtml");
            Html.RenderPartial("~/Views/InquireInvoice/ByWinningNumber.cshtml");
            Html.RenderPartial("~/Views/InquireInvoice/ByCancellation.cshtml");
            %>
    </table>
    <!--表格 結束-->
</div>
<% 
    ((CommonInquiry<InvoiceItem>)this.Model).RenderAlert(Html);
%>
<!--按鈕-->
<table width="100%" border="0" cellspacing="0" cellpadding="0">
    <tr>
        <td class="Bargain_btn">
            <input type="button" value="查詢" name="btnQuery" class="btn" onclick="$('form').prop('action', '<%= Url.Action(ViewBag.QueryAction) %>').submit();" />
            <input type="button" value="下載月報表" name="btnMonthlyReport" class="btn" onclick="downloadMonthlyReport();" />
        </td>
    </tr>
</table>
<script>
    function downloadMonthlyReport() {
        var event = event || window.event;
        var $form = $(event.target).closest('form');
        $form.launchDownload('<%= Url.Action("CreateMonthlyReportXlsx","InvoiceQuery") %>', null, 'report');
    }

    $(function() {
        var $opts = $('form select[name="sellerID"] option');
        if($opts.length==2) {
            $opts.eq(0).remove();
        }
    });
</script>
<!--表格 開始-->
<script runat="server">

    ModelSource<InvoiceItem> models;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        //if (ViewBag.HasQuery)
        //{
        //    ModelSource<InvoiceItem> models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;

        //    if (((ASP.module_inquiry_inquiryitem_inquireinvoiceconsumption_ascx)inquireConsumption).QueryForB2C)
        //    {
        //        models.Items = models.Items.Where(i => i.DonateMark == "0"
        //            && (i.PrintMark == "Y" || (i.PrintMark == "N" && i.InvoiceWinningNumber != null))
        //            && !i.CDS_Document.DocumentPrintLog.Any(l => l.TypeID == (int)Naming.DocumentTypeDefinition.E_Invoice));
        //    }

        //    result.Visible = true;
        //}
    }
</script>

