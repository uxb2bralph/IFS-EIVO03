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
    if (_model.InvoiceAllowanceSeller.Organization.OrganizationStatus.InvoiceNoticeSetting.CheckNotice(Naming.InvoiceNoticeStatus.CancellingAllowance))
    {
        var mailing = models.GetUserListByCompanyID(_model.InvoiceAllowanceBuyer.BuyerID)
                .Select(u => u.EMail).ToList();

        if (!String.IsNullOrEmpty(_model.InvoiceAllowanceBuyer.EMail))
        {
            mailing.Add(_model.InvoiceAllowanceBuyer.EMail);
        }
        else
        {
            foreach (var a in _model.InvoiceAllowanceDetails)
            {
                String trackCode = a.InvoiceAllowanceItem.InvoiceNo.Substring(0, 2);
                String no = a.InvoiceAllowanceItem.InvoiceNo.Substring(2);
                var invoice = models.GetTable<InvoiceItem>().Where(i => i.TrackCode == trackCode && i.No == no).FirstOrDefault();
                if (invoice != null)
                {
                    mailing.Add(invoice.InvoiceBuyer.EMail);
                }
            }
        }
        var mailTo = String.Join(",",
            mailing.Where(m => m != null));

        if (!String.IsNullOrEmpty(mailTo))
        {
            var enterprise = _model.InvoiceAllowanceSeller.Organization.EnterpriseGroupMember.FirstOrDefault();

            String Subject = $"{(enterprise?.EnterpriseGroup.EnterpriseName)} 折讓證明作廢通知(折讓證明單號碼:{_model.AllowanceNumber})";
            String body = Html.Partial("~/Views/Notification/Module/IssuedAllowanceCancellation.ascx", _model).ToString();

            body.SendMailMessage(mailTo, Subject);
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
