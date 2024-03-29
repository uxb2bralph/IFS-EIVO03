﻿using System;
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
using ModelExtension.Helper;
using System.Windows.Controls;
using static Model.Locale.Naming;

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

            return View("~/Views/Notification/DataUploadExceptionList.cshtml", items);
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

            return View("~/Views/Notification/CommissionedToReceiveA0401.cshtml", item);

        }

        public ActionResult NotifyToReceiveA0401(DocumentQueryViewModel viewModel)
        {
            ViewResult result = (ViewResult)IssueA0401(viewModel);
            InvoiceItem item = result.Model as InvoiceItem;

            if (item == null)
                return result;

            return View("~/Views/Notification/NotifyToReceiveA0401.cshtml", item);

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

            return View("~/Views/Notification/IssueC0501.cshtml", item);
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

            return View("~/Views/Notification/IssueAllowanceCancellation.cshtml", item);
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

        public ActionResult NotifyTwoFactorSettings(UserProfileViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
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

            item = item.LoadInstance(models).PrepareTwoFactorKey(models);

            return View("~/Views/Notification/NotifyTwoFactorSettings.cshtml", item);

        }

        public ActionResult NotifySystemAnnouncement(String[] mailTo)
        {
            return View("~/Views/Notification/NotifySystemAnnouncement.cshtml", mailTo);
        }

        public ActionResult NotifyLowerInvoiceNoStock(OrganizationViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.CompanyID = viewModel.DecryptKeyValue();
            }

            Organization item = models.GetTable<Organization>()
                    .Where(o => o.CompanyID == viewModel.CompanyID).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "資料錯誤!!");
            }

            return View("~/Views/Notification/NotifyLowerInvoiceNoStock.cshtml", item);
        }

        public ActionResult NotifyInvoiceNotUpload(OrganizationViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            if (viewModel.KeyID != null)
            {
                viewModel.CompanyID = viewModel.DecryptKeyValue();
            }

            Organization item = models.GetTable<Organization>()
                    .Where(o => o.CompanyID == viewModel.CompanyID)
                    .FirstOrDefault();

            if (viewModel == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "資料錯誤!!");
            }

            return View("~/Views/Notification/NotifyInvoiceNotUpload.cshtml", item);
            
        }
        public FileContentResult NotifyInvoiceNotUploadList(int? id)
        {
            //DateTime dt1 = DateTime.Now;
            if (id == 0) return null;
            int yyyy = int.Parse(id.ToString().Substring(0, 4));
            int mm = int.Parse(id.ToString().Substring(4, 2));
            int dd = int.Parse(id.ToString().Substring(6, 2));
            using (InvoiceManager mgr = new InvoiceManager())
            {
                DateTime checkDate = DateTime.Parse($"{yyyy}/{mm}/{dd}");

                var eligibleInvoiceCountZeroSellers
                    = InvoiceNotUploadNotification
                        .GetInvoiceUploadZeroWeatherSettingAlertOrNotList   (
                        mgr,
                        checkDate);

                var eligibleInvoiceCountZeroSellersOrgs = 
                    mgr.GetTable<Organization>()
                    .Where(x => eligibleInvoiceCountZeroSellers.Contains(x.CompanyID))
                    .Select(y=> new { 
                        SellerId = y.ReceiptNo,
                        SellerName = y.CompanyName,
                        CompanyId = y.CompanyID,
                        SellerStatus = y.OrganizationStatus.CurrentLevel
                    }).ToList();

                //DateTime dt2 = DateTime.Now;
                //Logger.Info($"TotalSeconds={(dt2 - dt1).TotalSeconds}");
                ClosedXML.Excel.XLWorkbook excel;
                using (DataSet ds = new DataSet())
                {
                    DataTable table = new DataTable($"未上傳發票{id}列表");
                    table.Columns.Add("統一編號");
                    table.Columns.Add("公司名稱");
                    table.Columns.Add("公司代碼");
                    foreach (var org in eligibleInvoiceCountZeroSellersOrgs)
                    {
                        var r = table.NewRow();
                        r[0] = org.SellerId;
                        r[1] = org.SellerName;
                        r[2] = org.CompanyId;
                        table.Rows.Add(r);
                    }

                    ds.Tables.Add(table);
                    excel = ds.ConvertToExcel();

                }

                using (var ms = new MemoryStream())
                {
                    excel.SaveAs(ms);
                    return File(ms.ToArray(), "application/octet-stream", $"未上傳發票{id}列表.xlsx");
                }
            }
        }
    }
}