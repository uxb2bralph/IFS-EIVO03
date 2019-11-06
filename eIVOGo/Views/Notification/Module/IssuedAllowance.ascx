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

<%  if (_model.InvoiceAllowanceSeller.Organization.OrganizationStatus.InvoiceNoticeSetting.CheckNotice(Naming.InvoiceNoticeStatus.UseCustomStyle))
    {
        ViewBag.MailTitle = "發票折讓證明";
        Html.RenderPartial("~/Views/Notification/Module/MailTitle.ascx", _model.InvoiceAllowanceSeller.Organization);
    }
    else
    {   %>
<p>親愛的客戶您好：</p>
<%= _model.InvoiceAllowanceSeller.CustomerName %>
已開立您下述電子發票折讓證明，請知悉。<br />
<%  }   %>
    <% 
        Html.RenderPartial("~/Views/Notification/Module/AllowanceMailView.ascx", _model);
    %>
信件寄送時間：<%= DateTime.Now %>
<script runat="server">

    ModelSource<InvoiceItem> models;
    InvoiceAllowance _model;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceAllowance)this.Model;

    }

</script>

