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
        if (_viewModel.ForTest == true)
        {
            Html.RenderPartial("~/Views/Notification/Module/IssuedA0401.ascx", _model);
        }
        else if (_model.Organization.OrganizationStatus.InvoiceNoticeSetting.CheckNotice(Naming.InvoiceNoticeStatus.Issuing))
        {

            String pdfFile = models.PrepareToDownload(_model, false);

            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(pdfFile, System.Net.Mime.MediaTypeNames.Application.Octet);
            ///修改附件檔名為發票號碼
            ///
            attachment.Name = String.Format("{0}{1}.pdf", _model.TrackCode, _model.No);

            var mailing = models.GetUserListByCompanyID(_model.InvoiceBuyer.BuyerID)
                    .Select(u => u.EMail).ToList();
            mailing.Add(_model.InvoiceBuyer.EMail);
            var mailTo = String.Join(",",
                mailing.Where(m => m != null));

            if (!String.IsNullOrEmpty(mailTo))
            {
                var enterprise = _model.InvoiceSeller.Organization.EnterpriseGroupMember.FirstOrDefault();

                String Subject = $"{(enterprise?.EnterpriseGroup.EnterpriseName)} 發票開立通知(發票號碼:{_model.TrackCode}{_model.No})";
                String body = Html.Partial("~/Views/Notification/Module/IssuedA0401.ascx", _model).ToString();

                body.SendMailMessage(mailTo, Subject, new System.Net.Mail.Attachment[] { attachment });
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
