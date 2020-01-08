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
        if (_model.Organization.OrganizationStatus.InvoiceNoticeSetting.CheckNotice(Naming.InvoiceNoticeStatus.UseCBEStyle))
        {
            Html.RenderPartial("~/Views/Notification/Module/IssuedCBE.cshtml", _model);
        }
        else
        {
            Html.RenderPartial("~/Views/Notification/Module/IssuedC0401.cshtml", _model);
        }
    }
    else if (_model.Organization.OrganizationStatus.InvoiceNoticeSetting.CheckNotice(Naming.InvoiceNoticeStatus.Issuing))
    {
        var mailing = models.GetUserListByCompanyID(_model.InvoiceBuyer.BuyerID)
                .Select(u => u.EMail).ToList();
        mailing.Add(_model.InvoiceBuyer.EMail);
        var mailTo = String.Join(",",
            mailing.Where(m => m != null));

        Logger.Debug($"InvoiceNo:{_model.TrackCode}{_model.No},mail to:{mailTo}");

        if (!String.IsNullOrEmpty(mailTo))
        {

            String subject = $"{_model.Organization.CompanyName}電子發票開立郵件通知";
            if (_model.InvoiceBuyer.IsB2C()
                || _model.Organization.OrganizationStatus.UseB2BStandalone == true)
            {
                String body = _model.Organization.OrganizationStatus.InvoiceNoticeSetting.CheckNotice(Naming.InvoiceNoticeStatus.UseCBEStyle)
                    ? Html.Partial("~/Views/Notification/Module/IssuedCBE.cshtml", _model).ToString()
                    : Html.Partial("~/Views/Notification/Module/IssuedC0401.cshtml", _model).ToString();

                if (_viewModel.AppendAttachment == true)
                {
                    body.SendMailMessage(mailTo, subject, _model.CDS_Document.Attachment.Select(a => a.StoredPath).ToArray());
                }
                else
                {
                    body.SendMailMessage(mailTo, subject);
                }
            }
            else
            {

                //將Log下的B2B發票PDF，Copy至暫存資料夾
                String pdfFile = models.PrepareToDownload(_model, false);

                if (_viewModel.AppendAttachment == true)
                {
                    var attached = _model.CDS_Document.Attachment.Select(a => a.StoredPath).ToList();
                    attached.Insert(0, pdfFile);
                    String body = Html.Partial("~/Views/Notification/Module/IssuedC0401.cshtml", _model).ToString();
                    body.SendMailMessage(mailTo, subject, attached.ToArray());
                }
                else
                {
                    String body = Html.Partial("~/Views/Notification/Module/IssuedC0401.cshtml", _model).ToString();
                    body.SendMailMessage(mailTo, subject, pdfFile);
                }
            }

            if (models.GetTable<IssuingNotice>().Any(d => d.DocID == _model.InvoiceID))
            {
                models.ExecuteCommand(
                    @"UPDATE IssuingNotice
                    SET IssueDate = {1}
                    WHERE (DocID = {0})", _model.InvoiceID, DateTime.Now);
            }
            else
            {
                models.ExecuteCommand(
                    @"Insert IssuingNotice (DocID,IssueDate) 
                    values ({0},{1})", _model.InvoiceID, DateTime.Now);
            }

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
