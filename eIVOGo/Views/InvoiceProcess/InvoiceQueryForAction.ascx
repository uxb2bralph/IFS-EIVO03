﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Register Assembly="Model" Namespace="Model.DataEntity" TagPrefix="cc1" %>
<%@ Register Src="~/Module/UI/PageAction.ascx" TagName="PageAction" TagPrefix="uc5" %>
<%@ Register Src="~/Module/UI/FunctionTitleBar.ascx" TagName="FunctionTitleBar" TagPrefix="uc6" %>

<%@ Register Src="~/Views/InquireInvoice/ByAttachment.ascx" TagPrefix="uc5" TagName="ByAttachment" %>

<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<!--交易畫面標題-->
<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", _viewModel.ActionTitle); %>
<div class="border_gray">
    <!--表格 開始-->
    <table id="queryArea" width="100%" border="0" cellpadding="0" cellspacing="0" class="left_title">
        <tr>
            <th colspan="2" class="Head_style_a">查詢條件
            </th>
        </tr>
        <% 
            ViewBag.ForPrint = false;
            ((CommonInquiry<InvoiceItem>)this.Model).Render(Html);
        %>
        <tr>
            <th>每頁資料筆數
            </th>
            <td class="tdleft">
                <input name="pageSize" type="text" value="<%= Request["pageSize"] ?? Uxnet.Web.Properties.Settings.Default.PageSize.ToString() %>" />
            </td>
        </tr> 
    </table>
    <!--表格 結束-->
</div>
<% 
    ((CommonInquiry<InvoiceItem>)this.Model).RenderAlert(Html);
%>
<!--按鈕-->
<table border="0" cellspacing="0" cellpadding="0" width="100%" class="queryAction">
    <tbody>
        <tr>
            <td class="Bargain_btn">
                <button type="button" onclick="uiInvoiceQuery.initQuery = true;uiInvoiceQuery.inquire();" >查詢</button>
            </td>
        </tr>
    </tbody>
</table>
<!--表格 開始-->
<%  Html.RenderPartial("~/Views/InvoiceProcess/ScriptHelper/Common.ascx"); %>
<%  Html.RenderPartial("~/Views/InvoiceProcess/ScriptHelper/InvoiceAction.ascx"); %>
<script runat="server">

    ModelSource<InvoiceItem> models;
    InquireInvoiceViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;
    }
</script>

