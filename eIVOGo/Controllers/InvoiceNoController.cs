using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using Model.DataEntity;
using Model.Models;
using Model.Models.ViewModel;
using Model.Schema.TurnKey.E0402;
using ModelExtension.Helper;
using Newtonsoft.Json;
using Utility;

using res = eIVOGo.Resource.Controllers.InvoiceNo;

namespace eIVOGo.Controllers
{
    [Authorize]
    public class InvoiceNoController : SampleController<InvoiceItem>
    {
        // GET: InvoiceNo
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
                if (viewModel.SellerID.HasValue)
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
                ModelState.AddModelError("Year", res.請選擇年份__);
            }

            if (!viewModel.PeriodNo.HasValue)
            {
                ModelState.AddModelError("PeriodNo", res.請選擇期別__);
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

            List<InquireVacantNoResult> items = new List<InquireVacantNoResult>();
            foreach (var org in orgItems)
            {
                items.AddRange(((EIVOEntityDataContext)models.GetDataContext()).InquireVacantNo(org.CompanyID, viewModel.Year, viewModel.PeriodNo));
            }

            return View("~/Views/InvoiceNo/VacantNo/QueryResult.ascx", items);
        }

        public ActionResult DownloadVacantNo(InquireNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)InquireVacantNo(viewModel);
            if (!ModelState.IsValid)
            {
                return result;
            }

            List<InquireVacantNoResult> items = (List<InquireVacantNoResult>)result.Model;

            if (items.Count <= 0)
            {
                ViewBag.Message = res.資料不存在__;
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

            return zipVacantNo(items);

        }

        private ActionResult zipVacantNo(List<InquireVacantNoResult> vacantNoItems)
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
                    var items = vacantNoItems.GroupBy(r => new { r.SellerID, r.TrackCode, r.Year, r.PeriodNo, r.InvoiceType });

                    foreach (var item in items)
                    {
                        var orgItem = models.GetTable<Organization>().Where(o => o.CompanyID == item.Key.SellerID).First();

                        BranchTrackBlank blankItem = new BranchTrackBlank
                        {
                            Main = new Main
                            {
                                BranchBan = orgItem.ReceiptNo,
                                HeadBan = orgItem.ReceiptNo,
                                YearMonth = String.Format("{0}{1:00}", item.Key.Year - 1911, item.Key.PeriodNo * 2),
                                InvoiceType = item.Key.InvoiceType == (byte)InvoiceTypeEnum.Item08 ? InvoiceTypeEnum.Item08 : InvoiceTypeEnum.Item07,
                                InvoiceTrack = item.Key.TrackCode
                            },
                        };

                        List<DetailsBranchTrackBlankItem> details = new List<DetailsBranchTrackBlankItem>();
                        var detailItems = item.ToList();
                        foreach (var blank in detailItems.Where(r => !r.CheckPrev.HasValue))
                        {
                            if (blank.CheckNext.HasValue)
                            {
                                var index = detailItems.IndexOf(blank);
                                details.Add(new DetailsBranchTrackBlankItem
                                {
                                    InvoiceBeginNo = String.Format("{0:00000000}", blank.InvoiceNo),
                                    InvoiceEndNo = String.Format("{0:00000000}", detailItems[index + 1].InvoiceNo)
                                });
                            }
                            else
                            {
                                details.Add(new DetailsBranchTrackBlankItem
                                {
                                    InvoiceBeginNo = String.Format("{0:00000000}", blank.InvoiceNo),
                                    InvoiceEndNo = String.Format("{0:00000000}", blank.InvoiceNo)
                                });
                            }
                        }

                        blankItem.Details = details.ToArray();

                        ZipArchiveEntry entry = zip.CreateEntry(String.Format("{0}{1:00}({2})-{3}.xml", item.Key.Year, item.Key.PeriodNo, orgItem.ReceiptNo, count++));
                        using (Stream outStream = entry.Open())
                        {
                            blankItem.ConvertToXml().Save(outStream);
                        }

                    }
                }
            }

            var result = new FilePathResult(outFile, "message/rfc822");
            result.FileDownloadName = res.空白發票字軌 + ".zip";
            return result;
        }

        public ActionResult DownloadVacantNoCsv(InquireNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)InquireVacantNo(viewModel);
            if (!ModelState.IsValid)
            {
                return result;
            }

            List<InquireVacantNoResult> items = (List<InquireVacantNoResult>)result.Model;

            if (items.Count <= 0)
            {
                ViewBag.Message = res.資料不存在__;
                return View("~/Views/Shared/AlertMessage.cshtml");
            }

            return View("~/Views/InvoiceNo/VacantNo/DownloadCsv.ascx", items);

        }


        public ActionResult CommitItem(InvoiceNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditNoInterval(viewModel);
            InvoiceNoInterval model = result.Model as InvoiceNoInterval;

            var table = models.GetTable<InvoiceNoInterval>();

            if (model == null)
            {
                if (!viewModel.TrackID.HasValue)
                {
                    ModelState.AddModelError("TrackID", res.未設定字軌__);
                }

                if (!viewModel.SellerID.HasValue)
                {
                    ModelState.AddModelError("SellerID", res.請選擇開立人__);
                }

            }

            if (!viewModel.StartNo.HasValue || !(viewModel.StartNo >= 0 && viewModel.StartNo < 100000000))
            {
                ModelState.AddModelError("StartNo", res.起號非8位整數__);
            }
            else if (!viewModel.EndNo.HasValue || !(viewModel.EndNo >= 0 && viewModel.EndNo < 100000000))
            {
                ModelState.AddModelError("EndNo", res.迄號非8位整數__);
            }
            else if (viewModel.EndNo <= viewModel.StartNo || ((viewModel.EndNo - viewModel.StartNo + 1) % 50 != 0))
            {
                ModelState.AddModelError("StartNo", res.不符號碼大小順序與差距為50之倍數原則__);
            }
            else
            {
                if (model != null)
                {
                    if (model.InvoiceNoAssignments.Count > 0)
                    {
                        ModelState.AddModelError("IntervalID", res.該區間之號碼已經被使用_不可修改____);
                    }
                    else if (table.Any(t => t.IntervalID != model.IntervalID && t.TrackID == model.TrackID && t.StartNo >= viewModel.EndNo && t.InvoiceNoAssignments.Count > 0
                        && t.SellerID == model.SellerID))
                    {
                        ModelState.AddModelError("StartNo", res.違反序時序號原則該區段無法修改__);
                    }
                    else if (table.Any(t => t.IntervalID != model.IntervalID && t.TrackID == model.TrackID
                        && ((t.EndNo <= viewModel.EndNo && t.EndNo >= viewModel.StartNo) || (t.StartNo <= viewModel.EndNo && t.StartNo >= viewModel.StartNo) || (t.StartNo <= viewModel.StartNo && t.EndNo >= viewModel.StartNo) || (t.StartNo <= viewModel.EndNo && t.EndNo >= viewModel.EndNo))))
                    {
                        ModelState.AddModelError("StartNo", res.系統中已存在重疊的區段__);
                    }
                }
                else
                {
                    if (table.Any(t => t.TrackID == viewModel.TrackID && t.StartNo >= viewModel.EndNo && t.InvoiceNoAssignments.Count > 0
                        && t.SellerID == viewModel.SellerID))
                    {
                        ModelState.AddModelError("StartNo", res.違反序時序號原則該區段無法新增__);
                    }
                    else if (table.Any(t => t.TrackID == viewModel.TrackID
                        && ((t.EndNo <= viewModel.EndNo && t.EndNo >= viewModel.StartNo) || (t.StartNo <= viewModel.EndNo && t.StartNo >= viewModel.StartNo) || (t.StartNo <= viewModel.StartNo && t.EndNo >= viewModel.StartNo) || (t.StartNo <= viewModel.EndNo && t.EndNo >= viewModel.EndNo))))
                    {
                        ModelState.AddModelError("StartNo", res.系統中已存在重疊的區段__);
                    }
                }
            }

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
                codeAssignment.InvoiceNoIntervals.Add(model);
            }

            model.StartNo = viewModel.StartNo.Value;
            model.EndNo = viewModel.EndNo.Value;

            models.SubmitChanges();

            return View("~/Views/InvoiceNo/Module/DataItem.cshtml", model);

        }

        public ActionResult EditNoInterval(InvoiceNoIntervalViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if (viewModel.KeyID != null)
            {
                viewModel.IntervalID = viewModel.DecryptKeyValue();
            }

            var item = models.GetTable<InvoiceNoInterval>()
                .Where(d => d.IntervalID == viewModel.IntervalID)
                .FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/JsAlert.cshtml", model: res.配號區間資料錯誤__);
            }

            var profile = HttpContext.GetUser();
            if (!profile.IsSystemAdmin())
            {
                if (item.InvoiceTrackCodeAssignment.SellerID != profile.CurrentUserRole.OrganizationCategory.CompanyID)
                    return View("~/Views/Shared/JsAlert.cshtml", model: res.配號區間資料錯誤__);
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
            catch (Exception ex)
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
                var current = item.InvoiceNoAssignments.OrderByDescending(a => a.InvoiceID).FirstOrDefault();
                if (current != null)
                {
                    var remained = item.EndNo - current.InvoiceNo.Value;
                    if (remained > 100)
                    {
                        var cutpoint = item.EndNo - ((remained - 100) / 50 * 50);
                        if (cutpoint == item.EndNo)
                        {
                            return Json(new { result = false, message = res.剩餘號碼不足單一本組數_無法分割_ }, JsonRequestBehavior.AllowGet);
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

            if (!viewModel.Parts.HasValue || viewModel.Parts <= 0)
            {
                ModelState.AddModelError("Parts", res.請輸入均分本數__);
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
            return View("~/Views/InvoiceNo/Module/TrackCodeSelector.ascx");
        }

        public ActionResult VacantNoIndex()
        {
            return View();
        }

        public ActionResult ProcessVacantNo(InquireNoIntervalViewModel viewModel)
        {
            var profile = HttpContext.GetUser();

            ViewBag.ViewModel = viewModel;

            if (!viewModel.SellerID.HasValue)
            {
                ModelState.AddModelError("SellerID", res.請選擇開立人__);
            }

            if (!viewModel.Year.HasValue)
            {
                ModelState.AddModelError("Year", res.請選擇年份__);
            }

            if (!viewModel.PeriodNo.HasValue)
            {
                ModelState.AddModelError("PeriodNo", res.請選擇期別__);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ModelState = ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            models.GetDataContext().ProcessInvoiceNo(viewModel.SellerID, viewModel.Year, viewModel.PeriodNo);

            ViewBag.Message = res.整理完成__;
            return View("~/Views/Shared/AlertMessage.cshtml");


        }

        public ActionResult EditPOSBooklets(InvoiceNoIntervalViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var item = models.GetTable<InvoiceNoInterval>().Where(i => i.IntervalID == viewModel.IntervalID).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/JsAlertMessage.ascx", model: res.配號區間資料錯誤__);
            }

            return View("~/Views/InvoiceNo/Module/EditPOSBooklets.cshtml", item);
        }

        public ActionResult UploadInvoiceTrackCode(InvoiceViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/InvoiceNo/Module/UploadInvoiceTrackCode.cshtml");
        }


        public ActionResult UploadToPreView(IEnumerable<HttpPostedFileBase> excelFile, string callback)
        {
            try
            {
                if (excelFile == null || excelFile.First() == null) throw new ApplicationException("未選取檔案或檔案上傳失敗");
                if (excelFile.Count() != 1) throw new ApplicationException("請上傳單一檔案");
                var file = excelFile.First();
                if (Path.GetExtension(file.FileName) != ".xlsx") throw new ApplicationException("請使用Excel 2007(.xlsx)格式");
                var stream = file.InputStream;
                XLWorkbook wb = new XLWorkbook(stream);
                if (wb.Worksheets.Count > 1)
                    throw new ApplicationException("Excel檔包含多個工作表");

                var items =

                       wb.Worksheets.First().RowsUsed().Select(row =>
                           string.Join("\t",
                               row.Cells(1, row.LastCellUsed(false).Address.ColumnNumber)
                               .Select(cell => cell.GetValue<string>()).ToArray()
                           )).ToArray();

                var csv =
                    string.Join("\n",
                        wb.Worksheets.First().RowsUsed().Select(row =>
                            string.Join("\t",
                                row.Cells(1, row.LastCellUsed(false).Address.ColumnNumber)
                                .Select(cell => cell.GetValue<string>()).ToArray()
                            )).ToArray());

                var models = new List<UploadInvoiceTrackCodeModel>();
                if (items.Count() > 1)
                {
                    for (int i = 1; i < items.Count(); i++)
                    {
                        var cells = items[i].Split('\t');

                        Regex reg1 = new Regex(@"^[A-Za-z]+$");

                        if (!(Convert.ToInt32(cells[0]) > 0) || cells[0].Length != 8)
                        {
                            throw new ApplicationException("營業人統編格式錯誤");
                        }

                        if (!(Convert.ToInt32(cells[1]) > 0) || cells[1].Length != 3)
                        {
                            throw new ApplicationException("年份格式錯誤");
                        }

                        if (!(Convert.ToInt32(cells[2]) > 0))
                        {
                            throw new ApplicationException("發票期別格式錯誤");
                        }

                        switch (Convert.ToInt32(cells[2]))
                        {
                            case 2:
                            case 4:
                            case 6:
                            case 8:
                            case 10:
                            case 12:
                                break;
                            default:
                                throw new ApplicationException("發票期別格式錯誤");
                        }

                        if (!reg1.IsMatch(cells[3]))
                        {
                            throw new ApplicationException("字軌格式錯誤");
                        }

                        if (!(Convert.ToInt32(cells[4]) > 0) || cells[4].Length != 8)
                        {
                            throw new ApplicationException("發票起號錯誤");
                        }

                        if (!(Convert.ToInt32(cells[5]) > 0) || cells[5].Length != 8)
                        {
                            throw new ApplicationException("發票迄號錯誤");
                        }

                        var model = new UploadInvoiceTrackCodeModel();
                        short year = Convert.ToInt16(cells[1]);
                        model.ReceiptNo = cells[0];
                        model.Year = year;
                        model.PeriodNo = Convert.ToInt32(cells[2]);
                        model.TrackCode = cells[3];
                        model.StartNo = Convert.ToInt32(cells[4]);
                        model.EndNo = Convert.ToInt32(cells[5]);

                        models.Add(model);
                    }
                }

                ViewBag.Models = models;

                return Content($@"<script>
{callback}({JsonConvert.SerializeObject(models)});
</script>", "text/html");
            }
            catch (Exception ex)
            {
                return Content($"<script>alert({JsonConvert.SerializeObject(ex.Message)})</script>", "text/html");
            }
        }


        /// <summary>
        /// 上傳發票字軌資料
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public ActionResult Upload(string viewModel)
        {
            // 取畫面資料
            List<UploadInvoiceTrackCodeModel> datas = JsonConvert.DeserializeObject<List<UploadInvoiceTrackCodeModel>>(viewModel);
            var insertDatas = new List<InvoiceNoInterval>();

            foreach (var item in datas)
            {
                // step1.
                string result = string.Empty;
                var year = item.Year + 1911;
                var periodNo = item.PeriodNo / 2;
                var startNo = item.StartNo;
                var endNo = item.EndNo;
                var model = new InvoiceNoInterval { };
                var sellerID = 0;
                var trackID = 0;

                //try
                //{
                // step1.判斷是否有此營業人
                var Organization = models.GetTable<Organization>().Where(t => t.ReceiptNo == item.ReceiptNo).FirstOrDefault();
                if (Organization != null)
                {
                    sellerID = Organization.CompanyID;

                    // step2.判斷是否設定字軌
                    try
                    {
                        trackID = models.GetTable<InvoiceTrackCode>()
                                   .Where(t => t.TrackCode == item.TrackCode
                                   && t.Year == year
                                   && t.PeriodNo == periodNo)
                                   .FirstOrDefault().TrackID;

                        if (trackID == 0)
                        {
                            ModelState.AddModelError("ErrorMsg", res.未設定字軌__);
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("ErrorMsg", res.未設定字軌__);
                    }                   

                    // step3.判斷發票起迄號正確性
                    if (!startNo.HasValue || !(startNo >= 0 && startNo < 100000000))
                    {
                        ModelState.AddModelError("ErrorMsg", res.起號非8位整數__);
                    }
                    else if (!endNo.HasValue || !(endNo >= 0 && endNo < 100000000))
                    {
                        ModelState.AddModelError("ErrorMsg", res.迄號非8位整數__);
                    }
                    else if (endNo <= startNo || ((endNo - startNo + 1) % 50 != 0))
                    {
                        ModelState.AddModelError("ErrorMsg", res.不符號碼大小順序與差距為50之倍數原則__);
                    }
                    else
                    {
                        var invoiceNoIntervals = models.GetTable<InvoiceNoInterval>();
                        if (invoiceNoIntervals.Any(t => t.TrackID == trackID && t.StartNo >= endNo && t.InvoiceNoAssignments.Count > 0
                            && t.SellerID == sellerID))
                        {
                            ModelState.AddModelError("ErrorMsg", res.違反序時序號原則該區段無法新增__);
                        }
                        else if (invoiceNoIntervals.Any(t => t.TrackID == trackID
                            && ((t.EndNo <= endNo && t.EndNo >= startNo) || (t.StartNo <= endNo && t.StartNo >= startNo) || (t.StartNo <= startNo && t.EndNo >= startNo) || (t.StartNo <= endNo && t.EndNo >= endNo))))
                        {
                            ModelState.AddModelError("ErrorMsg", res.系統中已存在重疊的區段__);
                        }

                    }

                    if (!ModelState.IsValid)
                    {
                        ViewBag.ModelState = ModelState;

                        ViewBag.DataRole = "add";

                        return View("~/Views/Shared/ReportInputError.cshtml");
                    }

                    // step4.判斷此供應商是否以分配發票字軌，若無則儲存一筆 
                    InvoiceTrackCodeAssignment codeAssignment;
                    codeAssignment = models.GetTable<InvoiceTrackCodeAssignment>().Where(t => t.SellerID == sellerID && t.TrackID == trackID).FirstOrDefault();

                    if (codeAssignment == null)
                    {
                        codeAssignment = new InvoiceTrackCodeAssignment
                        {
                            SellerID = sellerID,
                            TrackID = trackID
                        };

                        models.GetTable<InvoiceTrackCodeAssignment>().InsertOnSubmit(codeAssignment);
                    }

                    var insertData = new InvoiceNoInterval()
                    {
                        TrackID = trackID,
                        SellerID = sellerID,
                        StartNo = item.StartNo.Value,
                        EndNo = item.EndNo.Value
                    };

                    insertDatas.Add(insertData);
                }
                else
                {
                    // 營業人統編錯誤
                    ModelState.AddModelError("ErrorMsg", "營業人統編錯誤!!");

                    if (!ModelState.IsValid)
                    {
                        ViewBag.ModelState = ModelState;

                        ViewBag.DataRole = "add";

                        return View("~/Views/Shared/ReportInputError.cshtml");
                    }
                }  
            }

            // step5.儲存發票字軌資料
            foreach (var item in insertDatas)
            {
                models.GetTable<InvoiceNoInterval>().InsertOnSubmit(item);
            }

            models.SubmitChanges();

            return Content($"<script>alert({JsonConvert.SerializeObject("已匯入成功")})</script>", "text/html");
            //return Json(new { result = true }, JsonRequestBehavior.AllowGet);
            //return View("~/Views/InvoiceNo/MaintainInvoiceNoInterval.cshtml");
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
                DataTable table = new DataTable("Sheet1");
                ds.Tables.Add(table);
                
                table.Columns.Add("營業人統編");
                table.Columns.Add("年份");
                table.Columns.Add("發票期別");
                table.Columns.Add("字軌");
                table.Columns.Add("發票起號");
                table.Columns.Add("發票迄號");

                DataRow row = table.NewRow();
                row[0] = "42523557";
                row[1] = "109";
                row[2] = "8";
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
}