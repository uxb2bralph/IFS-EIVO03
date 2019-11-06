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
    if (_model.InvoiceAllowanceSeller.Organization.OrganizationStatus.DisableIssuingNotice == true)
    {

    }
    else
    {
        String pdfFile = models.PrepareToDownload(_model);

        System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(pdfFile, System.Net.Mime.MediaTypeNames.Application.Octet);
        ///修改附件檔名為發票號碼
        ///
        attachment.Name = String.Format("{0}.pdf", _model.AllowanceNumber);

        var mailing = models.GetUserListByCompanyID(_model.InvoiceAllowanceBuyer.BuyerID)
                .Select(u => u.EMail).ToList();

        mailing.Add(_model.InvoiceAllowanceBuyer.EMail);
        var mailTo = String.Join(",",
            mailing.Where(m => m != null));

        if (!String.IsNullOrEmpty(mailTo))
        {
            var enterprise = _model.InvoiceAllowanceSeller.Organization.EnterpriseGroupMember.FirstOrDefault();

            String Subject = $"{(enterprise?.EnterpriseGroup.EnterpriseName)} 折讓證明開立通知(折讓證明單號碼:{_model.AllowanceNumber})";
            String body = Html.Partial("~/Views/Notification/Module/IssuedAllowance.ascx", _model).ToString();

            body.SendMailMessage(mailTo, Subject, new System.Net.Mail.Attachment[] { attachment });
        }
    }
    %>

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
