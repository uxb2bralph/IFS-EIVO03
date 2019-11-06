<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.Linq.Expressions" %>
<%@ Import Namespace="System.Web.Mvc.Html" %>
<%@ Import Namespace="System.Xml" %>
<%@ Import Namespace="Newtonsoft.Json" %>
<%@ Import Namespace="eIVOGo.Controllers" %>
<%@ Import Namespace="eIVOGo.Helper" %>
<%@ Import Namespace="eIVOGo.Models" %>
<%@ Import Namespace="eIVOGo.Models.ViewModel" %>
<%@ Import Namespace="Model.Models.ViewModel" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Model.Schema.EIVO" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<div>
    <%  if (_model.LogoURL != null)
        { %>
    <img id="logo" style="max-width: 200px; height: auto;" src='<%= eIVOGo.Properties.Settings.Default.WebApDomain + VirtualPathUtility.ToAbsolute("~/" +_model.LogoURL) %>' />
    <%  }
        if (_model.OrganizationStatus.InvoiceNoticeSetting.CheckNotice(Naming.InvoiceNoticeStatus.UseCustomStyle))
        {   %>
    <span style="white-space: nowrap; color: #4E158A;font-size:xx-large;font-weight:bold;position:absolute;"><%= ViewBag.MailTitle %></span>
    <%  }
        else
        {   %>
    <span style="white-space: nowrap;"><strong><%= _model.CompanyName %><%= ViewBag.MailTitle %></strong></span>
    <%  } %>
</div>

<script runat="server">

    ModelSource<InvoiceItem> models;
    Organization _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (Organization)this.Model;

    }

</script>
