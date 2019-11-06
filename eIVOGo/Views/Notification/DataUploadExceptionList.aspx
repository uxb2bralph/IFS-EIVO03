<%@ Page Title="" Language="C#" MasterPageFile="~/template/ContentPage.Master" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.DataEntity" %>

<asp:Content ID="header" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="dataContent" ContentPlaceHolderID="dataContent" runat="server">
    <% 
        Html.RenderPartial("~/Views/Notification/Module/UploadDataExceptionNotification.ascx", _model);
    %>
</asp:Content>

<script runat="server">

    ModelSource<InvoiceItem> models;
    IQueryable<ExceptionLog> _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;

        _model = (IQueryable<ExceptionLog>)this.Model;

    }
</script>