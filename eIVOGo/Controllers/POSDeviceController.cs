using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.DataEntity;
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
                Authorization: Base64(SHA256([Vendor 統編] + [Activation Key] + [Seed]))

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

            viewModel.Seed = Request.Headers["Seed"].GetEfficientString();
            viewModel.Authorization = Request.Headers["Authorization"].GetEfficientString();

            List<InvoiceNoAllocation> items = models.AllocateInvoiceNo(viewModel);

            return Json(new
            {
                viewModel.SellerID,
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


    }
}