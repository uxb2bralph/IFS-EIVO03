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
using Utility;

namespace eIVOGo.Controllers
{
    public class NotificationController : SampleController<InvoiceItem>
    {
        // GET: Notification
        public ActionResult IssueAllowance(DocumentQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.id = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<InvoiceAllowance>().Where(a => a.AllowanceID == viewModel.id).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "資料錯誤!!");
            }


            return View(item);
        }

        public ActionResult DataUploadExceptionList(ExceptionLogQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var items = models.GetTable<ExceptionLog>().Where(g => g.ExceptionReplication != null);

            if (viewModel.CompanyID.HasValue)
                items = items.Where(g => g.CompanyID == viewModel.CompanyID);
            if (viewModel.MaxLogID.HasValue)
                items = items.Where(g => g.LogID <= viewModel.MaxLogID);

            return View(items);
        }

        public ActionResult IssueA0401(DocumentQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.id = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<InvoiceItem>().Where(a => a.InvoiceID == viewModel.id).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "資料錯誤!!");
            }

            return View("~/Views/Notification/IssueA0401.cshtml", item);
        }

        public ActionResult CommissionedToReceiveA0401(DocumentQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)IssueA0401(viewModel);
            InvoiceItem item = result.Model as InvoiceItem;

            if (item == null)
                return result;

            return View(item);

        }

        public ActionResult NotifyToReceiveA0401(DocumentQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)IssueA0401(viewModel);
            InvoiceItem item = result.Model as InvoiceItem;

            if (item == null)
                return result;

            return View(item);

        }

        public ActionResult ActivateUser(UserProfileViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if(viewModel.KeyID!=null)
            {
                viewModel.UID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<UserProfile>().Where(u => u.UID == viewModel.UID).FirstOrDefault();
            if (item == null)
            {
                item = models.GetTable<UserProfile>().Where(u => u.PID == viewModel.PID).FirstOrDefault();
            }

            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "資料錯誤!!");
            }

            if (item.ResetUserPassword == null)
            {
                item.ResetUserPassword = new ResetUserPassword { };
            }
            item.ResetUserPassword.ResetID = Guid.NewGuid();

            if (viewModel.ResetPass == true)
            {
                item.UserProfileStatus.CurrentLevel = (int)Naming.MemberStatusDefinition.Wait_For_Check;
            }

            models.SubmitChanges();

            viewModel.ResetID = item.ResetUserPassword.ResetID;

            return View("~/Views/Notification/ActivateUser.cshtml", item);

        }

        public ActionResult IssueC0401(DocumentQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)IssueA0401(viewModel);
            InvoiceItem item = result.Model as InvoiceItem;

            if (item == null)
                return result;

            return View("~/Views/Notification/IssueC0401.cshtml", item);
        }

        public ActionResult IssueWinningInvoice(DocumentQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)IssueA0401(viewModel);
            InvoiceItem item = result.Model as InvoiceItem;

            if (item == null)
                return result;

            return View("~/Views/Notification/IssueWinningInvoice.cshtml", item);
        }

        public ActionResult IssueC0501(DocumentQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.id = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<DerivedDocument>().Where(d => d.DocID == viewModel.id || d.SourceID == viewModel.id)
                .Select(d => d.ParentDocument.InvoiceItem).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "資料錯誤!!");
            }

            return View(item);
        }

        public ActionResult IssueAllowanceCancellation(DocumentQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.id = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<DerivedDocument>().Where(d => d.DocID == viewModel.id || d.SourceID == viewModel.id)
                .Select(d => d.ParentDocument.InvoiceAllowance).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "資料錯誤!!");
            }

            return View(item);
        }

        public ActionResult IssueCustomMessage(DocumentQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("IssueCustomMessage");
        }

        public ActionResult NotifyProcessException(ProcessRequestQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var profile = HttpContext.GetUser();

            if (viewModel.KeyID != null)
            {
                viewModel.TaskID = viewModel.DecryptKeyValue();
            }

            ProcessRequest item = models.GetTable<ProcessRequest>().Where(r => r.TaskID == viewModel.TaskID).FirstOrDefault();
            if (item != null)
            {
                return View("~/Views/Notification/NotifyProcessException.cshtml", item);
            }

            return Content("Data not found!!");
        }

        public ActionResult SendDailyReport(InquireInvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            Organization item = models.GetTable<Organization>().Where(c => c.CompanyID == viewModel.SellerID)
                .FirstOrDefault();

            if (item == null)
            {
                viewModel.ReceiptNo = viewModel.ReceiptNo.GetEfficientString();
                item = models.GetTable<Organization>().Where(c => c.ReceiptNo == viewModel.ReceiptNo)
                        .FirstOrDefault();
            }

            if (item == null)
            {
                ModelState.AddModelError("SellerID", "未指定營業人!!");
            }

            viewModel.SellerID = item.CompanyID;

            if (!ModelState.IsValid)
            {
                return Json(new { result = false, message = ModelState.ErrorMessage() }, JsonRequestBehavior.AllowGet);
            }

            if(!viewModel.DateFrom.HasValue)
            {
                viewModel.DateFrom = DateTime.Today.AddDays(-1);
            }

            if (!viewModel.DateTo.HasValue)
            {
                viewModel.DateTo = viewModel.DateFrom;
            }

            return View("~/Views/Notification/SendDailyReport.cshtml", item);

        }


    }
}