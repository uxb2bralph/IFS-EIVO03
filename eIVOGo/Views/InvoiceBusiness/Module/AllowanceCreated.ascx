<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="Business.Helper" %>
<%@ Import Namespace="Model.Helper" %>

<%  if (_model != null)
    { %>
<div>
    折讓證明單已開立，<br />
    號碼：<%= _model.AllowanceNumber %><br />
    <a href="<%= Url.Action("PrintSingleAllowanceAsPDF", "DataView", new { keyID = _model.AllowanceID.EncryptKey() }) %>" target="_blank">列印資料下載</a>
    <br />
    <a href="<%= Url.Action("ShowAllowance", "DataView", new { keyID = _model.AllowanceID.EncryptKey(), ResultMode = Naming.DataResultMode.Print }) %>" target="_blank">立即檢視列印</a>
    <br />
</div>
<%  } %>
<script>
    $global.removeTab('issueAllowance');
</script>

<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    InvoiceAllowance _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceAllowance)this.Model;
    }

</script>
