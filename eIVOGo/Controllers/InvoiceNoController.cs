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

namespace eIVOGo.Controllers
{
    [Authorize]
    public class InvoiceNoController : SampleController<InvoiceItem>
    {
        // GET: InvoiceNo
        public ActionResult MaintainInvoiceNoInterval()
        {
            return View();
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

            return View("~/Views/InvoiceNo/Module/QueryResult.ascx", items);
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

            List<InquireVacantNoResult> items = new List<InquireVacantNoResult>();
            foreach(var org in orgItems)
            {
                items.AddRange(((EIVOEntityDataContext)models.GetDataContext()).InquireVacantNo(org.CompanyID,viewModel.Year,viewModel.PeriodNo));
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

            if(items.Count<=0)
            {
                ViewBag.Message = "資料不存在!!";
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
                        foreach(var blank in detailItems.Where(r => !r.CheckPrev.HasValue))
                        {
                            if(blank.CheckNext.HasValue)
                            {
                                var index = detailItems.IndexOf(blank);
                                details.Add(new DetailsBranchTrackBlankItem
                                {
                                    InvoiceBeginNo = String.Format("{0:00000000}", blank.InvoiceNo),
                                    InvoiceEndNo = String.Format("{0:00000000}", detailItems[index+1].InvoiceNo)
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

            List<InquireVacantNoResult> items = (List<InquireVacantNoResult>)result.Model;

            if (items.Count <= 0)
            {
                ViewBag.Message = "資料不存在!!";
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
                    ModelState.AddModelError("TrackID", "未設定字軌!!");
                }

                if (!viewModel.SellerID.HasValue)
                {
                    ModelState.AddModelError("SellerID", "請選擇開立人!!");
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

            return View("~/Views/InvoiceNo/Module/DataItem.ascx", model);

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
                return View("~/Views/Shared/JsAlert.cshtml", model: "配號區間資料錯誤!!");
            }

            var profile = HttpContext.GetUser();
            if(!profile.IsSystemAdmin())
            {
                if(item.InvoiceTrackCodeAssignment.SellerID!=profile.CurrentUserRole.OrganizationCategory.CompanyID)
                    return View("~/Views/Shared/JsAlert.cshtml", model: "配號區間資料錯誤!!");
            }

            return View("~/Views/InvoiceNo/Module/EditItem.ascx", item);

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
                var current = item.InvoiceNoAssignments.OrderByDescending(a => a.InvoiceID).FirstOrDefault();
                if (current != null)
                {
                    var remained = item.EndNo - current.InvoiceNo.Value;
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
                result.ViewName = "~/Views/InvoiceNo/Module/AllotInterval.ascx";
            }
            return result;
        }


        public ActionResult IntervalItem(InvoiceNoIntervalViewModel viewModel)
        {
            ViewResult result = (ViewResult)EditNoInterval(viewModel);
            InvoiceNoInterval item = result.Model as InvoiceNoInterval;

            if (item == null)
                return result;

            return View("~/Views/InvoiceNo/Module/DataItem.ascx", item);
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

            models.GetDataContext().ProcessInvoiceNo(viewModel.SellerID, viewModel.Year, viewModel.PeriodNo);

            ViewBag.Message = "整理完成!!";
            return View("~/Views/Shared/AlertMessage.cshtml");


        }

        public ActionResult EditPOSBooklets(InvoiceNoIntervalViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            var item = models.GetTable<InvoiceNoInterval>().Where(i => i.IntervalID == viewModel.IntervalID).FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/JsAlertMessage.ascx",model:"配號區間資料錯誤!!");
            }

            return View("~/Views/InvoiceNo/Module/EditPOSBooklets.ascx", item);
        }

    }
}