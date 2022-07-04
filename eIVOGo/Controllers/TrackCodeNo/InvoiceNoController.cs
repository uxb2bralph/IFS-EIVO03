using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Locale;
using Model.Schema.TurnKey.E0402;
using Model.Security.MembershipManagement;
using Utility;
using Model.InvoiceManagement;
using Newtonsoft.Json;
using ModelExtension.Helper;
using CsvHelper;
using Uxnet.Com.Helper;
using Model.Helper;
using eIVOGo.Helper.Security.Authorization;

namespace eIVOGo.Controllers.TrackCodeNo
{
    [Authorize]
    public class InvoiceNoController : SampleController<InvoiceItem>
    {
        // GET: InvoiceNo
        [RoleAuthorize(RoleID = new Naming.RoleID[] { Naming.RoleID.ROLE_SYS, Naming.RoleID.ROLE_SELLER })]
        public ActionResult MaintainInvoiceNoInterval()
        {
            return View("~/Views/InvoiceNo/MaintainInvoiceNoInterval.cshtml");
        }

        public ActionResult InquireInterval(InquireNoIntervalViewModel viewModel)
        {
            var profile = HttpContext.GetUser();

            ViewBag.ViewModel = viewModel;

            IQueryable<InvoiceNoInterval> items = models.GetTable<InvoiceNoInterval>();
            if (profile.IsSystemAdmin())
            {
                if(viewModel.SellerID.HasValue)
                {
                    items = items.Where(t => t.InvoiceTrackCodeAssignment.SellerID == viewModel.SellerID);
                }
            }
            else
            {
                items = items.Join(profile.InitializeOrganizationQuery(models).Where(o => o.CompanyID == viewModel.SellerID),
                    n => n.SellerID, o => o.CompanyID, (n, o) => n);
            }

            if (viewModel.Year.HasValue)
            {
                items = items.Where(i => i.InvoiceTrackCodeAssignment.InvoiceTrackCode.Year == viewModel.Year);
            }

            if (viewModel.PeriodNo.HasValue)
                items = items.Where(i => i.InvoiceTrackCodeAssignment.InvoiceTrackCode.PeriodNo == viewModel.PeriodNo);

            return View("~/Views/InvoiceNo/Module/QueryResult.cshtml", items);
        }

        public ActionResult InquireVacantNo(InquireNoIntervalViewModel viewModel)
        {
            var profile = HttpContext.GetUser();
            
            ViewBag.ViewModel = viewModel;

            //if (!viewModel.SellerID.HasValue)
            //{
            //    ModelState.AddModelError("SellerID", "請選擇開立人!!");
            //}

            if (!viewModel.Year.HasValue)
            {
                ModelState.AddModelError("Year", "請選擇年份!!");
            }

            if (!viewModel.PeriodNo.HasValue)
            {
                ModelState.AddModelError("PeriodNo", "請選擇期別!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            IQueryable<Organization> orgItems;
            if (viewModel.SellerID.HasValue)
            {
                orgItems = models.GetTable<Organization>().Where(o => o.CompanyID == viewModel.SellerID);
            }
            else
            {
                orgItems = profile.InitializeOrganizationQuery(models);
            }

            var items = models.GetTable<InvoiceTrackCode>()
                .Where(t => t.Year == viewModel.Year)
                .Where(t => t.PeriodNo == viewModel.PeriodNo)
                .Join(models.GetTable<UnassignedInvoiceNo>()
                        .Join(orgItems, n => n.SellerID, o => o.CompanyID, (n, o) => n),
                    t => t.TrackID, n => n.TrackID, (t, n) => n);

            return View("~/Views/InvoiceNo/VacantNo/QueryResult.cshtml", items);
        }

        public ActionResult DownloadVacantNo(InquireNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)InquireVacantNo(viewModel);
            if (!ModelState.IsValid)
            {
                return result;
            }

            IQueryable<UnassignedInvoiceNo> items = (IQueryable<UnassignedInvoiceNo>)result.Model;

            if (items.Count() == 0)
            {
                ViewBag.Message = "資料不存在!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

            return zipVacantNo(viewModel, items);

        }

        private ActionResult zipVacantNo(InquireNoIntervalViewModel viewModel, IQueryable<UnassignedInvoiceNo> vacantNoItems)
        {
            String temp = Server.MapPath("~/temp");
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
            String outFile = Path.Combine(temp, Guid.NewGuid().ToString() + ".zip");
            using (var zipOut = System.IO.File.Create(outFile))
            {
                using (ZipArchive zip = new ZipArchive(zipOut, ZipArchiveMode.Create))
                {
                    int count = 1;
                    var items = vacantNoItems.GroupBy(r => new { r.SellerID, r.TrackID });

                    foreach (var item in items)
                    {
                        var orgItem = models.GetTable<Organization>().Where(o => o.CompanyID == item.Key.SellerID).First();
                        var trackCode = models.GetTable<InvoiceTrackCode>().Where(t => t.TrackID == item.Key.TrackID).First();

                        BranchTrackBlank blankItem = new BranchTrackBlank
                        {
                            Main = new Main
                            {
                                BranchBan = orgItem.OrganizationExtension.ExpirationDate.HasValue
                                    ? $"{orgItem.ReceiptNo}(註記停用:{orgItem.OrganizationExtension.ExpirationDate:yyyy/MM/dd})"
                                    : orgItem.ReceiptNo,
                                HeadBan = viewModel.BranchRelation == true && orgItem.AsInvoiceInsurer.Count > 0 ? orgItem.AsInvoiceInsurer.First().InvoiceAgent.ReceiptNo : orgItem.ReceiptNo,
                                YearMonth = String.Format("{0}{1:00}", trackCode.Year - 1911, trackCode.PeriodNo * 2),
                                InvoiceType = trackCode.InvoiceType == (byte)InvoiceTypeEnum.Item08 ? InvoiceTypeEnum.Item08 : InvoiceTypeEnum.Item07,
                                InvoiceTrack = trackCode.TrackCode
                            },
                        };

                        List<DetailsBranchTrackBlankItem> details = new List<DetailsBranchTrackBlankItem>();
                        var detailItems = item;
                        foreach(var blank in detailItems)
                        {
                            details.Add(new DetailsBranchTrackBlankItem
                            {
                                InvoiceBeginNo = String.Format("{0:00000000}", blank.InvoiceBeginNo),
                                InvoiceEndNo = String.Format("{0:00000000}", blank.InvoiceEndNo)
                            });
                        }

                        blankItem.Details = details.ToArray();

                        ZipArchiveEntry entry = zip.CreateEntry(String.Format("{0}{1:00}({2})-{3}.xml", trackCode.Year, trackCode.PeriodNo, orgItem.ReceiptNo, count++));
                        using (Stream outStream = entry.Open())
                        {
                            blankItem.ConvertToXml().Save(outStream);
                        }
                        
                    }
                }
            }

            var result = new FilePathResult(outFile, "application/octet-stream");
            result.FileDownloadName = "空白發票字軌.zip";
            return result;
        }

        public ActionResult DownloadVacantNoCsv(InquireNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)InquireVacantNo(viewModel);
            if (!ModelState.IsValid)
            {
                return result;
            }

            IQueryable<UnassignedInvoiceNo> items = (IQueryable<UnassignedInvoiceNo>)result.Model;

            if (items.Count() == 0)
            {
                ViewBag.Message = "資料不存在!!";
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

            return View("~/Views/InvoiceNo/VacantNo/DownloadCsv.cshtml", items);

        }

        private void checkInput(InvoiceNoIntervalViewModel viewModel, InvoiceNoInterval model)
        {
            var table = models.GetTable<InvoiceNoInterval>();

            if (model == null)
            {
                if (!viewModel.TrackID.HasValue)
                {
                    ModelState.AddModelError("TrackID", "字軌未設定!!");
                }

                if (!viewModel.SellerID.HasValue)
                {
                    ModelState.AddModelError("SellerID", "營業人錯誤!!");
                }

            }

            if (!viewModel.StartNo.HasValue || !(viewModel.StartNo >= 0 && viewModel.StartNo < 100000000))
            {
                ModelState.AddModelError("StartNo", "起號非8位整數!!");
            }
            else if (!viewModel.EndNo.HasValue || !(viewModel.EndNo >= 0 && viewModel.EndNo < 100000000))
            {
                ModelState.AddModelError("EndNo", "迄號非8位整數!!");
            }
            else if (viewModel.EndNo <= viewModel.StartNo || ((viewModel.EndNo - viewModel.StartNo + 1) % 50 != 0))
            {
                ModelState.AddModelError("StartNo", "不符號碼大小順序與差距為50之倍數原則!!");
            }
            else
            {
                if (model != null)
                {
                    if (model.InvoiceNoAssignments.Count > 0)
                    {
                        ModelState.AddModelError("IntervalID", "該區間之號碼已經被使用,不可修改!!!!");
                    }
                    else if (table.Any(t => t.IntervalID != model.IntervalID && t.TrackID == model.TrackID && t.StartNo >= viewModel.EndNo && t.InvoiceNoAssignments.Count > 0
                        && t.SellerID == model.SellerID))
                    {
                        ModelState.AddModelError("StartNo", "違反序時序號原則該區段無法修改!!");
                    }
                    else if (table.Any(t => t.IntervalID != model.IntervalID && t.TrackID == model.TrackID
                        && ((t.EndNo <= viewModel.EndNo && t.EndNo >= viewModel.StartNo) || (t.StartNo <= viewModel.EndNo && t.StartNo >= viewModel.StartNo) || (t.StartNo <= viewModel.StartNo && t.EndNo >= viewModel.StartNo) || (t.StartNo <= viewModel.EndNo && t.EndNo >= viewModel.EndNo))))
                    {
                        ModelState.AddModelError("StartNo", "系統中已存在重疊的區段!!");
                    }
                }
                else
                {
                    if (table.Any(t => t.TrackID == viewModel.TrackID && t.StartNo >= viewModel.EndNo && t.InvoiceNoAssignments.Count > 0
                        && t.SellerID == viewModel.SellerID))
                    {
                        ModelState.AddModelError("StartNo", "違反序時序號原則該區段無法新增!!");
                    }
                    else if (table.Any(t => t.TrackID == viewModel.TrackID
                        && ((t.EndNo <= viewModel.EndNo && t.EndNo >= viewModel.StartNo) || (t.StartNo <= viewModel.EndNo && t.StartNo >= viewModel.StartNo) || (t.StartNo <= viewModel.StartNo && t.EndNo >= viewModel.StartNo) || (t.StartNo <= viewModel.EndNo && t.EndNo >= viewModel.EndNo))))
                    {
                        ModelState.AddModelError("StartNo", "系統中已存在重疊的區段!!");
                    }
                }
            }
        }

        public ActionResult LockInterval(InvoiceNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditNoInterval(viewModel);
            InvoiceNoInterval model = result.Model as InvoiceNoInterval;
            if (model == null)
            {
                return result;
            }

            model.LockID = viewModel.LockID;
            models.SubmitChanges();

            return View("~/Views/InvoiceNo/Module/DataItem.cshtml", model);

        }

        public ActionResult CommitItem(InvoiceNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditNoInterval(viewModel);
            InvoiceNoInterval model = result.Model as InvoiceNoInterval;

            checkInput(viewModel, model);

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                if (model != null)
                    ViewBag.DataRole = "edit";
                else
                    ViewBag.DataRole = "add";
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            if (model == null)
            {
                var codeAssignment = models.GetTable<InvoiceTrackCodeAssignment>().Where(t => t.SellerID == viewModel.SellerID && t.TrackID == viewModel.TrackID).FirstOrDefault();
                if (codeAssignment == null)
                {
                    codeAssignment = new InvoiceTrackCodeAssignment
                    {
                        SellerID = viewModel.SellerID.Value,
                        TrackID = viewModel.TrackID.Value
                    };

                    models.GetTable<InvoiceTrackCodeAssignment>().InsertOnSubmit(codeAssignment);
                }

                model = new InvoiceNoInterval { };
                codeAssignment.InvoiceNoInterval.Add(model);
            }

            model.StartNo = viewModel.StartNo.Value;
            model.EndNo = viewModel.EndNo.Value;

            models.SubmitChanges();

            return View("~/Views/InvoiceNo/Module/DataItem.cshtml", model);

        }

        public ActionResult EditNoInterval(InvoiceNoIntervalViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if(viewModel.KeyID!=null)
            {
                viewModel.IntervalID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<InvoiceNoInterval>()
                .Where(d => d.IntervalID == viewModel.IntervalID)
                .FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml", model: "配號區間資料錯誤!!");
            }

            var profile = HttpContext.GetUser();
            if (!profile.IsSystemAdmin())
            {
                if (item.InvoiceTrackCodeAssignment.SellerID != profile.CurrentUserRole.OrganizationCategory.CompanyID
                        && !models.GetTable<InvoiceIssuerAgent>().Any(a => a.IssuerID == item.InvoiceTrackCodeAssignment.SellerID && a.AgentID == profile.CurrentUserRole.OrganizationCategory.CompanyID))
                    return View("~/Views/Shared/AlertMessage.cshtml", model: "配號區間資料錯誤!!");
            }

            return View("~/Views/InvoiceNo/Module/EditItem.cshtml", item);

        }

        public ActionResult DeleteNoInterval(InvoiceNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditNoInterval(viewModel);
            InvoiceNoInterval item = result.Model as InvoiceNoInterval;

            if (item == null)
                return result;

            try
            {

                models.ExecuteCommand("delete InvoiceNoInterval where IntervalID = {0}", item.IntervalID);
                return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            } 
            catch(Exception ex)
            {
                Logger.Error(ex);
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult SplitNoInterval(InvoiceNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditNoInterval(viewModel);
            InvoiceNoInterval item = result.Model as InvoiceNoInterval;

            if (item == null)
                return result;

            try
            {
                var currentNo = item.CurrentAllocatingNo();
                var remained = item.EndNo - currentNo + 1;
                if (remained > 100)
                {
                    var cutpoint = item.EndNo - ((remained - 100) / 50 * 50);
                    if (cutpoint == item.EndNo)
                    {
                        return Json(new { result = false, message = "剩餘號碼不足單一本組數，無法分割！" }, JsonRequestBehavior.AllowGet);
                    }

                    InvoiceNoInterval newItem = new InvoiceNoInterval
                    {
                        EndNo = item.EndNo,
                        SellerID = item.SellerID,
                        TrackID = item.TrackID,
                        StartNo = cutpoint + 1,
                    };
                    models.GetTable<InvoiceNoInterval>().InsertOnSubmit(newItem);
                    item.EndNo = cutpoint;
                    models.SubmitChanges();

                    return Json(new { result = true }, JsonRequestBehavior.AllowGet);

                }
                return Json(new { result = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult CommitAllotment(InvoiceNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditNoInterval(viewModel);
            InvoiceNoInterval item = result.Model as InvoiceNoInterval;

            if (item == null)
                return result;

            if(!viewModel.Parts.HasValue || viewModel.Parts<=0)
            {
                ModelState.AddModelError("Parts", "請輸入均分本數!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            int interval = viewModel.Parts.Value * 50;
            int startNo = item.StartNo + interval;
            int endNo = item.EndNo + 1;
            int intervalEndNo;
            item.EndNo = startNo - 1;

            var intervals = models.GetTable<InvoiceNoInterval>();

            while (startNo < endNo)
            {
                intervalEndNo = Math.Min(startNo + interval, endNo);
                intervals.InsertOnSubmit(new InvoiceNoInterval
                {
                    TrackID = item.TrackID,
                    SellerID = item.SellerID,
                    StartNo = startNo,
                    EndNo = intervalEndNo - 1,
                });
                startNo = intervalEndNo;
            }

            models.SubmitChanges();
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AllotInterval(InvoiceNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditNoInterval(viewModel);
            InvoiceNoInterval item = result.Model as InvoiceNoInterval;

            if (item != null)
            {
                result.ViewName = "~/Views/InvoiceNo/Module/AllotInterval.cshtml";
            }
            return result;
        }


        public ActionResult IntervalItem(InvoiceNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditNoInterval(viewModel);
            InvoiceNoInterval item = result.Model as InvoiceNoInterval;

            if (item == null)
                return result;

            return View("~/Views/InvoiceNo/Module/DataItem.cshtml", item);
        }

        public ActionResult TrackCodeSelector(InquireNoIntervalViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/InvoiceNo/Module/TrackCodeSelector.cshtml");
        }

        [RoleAuthorize(RoleID = new Naming.RoleID[] { Naming.RoleID.ROLE_SYS, Naming.RoleID.ROLE_SELLER })]
        public ActionResult VacantNoIndex()
        {
            return View("~/Views/InvoiceNo/VacantNoIndex.cshtml");
        }

        public ActionResult ProcessVacantNo(InquireNoIntervalViewModel viewModel)
        {
            var profile = HttpContext.GetUser();

            ViewBag.ViewModel = viewModel;

            if (!viewModel.SellerID.HasValue)
            {
                ModelState.AddModelError("SellerID", "請選擇開立人!!");
            }

            if (!viewModel.Year.HasValue)
            {
                ModelState.AddModelError("Year", "請選擇年份!!");
            }

            if (!viewModel.PeriodNo.HasValue)
            {
                ModelState.AddModelError("PeriodNo", "請選擇期別!!");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            viewModel.Push($"{viewModel.SellerID}-{viewModel.Year}{viewModel.PeriodNo:00}.json");

            //TrackNoIntervalManager manager = new TrackNoIntervalManager(models);
            //manager.SettleUnassignedInvoiceNOPeriodically(viewModel.Year.Value, viewModel.PeriodNo.Value, viewModel.SellerID);

            ViewBag.Message = "重新整理進行中，請稍後再次查詢!!";
            return View("~/Views/Shared/AlertMessage.cshtml");


        }

        public ActionResult EditPOSBooklets(InvoiceNoIntervalViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var item = models.GetTable<InvoiceNoInterval>().Where(i => i.IntervalID == viewModel.IntervalID).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/AlertMessage.cshtml",model:"配號區間資料錯誤!!");
            }

            return View("~/Views/InvoiceNo/Module/EditPOSBooklets.cshtml", item);
        }

        public ActionResult UploadInvoiceTrackCode(InvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/InvoiceNo/UploadInvoiceTrackCode.cshtml");
        }


        public ActionResult UploadToPreview(IEnumerable<HttpPostedFileBase> excelFile)
        {
            if (excelFile == null || excelFile.Count() < 1)
            {
                return Json(new { result = false, message = "未選取檔案或檔案上傳失敗" }, JsonRequestBehavior.AllowGet);
            }

            if (excelFile.Count() != 1)
            {
                return Json(new { result = false, message = "請上傳單一檔案" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                var file = excelFile.First();
                String fileName = Path.Combine(Logger.LogDailyPath, $"{DateTime.Now.Ticks}_{Path.GetFileName(file.FileName)}");
                file.SaveAs(fileName);

                List<UploadInvoiceTrackCodeModel> items;
                using (var ds = fileName.ImportExcelXLS())
                {
                    if (ds.Tables.Count == 0)
                    {
                        return Json(new { result = false, message = "Excel檔未包含工作表" }, JsonRequestBehavior.AllowGet);
                    }

                    items = new List<UploadInvoiceTrackCodeModel>();
                    foreach(DataRow r in ds.Tables[0].Rows)
                    {
                        UploadInvoiceTrackCodeModel item = new UploadInvoiceTrackCodeModel { };
                        try
                        {
                            item.ReceiptNo = r.GetString((int)UploadInvoiceTrackField.營業人統編).GetEfficientString();
                            item.TrackCode = r.GetString((int)UploadInvoiceTrackField.字軌).GetEfficientString();
                            item.Year = r.GetData<short>((int)UploadInvoiceTrackField.年份);
                            item.PeriodNo = r.GetData<int>((int)UploadInvoiceTrackField.發票期別);
                            item.StartNo = r.GetData<int>((int)UploadInvoiceTrackField.發票起號);
                            item.EndNo = r.GetData<int>((int)UploadInvoiceTrackField.發票迄號);
                        }
                        catch (Exception ex)
                        {
                            item.Message = ex.Message;
                        }
                        items.Add(item);
                    }
                }

                var profile = HttpContext.GetUser();

                foreach (var viewModel in items.Where(i => i.Message == null))
                {
                    ModelState.Clear();

                    if (!profile.IsSystemAdmin())
                    {
                        viewModel.SellerID = models.GetTable<InvoiceIssuerAgent>().Where(a => a.AgentID == profile.CurrentUserRole.OrganizationCategory.CompanyID
                                        && a.InvoiceInsurer.ReceiptNo == viewModel.ReceiptNo).FirstOrDefault()?.IssuerID;
                    }
                    else
                    {
                        viewModel.SellerID = models.GetTable<Organization>().Where(o => o.ReceiptNo == viewModel.ReceiptNo).FirstOrDefault()?.CompanyID;
                    }

                    var year = viewModel.Year + 1911;
                    viewModel.TrackID = models.GetTable<InvoiceTrackCode>().Where(t => t.Year == year && t.PeriodNo == viewModel.PeriodNo && t.TrackCode == viewModel.TrackCode)
                            .FirstOrDefault()?.TrackID;

                    checkInput(viewModel, null);

                    if(!ModelState.IsValid)
                    {
                        viewModel.Message = ModelState.ErrorMessage();
                    }
                }

                return View("~/Views/InvoiceNo/Module/PreviewInvoiceTrackCode.cshtml", items);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        static class MOF_InvoiceTrackNoField
        {
            public const int 營業人統編 = 0;
            public const int 發票類別代號 = 1;
            public const int 發票類別 = 2;
            public const int 發票期別 = 3;
            public const int 發票字軌名稱 = 4;
            public const int 發票起號 = 5;
            public const int 發票迄號 = 6;
        }

        public ActionResult UploadToPreviewMOF(HttpPostedFileBase theFile)
        {
            if (theFile == null)
            {
                return Json(new { result = false, message = "未選取檔案或檔案上傳失敗" }, JsonRequestBehavior.AllowGet);
            }

            try
            {
                String fileName = Path.Combine(Logger.LogDailyPath, $"{DateTime.Now.Ticks}_{Path.GetFileName(theFile.FileName)}");
                theFile.SaveAs(fileName);

                List<UploadInvoiceTrackCodeModel> items = new List<UploadInvoiceTrackCodeModel>();

                using (StreamReader sr = new StreamReader(fileName))
                {
                    var line = sr.ReadLine();
                    String[] column;
                    Regex checkYear = new Regex("([0-9]+)/([0-9]+)~([0-9]+)/([0-9]+)");
                    while ((column = sr.ReadLine()?.ParseCsvLine()) != null)
                    {
                        if (column.Length > MOF_InvoiceTrackNoField.發票迄號)
                        {
                            UploadInvoiceTrackCodeModel item = new UploadInvoiceTrackCodeModel { };
                            try
                            {
                                item.ReceiptNo = column[MOF_InvoiceTrackNoField.營業人統編].GetEfficientString();
                                item.TrackCode = column[MOF_InvoiceTrackNoField.發票字軌名稱].GetEfficientString();
                                var match = checkYear.Match(column[MOF_InvoiceTrackNoField.發票期別].GetEfficientString());
                                short year;
                                int monthStart;
                                int monthEnd;
                                if (match != null && match.Groups.Count == 5
                                    && match.Groups[1].Value == match.Groups[3].Value
                                    && short.TryParse(match.Groups[1].Value, out year)
                                    && int.TryParse(match.Groups[2].Value, out monthStart)
                                    && int.TryParse(match.Groups[4].Value, out monthEnd)
                                    && monthStart + 1 == monthEnd
                                    && (monthEnd % 2) == 0)
                                {
                                    item.Year = year;
                                    item.PeriodNo = monthEnd / 2;
                                }

                                int startNo, endNo;
                                if (int.TryParse(column[MOF_InvoiceTrackNoField.發票起號].GetEfficientString(), out startNo))
                                {
                                    item.StartNo = startNo;
                                }

                                if (int.TryParse(column[MOF_InvoiceTrackNoField.發票迄號].GetEfficientString(), out endNo))
                                {
                                    item.EndNo = endNo;
                                }
                            }
                            catch (Exception ex)
                            {
                                item.Message = ex.Message;
                            }
                            items.Add(item);
                        }
                    }
                }

                var profile = HttpContext.GetUser();

                foreach (var viewModel in items.Where(i => i.Message == null))
                {
                    ModelState.Clear();

                    if (!profile.IsSystemAdmin())
                    {
                        viewModel.SellerID = models.GetTable<InvoiceIssuerAgent>().Where(a => a.AgentID == profile.CurrentUserRole.OrganizationCategory.CompanyID
                                        && a.InvoiceInsurer.ReceiptNo == viewModel.ReceiptNo).FirstOrDefault()?.IssuerID;
                    }
                    else
                    {
                        viewModel.SellerID = models.GetTable<Organization>().Where(o => o.ReceiptNo == viewModel.ReceiptNo).FirstOrDefault()?.CompanyID;
                    }

                    var year = viewModel.Year + 1911;
                    viewModel.TrackID = models.GetTable<InvoiceTrackCode>().Where(t => t.Year == year && t.PeriodNo == viewModel.PeriodNo && t.TrackCode==viewModel.TrackCode)
                            .FirstOrDefault()?.TrackID;

                    checkInput(viewModel, null);

                    if (!ModelState.IsValid)
                    {
                        viewModel.Message = ModelState.ErrorMessage();
                    }
                }

                return View("~/Views/InvoiceNo/Module/PreviewInvoiceTrackCode.cshtml", items);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Json(new { result = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 上傳發票字軌資料
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public ActionResult CommitUpload(QueryViewModel viewModel)
        {
            // 取畫面資料
            if (viewModel.KeyItems != null && viewModel.KeyItems.Length > 0)
            {
                List<UploadInvoiceTrackCodeModel> items = viewModel.KeyItems
                    .Select(k => JsonConvert.DeserializeObject<UploadInvoiceTrackCodeModel>(k.DecryptData()))
                    .ToList();

                foreach(var item in items)
                {
                    ModelState.Clear();
                    CommitItem(item);
                    if(!ModelState.IsValid)
                    {
                        item.Message = ModelState.ErrorMessage();
                    }
                    else
                    {
                        item.Message = "已匯入成功";
                    }
                }

                return View("~/Views/InvoiceNo/Module/PreviewInvoiceTrackCode.cshtml", items);

            }

            return Json(new { result = false, message = "資料錯誤!!" }, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 取得上傳發票字軌資料的範例檔
        /// </summary>
        /// <returns></returns>
        public ActionResult GetUploadInvoiceTrackCodeSample()
        {
            //var items = models.GetTable<InvoiceItem>().Where(i => false).Take(100);

            Response.Clear();
            Response.ClearContent();
            Response.ClearHeaders();
            Response.AddHeader("Cache-control", "max-age=1");
            Response.ContentType = "application/octet-stream";
            Response.AddHeader("Content-Disposition", String.Format("attachment;filename={0}", HttpUtility.UrlEncode("UploadInvoiceTrackCodeSample.xlsx")));

            using (DataSet ds = new DataSet())
            {
                DataTable table = new DataTable("發票字軌號碼");
                ds.Tables.Add(table);

                table.Columns.Add(UploadInvoiceTrackField.營業人統編.ToString());
                table.Columns.Add(UploadInvoiceTrackField.年份.ToString());
                table.Columns.Add(UploadInvoiceTrackField.發票期別.ToString());
                table.Columns.Add(UploadInvoiceTrackField.字軌.ToString());
                table.Columns.Add(UploadInvoiceTrackField.發票起號.ToString());
                table.Columns.Add(UploadInvoiceTrackField.發票迄號.ToString());

                DataRow row = table.NewRow();
                row[0] = "42523557";
                row[1] = "109";
                row[2] = "1";
                row[3] = "CY";
                row[4] = "00000001";
                row[5] = "00000100";

                table.Rows.Add(row);

                using (var xls = ds.ConvertToExcel())
                {
                    xls.SaveAs(Response.OutputStream);
                }
            }

            Response.End();

            return new EmptyResult();
        }
    }

    public enum UploadInvoiceTrackField
    {
        營業人統編 = 0,
        年份 = 1,
        發票期別 = 2,
        字軌 = 3,
        發票起號 = 4,
        發票迄號 = 5,
    }
}