<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<%  var recordCount = _model.Count();
    if (recordCount > 0)
    {
        Html.RenderPartial("~/Views/AllowanceProcess/Module/AllowanceItemTable.ascx", _model);
        _viewModel.RecordCount = recordCount;
        _viewModel.OnPageCallScript = "uiAllowanceQuery.inquire(page)";
        Html.RenderPartial("~/Views/Common/Module/Pagination.ascx");
    } %>

<script runat="server">


    ModelSource<InvoiceItem> models;
    IQueryable<InvoiceAllowance> _model;
    InquireInvoiceViewModel _viewModel;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;

        _model = (IQueryable<InvoiceAllowance>)this.Model;
    }

</script>

