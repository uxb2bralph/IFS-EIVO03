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

<%  Html.RenderPartial("~/Views/SiteAction/FunctionTitleBar.cshtml", "查詢結果"); %>

<div class="border_gray" style="overflow-x: auto;">
    <%  var recordCount = _model.Count();
        if (recordCount > 0)
        { %>
    <%  Html.RenderPartial("~/Views/InvoiceProcess/Module/QueryPaging.ascx", _model); %>
    <%      if(ViewBag.ResultAction!=null)
            {
                Html.RenderPartial((String)ViewBag.ResultAction);
            } %>
    <%  }
        else
        { %>
    <font color="red">查無資料!!</font>
    <%  } %>
    <%--<%  var cmd = models.GetCommand(_model); %>
    <%= cmd.CommandText %>--%>
</div>

<script runat="server">


    ModelSource<InvoiceItem> models;
    IQueryable<InvoiceItem> _model;
    InquireInvoiceViewModel _viewModel;


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _viewModel = (InquireInvoiceViewModel)ViewBag.ViewModel;

        _model = (IQueryable<InvoiceItem>)this.Model;
    }

</script>

