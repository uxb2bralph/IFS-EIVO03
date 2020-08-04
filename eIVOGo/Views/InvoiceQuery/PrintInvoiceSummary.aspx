<%@ Page Title="" Language="C#" MasterPageFile="~/template/ContentPage.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" StylesheetTheme="Visitor" %>

<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Register Src="~/Views/Module/HidePostData.ascx" TagPrefix="uc1" TagName="HidePostData" %>
<%@ Register Src="~/Views/Module/InvoiceSummaryList.ascx" TagPrefix="uc2" TagName="InvoiceSummaryList" %>

<asp:Content ID="header" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="mainContent" ContentPlaceHolderID="mainContent" runat="server">

    <uc1:HidePostData runat="server" ID="HidePostData" />
    <uc2:InvoiceSummaryList runat="server" ID="InvoiceSummaryList" />

</asp:Content>


<%--<script runat="server">

    ModelSource<InvoiceItem> models;
    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        models.DataSourcePath = VirtualPathUtility.ToAbsolute("~/InvoiceQuery/InvoiceSummaryGridPage");
    }

    public override void Dispose()
    {
        if (models != null)
            models.Dispose();

        base.Dispose();
    }

</script>--%>
