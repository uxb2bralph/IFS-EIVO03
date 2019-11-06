using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Module.Common;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.DocumentManagement;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Schema.EIVO.B2B;
using Model.Schema.TurnKey;
using Model.Schema.TXN;
using ModelExtension.DataExchange;
using ModelExtension.Helper;
using Utility;
using Uxnet.Com.Security.UseCrypto;

namespace eIVOGo.Controllers
{
    public class BusinessRelationshipController : SampleController<InvoiceItem>
    {
        public ActionResult MaintainRelationship(String message)
        {
            ViewBag.Message = message;
            return View();
        }

        public ActionResult B2BIndex(BusinessRelationshipQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View();
        }

        public ActionResult ImportCounterpartBusinessXml(BusinessRelationshipQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View();
        }

        public ActionResult ImportCounterpartBusinessCsv(String message)
        {
            var userProfile = HttpContext.GetUser();
            var uploadMgr = userProfile["UploadManager"] as BusinessCounterpartUploadManager;
            if (uploadMgr != null)
                uploadMgr.Dispose();
            userProfile["UploadManager"] = null;

            ViewBag.Message = message;
            return View();
        }

        public ActionResult ImportCounterpartBusiness(String message)
        {
            var userProfile = HttpContext.GetUser();
            var uploadMgr = userProfile["UploadManager"] as BranchBusinessCounterpartUploadManager;
            if (uploadMgr != null)
                uploadMgr.Dispose();
            userProfile["UploadManager"] = null;

            ViewBag.Message = message;
            return View();
        }

        public ActionResult InquireBusinessRelationship(BusinessRelationshipQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var profile = HttpContext.GetUser();
            if (!profile.IsSystemAdmin())
            {
                viewModel.CompanyID = profile.CurrentUserRole.OrganizationCategory.CompanyID;
            }

            Expression<Func<Organization, bool>> queryExpr = i => true;

            if (!String.IsNullOrEmpty(viewModel.ReceiptNo))
            {
                queryExpr = queryExpr.And(i => i.ReceiptNo == viewModel.ReceiptNo);
            }
            if (!String.IsNullOrEmpty(viewModel.CompanyName))
            {
                queryExpr = queryExpr.And(i => i.CompanyName.Contains(viewModel.CompanyName) || i.OrganizationBranch.Any(b => b.BranchName.Contains(viewModel.CompanyName)));
            }
            if (viewModel.CompanyStatus.HasValue)
            {
                queryExpr = queryExpr.And(i => i.OrganizationStatus.CurrentLevel == viewModel.CompanyStatus);
            }
            if (!String.IsNullOrEmpty(viewModel.BranchNo))
            {
                viewModel.BranchNo = viewModel.BranchNo.GetEfficientString();
                ViewBag.ByBranchNo = viewModel.BranchNo;
                queryExpr = queryExpr.And(i => i.OrganizationBranch.Any(b => b.BranchNo == viewModel.BranchNo));
            }


            //主動列印
            if (viewModel.EntrustToPrint.HasValue)
            {
                if (viewModel.EntrustToPrint == true)
                {
                    queryExpr = queryExpr.And(i => i.OrganizationStatus.EntrustToPrint == true);
                }
                else
                {
                    queryExpr = queryExpr.And(i => i.OrganizationStatus.EntrustToPrint == false || !i.OrganizationStatus.EntrustToPrint.HasValue);
                }
            }
            //自動接收
            if (viewModel.Entrusting.HasValue)
            {
                if (viewModel.Entrusting == true)
                {
                    queryExpr = queryExpr.And(i => i.OrganizationStatus.Entrusting == true);
                }
                else
                {
                    queryExpr = queryExpr.And(i => i.OrganizationStatus.Entrusting == false || !i.OrganizationStatus.Entrusting.HasValue);
                }
            }


            var org = models.GetTable<Organization>();
            IQueryable<BusinessRelationship> items;

            if (viewModel.BusinessType.HasValue)
            {
                items = viewModel.CompanyID.HasValue
                    ? models.GetTable<BusinessRelationship>().Where(b => b.MasterID == viewModel.CompanyID.Value && b.BusinessID == viewModel.BusinessType)
                        .Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b)
                    : models.GetTable<BusinessRelationship>().Where(b => b.BusinessID == viewModel.BusinessType)
                        .Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b);
            }
            else
            {
                items = viewModel.CompanyID.HasValue
                    ? models.GetTable<BusinessRelationship>().Where(b => b.MasterID == viewModel.CompanyID)
                        .Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b)
                    : models.GetTable<BusinessRelationship>()
                        .Join(org.Where(queryExpr), b => b.RelativeID, o => o.CompanyID, (b, o) => b);
            }

            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex--;
                return View("~/Views/BusinessRelationship/Module/ItemList.ascx", items);
            }
            else
            {
                viewModel.PageIndex = 0;
                return View("~/Views/BusinessRelationship/Module/QueryResult.ascx", items);
            }

        }

        public ActionResult DeleteItem(int businessID, int masterID, int relativeID)
        {
            var item = models.DeleteAny<BusinessRelationship>(m => m.MasterID == masterID && m.RelativeID == relativeID && m.BusinessID == businessID);
            if (item == null)
            {
                return Json(new { result = false, message = "營業人資料錯誤!!" });
            }

            return Json(new { result = true });
        }

        public ActionResult Deactivate(int businessID, int masterID, int relativeID)
        {
            var item = models.GetTable<BusinessRelationship>().Where(m => m.MasterID == masterID && m.RelativeID == relativeID && m.BusinessID == businessID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/SiteAction/Alert.ascx", model: "營業人資料錯誤!!");
            }

            item.CurrentLevel = (int)Naming.MemberStatusDefinition.Mark_To_Delete;
            models.SubmitChanges();

            return View("~/Views/BusinessRelationship/Module/DataItem.ascx", item);
        }

        public ActionResult Activate(int businessID, int masterID, int relativeID)
        {
            var item = models.GetTable<BusinessRelationship>().Where(m => m.MasterID == masterID && m.RelativeID == relativeID && m.BusinessID == businessID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/SiteAction/Alert.ascx", model: "營業人資料錯誤!!");
            }

            item.CurrentLevel = (int)Naming.MemberStatusDefinition.Checked;
            models.SubmitChanges();

            return View("~/Views/BusinessRelationship/Module/DataItem.ascx", item);
        }

        public ActionResult SetEntrusting(int businessID, int masterID, int relativeID, bool status)
        {
            var item = models.GetTable<BusinessRelationship>().Where(m => m.MasterID == masterID && m.RelativeID == relativeID && m.BusinessID == businessID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/SiteAction/Alert.ascx", model: "營業人資料錯誤!!");
            }

            item.Counterpart.OrganizationStatus.Entrusting = status;
            models.SubmitChanges();

            return View("~/Views/BusinessRelationship/Module/DataItem.ascx", item);
        }

        public ActionResult SetEntrustToPrint(int businessID, int masterID, int relativeID, bool status)
        {
            var item = models.GetTable<BusinessRelationship>().Where(m => m.MasterID == masterID && m.RelativeID == relativeID && m.BusinessID == businessID).FirstOrDefault();
            if (item == null)
            {
                return View("~/Views/SiteAction/Alert.ascx", model: "營業人資料錯誤!!");
            }

            item.Counterpart.OrganizationStatus.EntrustToPrint = status;
            models.SubmitChanges();

            return View("~/Views/BusinessRelationship/Module/DataItem.ascx", item);
        }

        public ActionResult CommitItem(BusinessRelationshipViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.CompanyName))
            {
                ModelState.AddModelError("CompanyName", "營業人名稱格式錯誤");
            }

            if (viewModel.ReceiptNo == null || !Regex.IsMatch(viewModel.ReceiptNo, "\\d{8}"))
            {
                ModelState.AddModelError("ReceiptNo", "統編格式錯誤");
            }

            //if (string.IsNullOrEmpty(viewModel.ContactEmail) || !viewModel.ContactEmail.Contains('@'))
            //{
            //    ModelState.AddModelError("ContactEmail", "聯絡人電子郵件格式錯誤");
            //}

            //if (string.IsNullOrEmpty(viewModel.Addr))
            //{
            //    ModelState.AddModelError("Addr", "地址格式錯誤");
            //}

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/SiteAction/Alert.ascx", model: null);
            }

            UserProfile userProfile;
            BusinessRelationship model = viewModel.ApplyCounterpartBusiness(DataSource, out userProfile);
            Organization item = model.BusinessMaster;
            if (userProfile != null)
            {
                userProfile.SendActivationNotice();
            }

            if (item.ReceiptNo == "0000000000")
            {
                return View("~/Views/Shared/JsAlert.ascx", model: "修改完成!!");
            }

            return View("~/Views/BusinessRelationship/Module/DataItem.ascx", model);

        }

        public ActionResult ImportCsv(BusinessRelationshipViewModel viewModel)
        {
            var userProfile = HttpContext.GetUser();

            if (!viewModel.CompanyID.HasValue)
            {
                ModelState.AddModelError("CompanyID", "請先建立集團成員!!");
            }
            else if (!viewModel.BusinessType.HasValue)
            {
                ModelState.AddModelError("BusinessType", "請選擇相對營業人類別!!");
            }
            else if (Request.Files.Count == 0)
            {
                ModelState.AddModelError("csvFile", "請匯入檔案!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                ViewBag.Message = ModelState.ErrorMessage();
                return View("~/Views/BusinessRelationship/ImportCounterpartBusiness.aspx");
            }

            var file = Request.Files[0];
            String fileName = Path.Combine(Logger.LogDailyPath, DateTime.Now.Ticks + "_" + Path.GetFileName(file.FileName));
            file.SaveAs(fileName);

            var mgr = new BranchBusinessCounterpartUploadManager();
            mgr.BusinessType = (Naming.InvoiceCenterBusinessType)viewModel.BusinessType;
            mgr.MasterID = viewModel.CompanyID;
            mgr.ParseData(userProfile, fileName, Encoding.GetEncoding(Request["encoding"]));

            userProfile["UploadManager"] = mgr;

            return View("~/Views/BusinessRelationship/ImportCounterpartBusiness.aspx");

        }

        public ActionResult ImportXml(BusinessRelationshipViewModel viewModel)
        {
            var userProfile = HttpContext.GetUser();

            if (!viewModel.CompanyID.HasValue)
            {
                ModelState.AddModelError("CompanyID", "請先建立集團成員!!");
            }
            else if (!viewModel.BusinessType.HasValue)
            {
                ModelState.AddModelError("BusinessType", "請選擇相對營業人類別!!");
            }
            else if (Request.Files.Count == 0)
            {
                ModelState.AddModelError("theFile", "請匯入檔案!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/Shared/ReportInputError.ascx");
            }

            var file = Request.Files[0];
            String fileName = Path.Combine(Logger.LogDailyPath, DateTime.Now.Ticks + "_" + Path.GetFileName(file.FileName));
            file.SaveAs(fileName);

            CounterpartBusinessExchange exchange = new CounterpartBusinessExchange();
            exchange.ExchangeData(viewModel, fileName);
            if(exchange.NewUsers.Count>0)
            {
                Task.Factory.StartNew(() =>
                {
                    foreach (var profile in exchange.NewUsers)
                    {
                        profile.SendActivationNotice();
                    }
                });
            }
            ViewBag.ImportFile = fileName;

            return View("~/Views/BusinessRelationship/Module/CounterpartBusinessXmlResult.ascx", exchange);

        }

        public ActionResult ImportBusinessCsv(BusinessRelationshipViewModel viewModel)
        {
            var userProfile = HttpContext.GetUser();

            if (!viewModel.CompanyID.HasValue)
            {
                ModelState.AddModelError("CompanyID", "請先建立集團成員!!");
            }
            else if (!viewModel.BusinessType.HasValue)
            {
                ModelState.AddModelError("BusinessType", "請選擇相對營業人類別!!");
            }
            else if (Request.Files.Count == 0)
            {
                ModelState.AddModelError("csvFile", "請匯入檔案!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                ViewBag.Message = ModelState.ErrorMessage();
                return View("~/Views/BusinessRelationship/ImportCounterpartBusinessCsv.aspx");
            }

            var file = Request.Files[0];
            String fileName = Path.Combine(Logger.LogDailyPath, DateTime.Now.Ticks + "_" + Path.GetFileName(file.FileName));
            file.SaveAs(fileName);

            var mgr = new BusinessCounterpartUploadManager();
            mgr.BusinessType = (Naming.InvoiceCenterBusinessType)viewModel.BusinessType;
            mgr.MasterID = viewModel.CompanyID;
            mgr.ParseData(userProfile, fileName, Encoding.GetEncoding(Request["encoding"]));

            userProfile["UploadManager"] = mgr;

            return View("~/Views/BusinessRelationship/ImportCounterpartBusinessCsv.aspx");

        }

        public ActionResult ImportCounterpartBusinessList(int? pageIndex, int? filterMode)
        {

            if (pageIndex.HasValue)
            {
                ViewBag.PageIndex = pageIndex - 1;
            }
            else
            {
                ViewBag.PageIndex = 0;
            }

            ViewBag.FilterMode = filterMode;

            var profile = HttpContext.GetUser();
            var mgr = (BranchBusinessCounterpartUploadManager)profile["UploadManager"];

            if (mgr == null)
            {
                return View("~/Views/SiteAction/Alert.ascx", model: "連線逾時，請重新匯入資料!!");
            }

            return View("~/Views/BusinessRelationship/Module/ImportCounterpartBusinessList.ascx");

        }

        public ActionResult ImportCounterpartBusinessCsvList(int? pageIndex, int? filterMode)
        {

            if (pageIndex.HasValue)
            {
                ViewBag.PageIndex = pageIndex - 1;
            }
            else
            {
                ViewBag.PageIndex = 0;
            }

            ViewBag.FilterMode = filterMode;

            var profile = HttpContext.GetUser();
            var mgr = (BusinessCounterpartUploadManager)profile["UploadManager"];

            if (mgr == null)
            {
                return View("~/Views/SiteAction/Alert.ascx", model: "連線逾時，請重新匯入資料!!");
            }

            return View("~/Views/BusinessRelationship/Module/ImportCounterpartBusinessCsvList.ascx");

        }

        public ActionResult ImportCounterpartBusinessXmlList(int? pageIndex, int? filterMode)
        {

            if (pageIndex.HasValue)
            {
                ViewBag.PageIndex = pageIndex - 1;
            }
            else
            {
                ViewBag.PageIndex = 0;
            }

            ViewBag.FilterMode = filterMode;

            var profile = HttpContext.GetUser();
            var mgr = (BusinessCounterpartXmlUploadManager)profile["UploadManager"];

            if (mgr == null)
            {
                return View("~/Views/SiteAction/Alert.ascx", model: "連線逾時，請重新匯入資料!!");
            }

            return View("~/Views/BusinessRelationship/Module/ImportCounterpartBusinessXmlList.ascx");

        }

        public ActionResult CommitImport()
        {
            var userProfile = HttpContext.GetUser();
            var uploadMgr = (BranchBusinessCounterpartUploadManager)userProfile["UploadManager"];
            if (uploadMgr.IsValid)
            {
                try
                {
                    uploadMgr.Save();
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { result = true, message = "資料檔有錯,無法匯入!!" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CommitImportXml()
        {
            var userProfile = HttpContext.GetUser();
            var uploadMgr = (BusinessCounterpartXmlUploadManager)userProfile["UploadManager"];
            if (uploadMgr.IsValid)
            {
                try
                {
                    uploadMgr.Save();
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { result = true, message = "資料檔有錯,無法匯入!!" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CommitImportBusinessCsv()
        {
            var userProfile = HttpContext.GetUser();
            var uploadMgr = (BusinessCounterpartUploadManager)userProfile["UploadManager"];
            if (uploadMgr.IsValid)
            {
                try
                {
                    uploadMgr.Save();
                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                    return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { result = true, message = "資料檔有錯,無法匯入!!" }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CancelImport()
        {
            var userProfile = HttpContext.GetUser();
            IDisposable uploadMgr = userProfile["UploadManager"] as BranchBusinessCounterpartUploadManager;
            if (uploadMgr == null)
            {
                uploadMgr = userProfile["UploadManager"] as BusinessCounterpartUploadManager;
            }
            if (uploadMgr != null)
            {
                uploadMgr.Dispose();
            }
            userProfile["UploadManager"] = null;

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetCounterpartBusinessSample()
        {
            CounterpartBusinessExchange exchange = new CounterpartBusinessExchange();
            using (var xls = exchange.GetSample())
            {
                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.AddHeader("Cache-control", "max-age=1");
                Response.ContentType = "application/vnd.ms-excel";
                Response.AddHeader("Content-Disposition", $"attachment;filename={HttpUtility.UrlEncode("相對營業人")}.xlsx");

                xls.SaveAs(Response.OutputStream);
            }

            return new EmptyResult { };
        }

    }
}