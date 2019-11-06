<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
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
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>
<%--發票錯誤訊息--%>
<div align="center">
        下列資料由商家上傳時,發生錯誤,請處理!!</div>
<%  Html.RenderPartial("~/Views/Notification/Module/InvoiceException.ascx", _model.Where(g => g.TypeID == (int)Naming.DocumentTypeDefinition.E_Invoice)); %>
<%  Html.RenderPartial("~/Views/Notification/Module/InvoiceCancellationException.ascx", _model.Where(g => g.TypeID == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation)); %>
<%  Html.RenderPartial("~/Views/Notification/Module/AllowanceException.ascx", _model.Where(g => g.TypeID == (int)Naming.DocumentTypeDefinition.E_Allowance)); %>
<%  Html.RenderPartial("~/Views/Notification/Module/AllowanceCancellationException.ascx", _model.Where(g => g.TypeID == (int)Naming.DocumentTypeDefinition.E_Allowance)); %>

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
