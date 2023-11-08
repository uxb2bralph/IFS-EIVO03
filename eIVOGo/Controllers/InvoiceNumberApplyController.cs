using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using eIVOGo.Helper;
using eIVOGo.Helper.Security.Authorization;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using eIVOGo.Module.Common;
using eIVOGo.Properties;
using Microsoft.Ajax.Utilities;
using Model.DataEntity;
using Model.Locale;
using Model.Models.ViewModel;
using ModelExtension.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Services.Description;
using System.Windows.Controls;
using System.Xml;
using Utility;
using static Model.Locale.Naming;

namespace eIVOGo.Controllers
{
    public class InvoiceNumberApplyController : SampleController<InvoiceItem>
    {
        InvoiceNumberApplyResponse response = new InvoiceNumberApplyResponse();
        InvoiceNumberApplyService service;

        // GET: InvoiceApply
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Message(InvoiceNumberApplyResponse response)
        {
            return View(response);
        }

        [AuthorizedSysAdmin()]
        public ActionResult QueryIndex(InvoiceNumberApplyQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View();
        }

        [AuthorizedSysAdmin()]
        public ActionResult Query(InvoiceNumberApplyQueryViewModel viewModel)
        {
            service = new InvoiceNumberApplyService(viewModel.BusinessId);

            IEnumerable<FileInfo> fileInfos 
                = service.GetApplyJsonFilesInfo();

            IEnumerable<InvoiceNumberApplyQueryViewModel> applyFilterResult
                = fileInfos
                .Select(x => new InvoiceNumberApplyQueryViewModel
                    {
                        BusinessId = x.Name.Substring(6, 8),
                        ApplyUpdateTime = x.LastWriteTime,
                    }).ToList();

            ViewBag.ViewModel = viewModel;
            ViewBag.Model = applyFilterResult;
            return View("../InvoiceNumberApply/QueryResult", ViewBag);
        }

        public ActionResult Apply(string templateID= "AppointApply")
        {
            InvoiceNumberApply apply=null;
            if (templateID.Equals("AppointApply"))
            {
                apply = new InvoiceNumberApply().getTemplateNumberApplyData();
            }
            else if (templateID.Equals("TestData"))
            {
                apply = new InvoiceNumberApply().getTestData();
            }
            else if (templateID.Equals("TestDataWithWrongFormat"))
            {
                apply = new InvoiceNumberApply().getTestDataWithWrongFormat();
            }

            if (apply==null)
            {
                response.Message = "傳入參數有誤";
                return RedirectToAction("Message",
                    "InvoiceNumberApply",
                    response);
            }

            InvoiceNumberApplyViewModel applyViewModel = new InvoiceNumberApplyViewModel(apply);
            return View(applyViewModel);
        }
        
        [HttpGet]
        public JsonResult SysSupplier(string sysSupplierID)
        {
            return Json(
                 AppSettings.Default.InvoiceNumberApplySetting.SysSupplier
                    .Where(x => x.ID == sysSupplierID)
                    .FirstOrDefault().JsonStringify()
                , JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult PaperTestSet(string paperTestSetID)
        {
            return Json(
                 AppSettings.Default.InvoiceNumberApplySetting.PaperTestSet
                    .Where(x => x.ID == paperTestSetID)
                    .FirstOrDefault().JsonStringify()
                , JsonRequestBehavior.AllowGet);
        }
        [AuthorizedSysAdmin()]
        public ActionResult Update(string businessID)
        {
            service = new InvoiceNumberApplyService(businessID);

            InvoiceNumberApply apply 
                = service.GetApplyViewModelFromJson();
            InvoiceNumberApplyViewModel applyViewModel 
                = new InvoiceNumberApplyViewModel(apply);
            return View("../InvoiceNumberApply/Apply", applyViewModel);
        }

        [AuthorizedSysAdmin()]
        public ActionResult MoveFile(string businessID) 
        {
            InvoiceNumberApplyService.moveJsonFile(businessID);
            return Json(new { result = true });
        }

        [AuthorizedSysAdmin()]
        public ActionResult TransferOrganization(string businessID)
        {
            #region check
            response.BusinessID = businessID;

            if (string.IsNullOrEmpty(businessID))
            {
                response.Message = "統編不存在";
                response.IsInvalid = true;
            }

            service = new InvoiceNumberApplyService(businessID);

            InvoiceNumberApply apply
                = service.GetApplyViewModelFromJson();

            if (apply == null)
            {
                response.Message = "Json檔不存在";
                response.IsInvalid = true;
            }
            #endregion
            if (response.IsInvalid)
            {
                ModelState.AddModelError("Message", response.DisplayMessage);
                Logger.Info(response.DisplayMessage);
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            try
            {
                OrganizationViewModel organizationViewModel
                    = service.ApplyConvertedOrganization(apply);

                Organization organization
                    = organizationViewModel.CommitOrganizationViewModel(models, ModelState);

                if (organization == null || !ModelState.IsValid)
                {
                    var msg = ModelStateErrLog();
                    ModelState.AddModelError("Message", msg);
                    return View("~/Views/Shared/ReportInputError.cshtml");
                }

                InvoiceNumberApplyService.moveJsonFile(businessID);
            }
            catch (Exception ex)
            {
                response.Message = "營業人轉換失敗.";
                response.IsInvalid = true;
                Logger.Warn(response.DisplayMessage+"   "+ex.ToString());
                ModelState.AddModelError("Message", response.DisplayMessage);
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            return Json(new { result = true });
        }

        public ActionResult SetAll(string businessID)
        {
            new InvoiceNumberApplyService(businessID);

            if (businessID == "" || businessID == null)
            {
                response.IsInvalid = true;
                response
                    = new InvoiceNumberApplyResponse(
                        message:"businessID is null or empty.");
            }
            response.BusinessID = businessID;

            InvoiceNumberApply viewBase =
                 new InvoiceNumberApplyService(businessID).GetApplyViewModelFromJson();
            if (!response.IsInvalid && viewBase == null)
            {
                response.IsInvalid = true;
                response.Message = "找不到相關JSON檔,或JSON格式有誤.";
            }

            if (response.IsInvalid) 
            {
                Logger.Info(response.DisplayMessage);
                return RedirectToAction("Message",
                    "InvoiceNumberApply",
                    response);
            }

            IEnumerable<InvoiceNumberApplyWordSetting> wordSets
                 = AppSettings.Default.InvoiceNumberApplyWordSetting;
            UTF8Encoding encoding = new UTF8Encoding();
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                    {
                        foreach (InvoiceNumberApplyWordSetting wordset in wordSets)
                        {
                            var file = archive.CreateEntry(wordset.OutputName);
                            InvoiceNumberApplyService sb
                                = new InvoiceNumberApplyService(viewBase, wordset);

                            byte[] contentAsBytes = encoding.GetBytes(sb.GetReplacedWordXmlString());
                            using (var stream = file.Open())
                            {
                                stream.Write(contentAsBytes, 0, contentAsBytes.Length);
                            }
                        }
                    }

                    return File(memoryStream.ToArray(), "application/zip", AppSettings.Default.InvoiceNumberApplySetting.GetZipFileName(businessID));
                };
            }
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
                response.IsInvalid = true;
                response.Message = "JSON轉Word失敗.";
                return RedirectToAction("Message",
                    "InvoiceNumberApply",
                    response);
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Create(InvoiceNumberApplyViewModel viewModel)
        {
            if (viewModel == null)
            {
                return View("../InvoiceNumberApply/Apply", viewModel);
            }

            if (viewModel.Apply == null || !this.ModelState.IsValid)
            {
                ModelStateErrLog();
                viewModel.ValidCode = string.Empty;
                viewModel.EncryptedCode = string.Empty;
                return View("../InvoiceNumberApply/Apply", viewModel);
            }

            response.BusinessID = viewModel.Apply.BusinessID;

            try
            {
                string jsonFilePath = AppSettings.Default.InvoiceNumberApplySetting.GetApplyFilePath(viewModel.Apply.BusinessID);
                viewModel.Apply.SerializeObjectToJsonFile(jsonFilePath);
            }
            catch (Exception ex)
            {
                response.IsInvalid = true;
                response.Message = "資料儲存失敗";
                Logger.Warn(ex.ToString());
                return RedirectToAction("Message",
                    "InvoiceNumberApply",
                    response);
            }

            try
            {
                if (AppSettings.Default.InvoiceNumberApplySetting.NotifyEnable)
                {
                    SharedFunction.SendMailMessage(viewModel.Apply.ToString()
                        , Properties.Settings.Default.WebMaster
                        , string.Format("電子發票號碼申請-{0}", viewModel.Apply.BusinessID));
                }
            }
            catch (Exception)
            {
                response.IsInvalid = true;
                response.Message = "申請email寄送失敗.";
                Logger.Error(response.DisplayMessage);
                return RedirectToAction("Message",
                    "InvoiceNumberApply",
                    response);
            }

            return View(response);
        }

        private string ModelStateErrLog()
        {
            IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
            string logModelStateMsg = string.Empty;
            var ModelStateErrors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .ToDictionary(
                    k => k.Key,
                    k => k.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
            if (ModelStateErrors.Count > 0)
            {
                foreach (KeyValuePair<string, string[]> kvp in ModelStateErrors)
                {
                    logModelStateMsg += (string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value[0].ToString()));
                }
                Logger.Info(logModelStateMsg);
            }

            return logModelStateMsg;
        }
    }

}