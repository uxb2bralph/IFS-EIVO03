<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Web.Mvc" %>
<%@ Import Namespace="System.Security.Cryptography.X509Certificates" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%  foreach (var item in _model)
    {   %>
<option><%= item.Subject %></option>
<%  } %>
<script runat="server">

    ModelStateDictionary _modelState;
    CertQueryViewModel _viewModel;
    IEnumerable<X509Certificate2> _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        _modelState = (ModelStateDictionary)ViewBag.ModelState;
        _viewModel = (CertQueryViewModel)ViewBag.ViewModel;
        _model = (IEnumerable<X509Certificate2>)this.Model;
    }

</script>
