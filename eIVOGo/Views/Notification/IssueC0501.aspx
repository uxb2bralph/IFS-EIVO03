<%@ Page Title="" Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewPage" %>

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
<%@ Import Namespace="eIVOGo.Module.Common" %>
<%@ Import Namespace="Model.DataEntity" %>
<%@ Import Namespace="Model.Helper" %>
<%@ Import Namespace="Model.Locale" %>
<%@ Import Namespace="Utility" %>
<%@ Import Namespace="Uxnet.Web.WebUI" %>

<% 
    if(_viewModel.ForTest==true)
    {
        Html.RenderPartial("~/Views/Notification/Module/IssuedC0501.ascx", _model);
    }
    else if (_model.Organization.OrganizationStatus.InvoiceNoticeSetting.CheckNotice(Naming.InvoiceNoticeStatus.Cancelling))
    {
        var mailing = models.GetUserListByCompanyID(_model.InvoiceBuyer.BuyerID)
                .Select(u => u.EMail).ToList();
        mailing.Add(_model.InvoiceBuyer.EMail);
        var mailTo = String.Join(",",
            mailing.Where(m => m != null));

        if (!String.IsNullOrEmpty(mailTo))
        {
            String subject = _model.Organization.CompanyName + "作廢電子發票郵件通知";
            String body = Html.Partial("~/Views/Notification/Module/IssuedC0501.ascx", _model).ToString();
            body.SendMailMessage(mailTo, subject);
        }

    }

%>
<script runat="server">

    ModelSource<InvoiceItem> models;
    InvoiceItem _model;
    DocumentQueryViewModel _viewModel;

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        models = ((SampleController<InvoiceItem>)ViewContext.Controller).DataSource;
        _model = (InvoiceItem)this.Model;
        _viewModel = (DocumentQueryViewModel)ViewBag.ViewModel;
    }


</script>
