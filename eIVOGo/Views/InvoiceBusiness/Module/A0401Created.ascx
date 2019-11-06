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
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="Business.Helper" %>

<%  if (_model != null)
    {%>
<div>
    <a href="<%= Url.Action("PrintSingleInvoiceAsPDF","DataView",new { keyID = _model.InvoiceID.EncryptKey() }) %>" target="_blank">列印資料下載</a>
    <br />
    <a href="<%= Url.Action("ShowInvoice","DataView",new { keyID = _model.InvoiceID.EncryptKey(),ResultMode = Naming.DataResultMode.Print }) %>" target="_blank">立即檢視列印</a>
    <br />
</div>
<script>
    $('form').resetForm();
</script>
<%  } %>

<script runat="server">

    ModelStateDictionary _modelState;
    ModelSource<InvoiceItem> models;
    InvoiceItem _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceItem)this.Model;
    }

</script>
