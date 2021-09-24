using ApplicationResource;
using Business.Helper;
using Model.DataEntity;
using Model.Locale;
using Model.Models.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Utility;
using Model.Helper;
using TaskCenter.Helper.RequestAction;
using TaskCenter.Properties;
using Model.InvoiceManagement;
using Model.Security;
using System.Security.Cryptography;
using System.Xml;
using Model.Schema.EIVO;
using Model.Schema.TXN;
using Business.Helper.InvoiceProcessor;

namespace TaskCenter.Controllers
{
    public class InvoiceServiceController : SampleController
    {
        public ActionResult UploadInvoiceAutoTrackNo(InvoiceRequestViewModel viewModel)
        {
            InvoiceRoot invoice = FromJsonBody<InvoiceRequestViewModel>().InvoiceRoot;
            Root result = createMessageToken();
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { InvoiceClientID = viewModel.ClientID, ProcessType = viewModel.ProcessType })
            {
                OrganizationToken token = viewModel.CheckRequestToken(this);
                manager.UploadInvoiceAutoTrackNo(invoice, result, token);
            }
            return Content(result.JsonStringify(), "application/json");
        }

        public ActionResult UploadInvoice(InvoiceRequestViewModel viewModel)
        {
            Root result = createMessageToken();
            InvoiceRoot invoice = FromJsonBody<InvoiceRequestViewModel>().InvoiceRoot;
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { InvoiceClientID = viewModel.ClientID, ProcessType = viewModel.ProcessType })
            {
                OrganizationToken token = viewModel.CheckRequestToken(this);
                manager.UploadInvoice(invoice, result, token);
            }

            return Content(result.JsonStringify(), "application/json");
        }

        public ActionResult UploadInvoiceCancellation(InvoiceRequestViewModel viewModel)
        {
            Root result = createMessageToken();
            CancelInvoiceRoot item = FromJsonBody<InvoiceRequestViewModel>().CancelInvoiceRoot;
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { })
            {
                OrganizationToken token = viewModel.CheckRequestToken(this);
                manager.UploadInvoiceCancellation(result, item, token);
            }
            return Content(result.JsonStringify(), "application/json");
        }

        public ActionResult UploadAllowance(InvoiceRequestViewModel viewModel)
        {
            Root result = createMessageToken();
            AllowanceRoot allowance = FromJsonBody<InvoiceRequestViewModel>().AllowanceRoot;
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { })
            {
                OrganizationToken token = viewModel.CheckRequestToken(this);
                manager.UploadAllowance(result, allowance, token);
            }
            return Content(result.JsonStringify(), "application/json");
        }

        public ActionResult UploadAllowanceCancellation(InvoiceRequestViewModel viewModel)
        {
            Root result = createMessageToken();
            CancelAllowanceRoot item = FromJsonBody<InvoiceRequestViewModel>().CancelAllowanceRoot;
            using (InvoiceManagerV3 manager = new InvoiceManagerV3 { })
            {
                OrganizationToken token = viewModel.CheckRequestToken(this);
                manager.UploadAllowanceCancellation(result, item, token);
            }
            return Content(result.JsonStringify(), "application/json");
        }

        protected Root createMessageToken()
        {
            Root result = new Root
            {
                UXB2B = "電子發票系統",
                Result = new RootResult
                {
                    timeStamp = DateTime.Now,
                    value = 0
                }
            };
            return result;
        }
    }
}