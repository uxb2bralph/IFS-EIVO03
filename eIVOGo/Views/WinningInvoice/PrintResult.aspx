<%@ Page Title="" Language="C#" MasterPageFile="~/template/ContentPage.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" StylesheetTheme="Visitor" %>

<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="eIVOGo.Controllers" %>

<asp:Content ID="header" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContent" runat="server">
    查詢條件：
    <% 
        ((CommonInquiry<InvoiceItem>)this.Model).RenderQueryMessage(Html);
        Html.RenderPartial("~/Views/Module/HidePostData.ascx");
       Html.RenderPartial("~/Views/Module/WinningInvoiceReport.ascx", models); 
    %>
</asp:Content>
<script runat="server">
    ModelSource<InvoiceItem> models;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        ViewBag.ActionName = eIVOGo.Resource.Views.Common.TreeView.首頁___發票作業;
         models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
    }

</script>
