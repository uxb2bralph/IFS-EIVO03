using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.DataEntity;
using Model.Security;
using eIVOGo.Helper;
using eIVOGo.Models;
using Model.Security.MembershipManagement;
using Business.Helper;
using System.Text;
using Model.Locale;
using System.Web.Script.Serialization;
using System.IO;
using System.IO.Compression;
using Utility;
using System.Threading;
using System.Data.SqlClient;
using System.Data;
using ClosedXML.Excel;
using System.Data.Linq;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using Model.InvoiceManagement;
using Newtonsoft.Json;
using System.Xml;
using Model.Schema.EIVO;
using System.Xml.Linq;
using System.Web.Http;
using Model.InvoiceManagement.Validator;
using System.Text.RegularExpressions;

namespace eIVOGo.Controllers
{
    public class POSDeviceController : SampleController<InvoiceItem>
    {
        // GET: POSDevice
        public ActionResult AllocateInvoiceNo(POSDeviceViewModel viewModel)
        {
            //if (String.IsNullOrEmpty(Request.ContentType) && String.IsNullOrEmpty(Request.Params["Query_String"]))
            //{
            //    using (StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
            //    {
            //        viewModel = JsonConvert.DeserializeObject<POSDeviceViewModel>(reader.ReadToEnd());
            //    }
            //}

            /**
                Http Header
                Seed: RANDOM[16]
                Authorization: Base64(ToHexString(SHA256([Vendor 統編] + [Activation Key] + [Seed])))

                {
	                "SellerID": "[商家統編]",
	                "Booklet": 1
                }
             */

            Request.SaveAs(Path.Combine(Logger.LogDailyPath, String.Format("{0}.txt", DateTime.Now.Ticks)), true);

            if (viewModel.Booklet.HasValue)
            {
                viewModel.quantity = viewModel.Booklet * 50;
            }

            if (Request.Headers["Seed"] != null)
            {
                viewModel.Seed = Request.Headers["Seed"].GetEfficientString();
            }
            if(Request.Headers["Authorization"] != null)
            {
                viewModel.Authorization = Request.Headers["Authorization"].GetEfficientString();
            }

            List<InvoiceNoAllocation> items = models.AllocateInvoiceNo(viewModel);
            var item = items.FirstOrDefault();

            return Json(new
            {
                viewModel.SellerID,
                item?.InvoiceNoInterval.InvoiceTrackCodeAssignment.InvoiceTrackCode.Year,
                item?.InvoiceNoInterval.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo,
                invoice_issue = items.Select(t => new
                {
                    sn = t.InvoiceNoInterval.InvoiceTrackCodeAssignment.InvoiceTrackCode.TrackCode + String.Format("{0:00000000}", t.InvoiceNo),
                    random = t.RandomNo,
                    aesbase64 = t.EncryptedContent
                }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CommitInvoice(InvoiceViewModel viewModel)
        {
            if (String.IsNullOrEmpty(Request.ContentType) && String.IsNullOrEmpty(Request.Params["Query_String"]))
            {
                using (StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
                {
                    viewModel = JsonConvert.DeserializeObject<InvoiceViewModel>(reader.ReadToEnd());
                }
            }

            int no;
            int.TryParse(viewModel.No, out no);
            var item = models.GetTable<InvoiceNoAllocation>().Where(i => i.InvoiceNo == no && i.InvoiceNoInterval.InvoiceTrackCodeAssignment.InvoiceTrackCode.TrackCode == viewModel.TrackCode).FirstOrDefault();

            if (item == null)
                return Json(new { result = false, message = "發票號碼錯誤!!" }, JsonRequestBehavior.AllowGet);

            var seller = item.InvoiceNoInterval.InvoiceTrackCodeAssignment.Organization;

            viewModel.SellerName = seller.CompanyName;
            viewModel.SellerReceiptNo = seller.ReceiptNo;

            InvoiceViewModelValidator<InvoiceItem> validator = new InvoiceViewModelValidator<InvoiceItem>(this.DataSource, seller);
            var exception = validator.Validate(viewModel);
            if (exception != null)
            {
                return Json(new { result = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
            }

            InvoiceItem newItem = validator.InvoiceItem;
            models.GetTable<InvoiceItem>().InsertOnSubmit(newItem);
            item.Status = (int)Naming.UploadStatusDefinition.匯入成功;
            models.SubmitChanges();

            viewModel.TrackCode = newItem.TrackCode;
            viewModel.No = newItem.No;

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetIncompleteInvoiceNo(POSDeviceViewModel viewModel)
        {
            if (String.IsNullOrEmpty(Request.ContentType) && String.IsNullOrEmpty(Request.Params["Query_String"]))
            {
                using (StreamReader reader = new StreamReader(Request.InputStream, Request.ContentEncoding))
                {
                    viewModel = JsonConvert.DeserializeObject<POSDeviceViewModel>(reader.ReadToEnd());
                }
            }

            List<InvoiceNoAllocation> items = new List<InvoiceNoAllocation>();

            //receiptNo = receiptNo.GetEfficientString();
            var seller = models.GetTable<Organization>().Where(c => c.ReceiptNo == viewModel.company_id).FirstOrDefault();
            if (seller != null)
            {
                items = models.GetTable<InvoiceNoAllocation>().Where(d => d.Status == (int)Naming.UploadStatusDefinition.等待匯入)
                    .Where(d => d.InvoiceNoInterval.InvoiceTrackCodeAssignment.SellerID == seller.CompanyID).ToList();
            }

            return Json(new
            {
                invoice_pending = items.Select(t => new
                {
                    sn = t.InvoiceNoInterval.InvoiceTrackCodeAssignment.InvoiceTrackCode.TrackCode + String.Format("{0:00000000}", t.InvoiceNo),
                    time = String.Format("{0:yyyy/MM/dd HH:mm:ss}", t.AllocateDate)
                }).ToArray()
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VerifyAllowance()
        {
            XmlDocument viewModel = new XmlDocument();
            if (Request.ContentLength > 0)
            {
                viewModel.Load(Request.InputStream);
            }
            ViewBag.ViewModel = viewModel;
            return View("~/Views/POSDevice/VerifyAllowance.cshtml");
        }

        public ActionResult UploadAttachment()
        {

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InspectInvoice(POSDeviceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.Seed == null)
            {
                viewModel.Seed = Request.Headers["Seed"].GetEfficientString();
            }
            if (viewModel.Authorization == null)
            {
                viewModel.Authorization = Request.Headers["Authorization"].GetEfficientString();
            }

            InvoiceItem item = null;
            var receiptNo = viewModel.company_id.GetEfficientString();
            var orgItems = models.GetTable<Organization>().Where(c => c.ReceiptNo == receiptNo);
            var seller = orgItems.FirstOrDefault();
            if (seller != null)
            {
                bool auth = models.CheckAuthToken(seller, viewModel) != null;
                if (auth)
                {
                    viewModel.InvoiceNo = viewModel.InvoiceNo.GetEfficientString();
                    var match = viewModel.InvoiceNo.ParseInvoiceNo();

                    if (match.Success)
                    {
                        var issuers = models.GetTable<InvoiceIssuerAgent>().Where(c => c.AgentID == seller.CompanyID);
                        item = models.GetTable<InvoiceItem>()
                                .Where(i => i.TrackCode == match.Groups[1].Value && i.No == match.Groups[2].Value)
                                .Where(i => i.SellerID == seller.CompanyID || issuers.Any(a => a.IssuerID == i.SellerID))
                                .OrderByDescending(i => i.InvoiceID)
                                .FirstOrDefault();

                    }
                }
            }

            return View("~/Views/POSDevice/InspectInvoice.cshtml", item);

        }

        public ActionResult InspectAllowance(POSDeviceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.Seed == null)
            {
                viewModel.Seed = Request.Headers["Seed"].GetEfficientString();
            }
            if (viewModel.Authorization == null)
            {
                viewModel.Authorization = Request.Headers["Authorization"].GetEfficientString();
            }

            InvoiceAllowance item = null;
            var receiptNo = viewModel.company_id.GetEfficientString();
            var orgItems = models.GetTable<Organization>().Where(c => c.ReceiptNo == receiptNo);
            var seller = orgItems.FirstOrDefault();
            if (seller != null)
            {
                bool auth = models.CheckAuthToken(seller, viewModel) != null;
                if (auth)
                {
                    viewModel.AllowanceNo = viewModel.AllowanceNo.GetEfficientString();

                    var issuers = models.GetTable<InvoiceIssuerAgent>().Where(c => c.AgentID == seller.CompanyID);
                    item = models.GetTable<InvoiceAllowance>()
                    .Where(i => i.AllowanceNumber == viewModel.AllowanceNo)
                            .Where(i => i.InvoiceAllowanceSeller.SellerID == seller.CompanyID || issuers.Any(a => a.IssuerID == i.InvoiceAllowanceSeller.SellerID))
                            .OrderByDescending(a => a.AllowanceID)
                            .FirstOrDefault();
                }
            }

            return View("~/Views/DataView/D0401.cshtml", item);

        }

    }
}