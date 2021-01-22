using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using ModelExtension.Helper;
using Utility;
using Uxnet.Com.DataAccessLayer;
using eIVOGo.Published;
using Newtonsoft.Json;

namespace eIVOGo.Controllers
{
    public class InvoiceXmlServiceController : SampleController<InvoiceItem>
    {
        XmlDocument uploadData;
        // GET: InvoiceService
        public ActionResult AcknowledgeLivingReport()
        {
            (new eInvoiceService()).AcknowledgeLivingReport(uploadData);
            return new EmptyResult { };
        }

        public ActionResult AcknowledgeReceiving()
        {
            (new eInvoiceService()).AcknowledgeReceiving(uploadData);
            return new EmptyResult { };
        }

        public ActionResult AlertFailedTransaction()
        {
            (new eInvoiceService()).AlertFailedTransaction(uploadData);
            return new EmptyResult { };
        }
        public ActionResult B2BReceiveA0501() { var result = (new eInvoiceService()).B2BReceiveA0501(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2BReceiveB0501() { var result = (new eInvoiceService()).B2BReceiveB0501(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2BUploadAllowance() { var result = (new eInvoiceService()).B2BUploadAllowance(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2BUploadAllowanceCancellation() { var result = (new eInvoiceService()).B2BUploadAllowanceCancellation(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2BUploadBuyerInvoice() { var result = (new eInvoiceService()).B2BUploadBuyerInvoice(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2BUploadInvoice() { var result = (new eInvoiceService()).B2BUploadInvoice(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2BUploadInvoiceCancellation() { var result = (new eInvoiceService()).B2BUploadInvoiceCancellation(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2BUploadReceipt() { var result = (new eInvoiceService()).B2BUploadReceipt(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2BUploadReceiptCancellation() { var result = (new eInvoiceService()).B2BUploadReceiptCancellation(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2CUploadInvoice() { var result = (new eInvoiceService()).B2CUploadInvoice(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult B2CUploadInvoiceCancellation() { var result = (new eInvoiceService()).B2CUploadInvoiceCancellation(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult DeleteTempForReceivePDF(String pdfFile)
        {
            var result = (new eInvoiceService()).DeleteTempForReceivePDF(uploadData,pdfFile);
            result.ConvertToXml().Save(Response.OutputStream);
            return new EmptyResult { };
        }
        public ActionResult GetCurrentYearInvoiceTrackCode() { var result = (new eInvoiceService()).GetCurrentYearInvoiceTrackCode(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult GetCustomerIdListByAgent()
        {
            var result = (new eInvoiceService()).GetCustomerIdListByAgent(uploadData);
            result.ConvertToXml().Save(Response.OutputStream);
            return new EmptyResult { };
        }
        public ActionResult GetIncomingAllowanceCancellations() { var result = (new eInvoiceService()).GetIncomingAllowanceCancellations(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult GetIncomingAllowances() { var result = (new eInvoiceService()).GetIncomingAllowances(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult GetIncomingInvoiceCancellations() { var result = (new eInvoiceService()).GetIncomingInvoiceCancellations(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult GetIncomingInvoices() { var result = (new eInvoiceService()).GetIncomingInvoices(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult GetIncomingWinningInvoices() { var result = (new eInvoiceService()).GetIncomingWinningInvoices(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult GetInvoiceMailTracking(String clientID)
        {
            var result = (new eInvoiceService()).GetInvoiceMailTracking(uploadData,clientID);
            result.ConvertToXml().Save(Response.OutputStream);
            return new EmptyResult { };
        }
        public ActionResult GetInvoiceReturnedMail(String clientID)
        {
            var result = (new eInvoiceService()).GetInvoiceReturnedMail(uploadData,clientID);
            result.ConvertToXml().Save(Response.OutputStream);
            return new EmptyResult { };
        }
        public ActionResult GetInvoicesMap() { var result = (new eInvoiceService()).GetInvoicesMap(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult GetRegisteredMember() { var result = (new eInvoiceService()).GetRegisteredMember(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult GetSignerCertificateContent(String activationKey)
        {
            var result = (new eInvoiceService()).GetSignerCertificateContent(activationKey);
            result.ConvertToXml().Save(Response.OutputStream);
            return new EmptyResult { };
        }
        public ActionResult GetUpdatedWelfareAgenciesInfo(String sellerReceiptNo)
        {
            var result = (new eInvoiceService()).GetUpdatedWelfareAgenciesInfo(sellerReceiptNo);
            result.Save(Response.OutputStream); return new EmptyResult { };
        }
        public ActionResult GetVacantInvoiceNo(String receiptNo)
        {
            var result = (new eInvoiceService()).GetVacantInvoiceNo(uploadData,receiptNo);
            result.Save(Response.OutputStream);
            return new EmptyResult { };
        }
        public ActionResult GetWelfareAgenciesInfo(String sellerReceiptNo) { var result = (new eInvoiceService()).GetWelfareAgenciesInfo(sellerReceiptNo); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult NotifyCounterpartBusiness()
        {
            (new eInvoiceService()).NotifyCounterpartBusiness(uploadData);
            return new EmptyResult { };
        }

        public ActionResult ReceiveContentAsPDF(String clientID)
        {
            var result = (new eInvoiceService()).ReceiveContentAsPDF(uploadData,clientID);
            result.ConvertToXml().Save(Response.OutputStream);
            return new EmptyResult { };
        }
        public ActionResult UploadA0201() { var result = (new eInvoiceService()).UploadA0201(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadA0401() { var result = (new eInvoiceService()).UploadA0401(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadA0501() { var result = (new eInvoiceService()).UploadA0501(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadAllowance() { var result = (new eInvoiceService()).UploadAllowance(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadAllowanceCancellation() { var result = (new eInvoiceService()).UploadAllowanceCancellation(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadAllowanceCancellationV2() { var result = (new eInvoiceService()).UploadAllowanceCancellationV2(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadAllowanceV2() { var result = (new eInvoiceService()).UploadAllowanceV2(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadB0201() { var result = (new eInvoiceService()).UploadB0201(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadB0401() { var result = (new eInvoiceService()).UploadB0401(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadB0501() { var result = (new eInvoiceService()).UploadB0501(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadBranchTrack() { var result = (new eInvoiceService()).UploadBranchTrack(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadBranchTrackBlank() { var result = (new eInvoiceService()).UploadBranchTrackBlank(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadCounterpartBusiness() { var result = (new eInvoiceService()).UploadCounterpartBusiness(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadInvoice() { var result = (new eInvoiceService()).UploadInvoice(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadInvoiceAutoTrackNo() { var result = (new eInvoiceService()).UploadInvoiceAutoTrackNo(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadInvoiceAutoTrackNoByClient(String clientID,int channelID )
        {
            var result = (new eInvoiceService()).UploadInvoiceAutoTrackNoByClient(uploadData,clientID,channelID);
            result.Save(Response.OutputStream);
            return new EmptyResult { };
        }
        public ActionResult UploadInvoiceAutoTrackNoV2() { var result = (new eInvoiceService()).UploadInvoiceAutoTrackNoV2(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadInvoiceCancellation() { var result = (new eInvoiceService()).UploadInvoiceCancellationV2(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadInvoiceCancellationV2() { var result = (new eInvoiceService()).UploadInvoiceCancellationV2(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadInvoiceCancellationV2_C0501() { var result = (new eInvoiceService()).UploadInvoiceCancellationV2_C0501(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadInvoiceEnterprise() { var result = (new eInvoiceService()).UploadInvoiceEnterprise(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadInvoiceV2() { var result = (new eInvoiceService()).UploadInvoiceV2(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }
        public ActionResult UploadInvoiceV2_C0401() { var result = (new eInvoiceService()).UploadInvoiceV2_C0401(uploadData); result.Save(Response.OutputStream); return new EmptyResult { }; }



        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            uploadData = new XmlDocument();
            uploadData.Load(Request.InputStream);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
            Response.ContentType = "text/xml";
        }
    }
}