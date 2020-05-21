using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Model.DataEntity;
using Model.Locale;
using Model.Models.ViewModel;
using Utility;
using eIVOGo.Resource.Controllers;

namespace eIVOGo.Controllers
{
    public class TrackCodeController : SampleController<InvoiceItem>
    {
        // GET: TrackCode
        public ActionResult Index()
        {
            return View("~/Views/TrackCode/Index.cshtml");
            //return View();
        }

        public ActionResult Inquire(TrackCodeQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            var pageIndex = viewModel.PageIndex;
            var pageSize = viewModel.PageSize;

            IQueryable<InvoiceTrackCode> items = models.GetTable<InvoiceTrackCode>()
                .Where(t => t.Year == viewModel.Year);

            if (viewModel.PeriodNo.HasValue && viewModel.PeriodNo.Value != 0)
            {
                items = items.Where(t => t.PeriodNo == viewModel.PeriodNo);
            }
            items = items.OrderBy(t => t.PeriodNo).ThenBy(t => t.TrackCode);

            ViewBag.PageSize = viewModel.PageSize.HasValue && viewModel.PageSize > 0 ? viewModel.PageSize.Value : Uxnet.Web.Properties.Settings.Default.PageSize;

            List<InvoiceTrackCodeItem> datas = new List<InvoiceTrackCodeItem>();

            if (items.Count() > 0)
            {
                foreach (var item in items)
                {
                    var aa = new InvoiceTrackCodeItem
                    {
                        TrackID = item.TrackID,
                        TrackCode = item.TrackCode,
                        Year = item.Year,
                        PeriodNo = item.PeriodNo,
                        InvoiceType = item.InvoiceType
                    };

                    datas.Add(aa);
                }
                viewModel.Results = datas;
            }

            if (viewModel.PageIndex.HasValue)
            {
                if (viewModel.Sort != null && viewModel.Sort.Length > 0)
                    ViewBag.Sort = viewModel.Sort.Where(s => s.HasValue).Select(s => s.Value).ToArray();
                ViewBag.PageIndex = viewModel.PageIndex - 1;
                return View("~/Views/TrackCode/Module/ItemList.cshtml", viewModel);
                //return View("~/Views/TrackCode/Module/ItemList.cshtml", JsonConvert.SerializeObject(viewModel));
            }
            else
            {
                ViewBag.PageIndex = 0;

                return View("~/Views/TrackCode/Index.cshtml", viewModel);
                //return View("~/Views/TrackCode/Module/QueryResult.ascx", items);
            }
        }

        //[HttpPost]
        //public ActionResult Index(TrackCodeQueryViewModel viewModel)
        //{
        //    ViewBag.ViewModel = viewModel;

        //    var pageIndex = viewModel.PageIndex;
        //    var pageSize = viewModel.PageSize;

        //    IQueryable<InvoiceTrackCode> items = models.GetTable<InvoiceTrackCode>()
        //        .Where(t => t.Year == viewModel.Year);

        //    if (viewModel.PeriodNo.HasValue)
        //    {
        //        items = items.Where(t => t.PeriodNo == viewModel.PeriodNo);
        //    }
        //    items = items.OrderBy(t => t.PeriodNo).ThenBy(t => t.TrackCode);

        //    ViewBag.PageSize = viewModel.PageSize.HasValue && viewModel.PageSize > 0 ? viewModel.PageSize.Value : Uxnet.Web.Properties.Settings.Default.PageSize;

        //    List<InvoiceTrackCodeItem> datas = new List<InvoiceTrackCodeItem>();

        //    if (items.Count() > 0)
        //    {
        //        foreach (var item in items)
        //        {
        //            var aa = new InvoiceTrackCodeItem
        //            {
        //                TrackID = item.TrackID,
        //                TrackCode = item.TrackCode,
        //                Year = item.Year,
        //                PeriodNo = item.PeriodNo,
        //                InvoiceType = item.InvoiceType
        //            };

        //            datas.Add(aa);
        //        }
        //        viewModel.Results = datas;
        //    }

        //    if (viewModel.PageIndex.HasValue)
        //    {
        //        if (viewModel.Sort != null && viewModel.Sort.Length > 0)
        //            ViewBag.Sort = viewModel.Sort.Where(s => s.HasValue).Select(s => s.Value).ToArray();
        //        ViewBag.PageIndex = viewModel.PageIndex - 1;

        //        //RedirectToAction("~/Views/TrackCode/ItemList.cshtml","TrackCode", viewModel);
        //        //return View("~/Views/TrackCode/ItemList.cshtml", viewModel);
        //        return View("~/Views/TrackCode/Module/ItemList.cshtml", viewModel);


        //        //return View("~/Views/TrackCode/Module/ItemList.cshtml", JsonConvert.SerializeObject(viewModel));
        //    }
        //    else
        //    {
        //        ViewBag.PageIndex = 0;

        //        return View("~/Views/TrackCode/Index.cshtml", viewModel);
        //        //return View("~/Views/TrackCode/Module/QueryResult.ascx", items);
        //    }

        //}

        public ActionResult EditItem(int? id)
        {
            var item = models.GetTable<InvoiceTrackCode>()
                .Where(d => d.TrackID == id)
                .FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/JsAlert.cshtml", model: TrackCode.發票字軌資料錯誤);
            }

            return View("~/Views/TrackCode/Module/EditItem.cshtml", item);

        }

        public ActionResult DeleteItem(int? id)
        {
            var item = models.DeleteAny<InvoiceTrackCode>(d => d.TrackID == id);

            if (item == null)
            {
                return Json(new { result = false, message = TrackCode.發票字軌資料錯誤 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { result = true }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DataItem(int? id)
        {
            var item = models.GetTable<InvoiceTrackCode>()
                .Where(d => d.TrackID == id)
                .FirstOrDefault();

            if (item == null)
            {
                return View("~/Views/Shared/JsAlert.cshtml", model: TrackCode.發票字軌資料錯誤);
            }

            return View("~/Views/TrackCode/Module/DataItem.ascx", item);

        }

        public ActionResult CommitItem(TrackCodeViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;

            viewModel.TrackCode = viewModel.TrackCode.GetEfficientString();
            if (viewModel.TrackCode == null || !Regex.IsMatch(viewModel.TrackCode, "^[A-Za-z]{2}$"))
            {
                ModelState.AddModelError("TrackCode", TrackCode.字軌應為二位英文字母);
            }

            var model = models.GetTable<InvoiceTrackCode>().Where(t => t.TrackID == viewModel.TrackID).FirstOrDefault();
            if (model == null)
            {
                if (!viewModel.PeriodNo.HasValue || viewModel.PeriodNo > 6 || viewModel.PeriodNo < 1)
                {
                    ModelState.AddModelError("PeriodNo", TrackCode.請選擇期別);
                }
                else if (!viewModel.Year.HasValue)
                {
                    ModelState.AddModelError("Year", TrackCode.請選擇年份);
                }
                else
                {
                    var item = models.GetTable<InvoiceTrackCode>().Where(t => t.Year == viewModel.Year && t.TrackCode == viewModel.TrackCode
                    && t.PeriodNo == viewModel.PeriodNo).FirstOrDefault();

                    if (item != null && item.TrackID != viewModel.TrackID)
                    {
                        ModelState.AddModelError("TrackCode", TrackCode.字軌重複);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                if (model != null)
                    ViewBag.DataRole = "edit";
                else
                    ViewBag.DataRole = "add";

                ViewBag.ModelState = ModelState;
                return View("~/Views/Shared/ReportInputError.cshtml");
            }

            if (model == null)
            {
                model = new InvoiceTrackCode
                {
                    Year = viewModel.Year.Value,
                    PeriodNo = viewModel.PeriodNo.Value,
                };
                models.GetTable<InvoiceTrackCode>().InsertOnSubmit(model);
            }

            model.TrackCode = viewModel.TrackCode;
            model.InvoiceType = viewModel.InvoiceType.HasValue
                ? (byte)viewModel.InvoiceType.Value
                : (byte)Naming.InvoiceTypeDefinition.一般稅額計算之電子發票;

            models.SubmitChanges();

            return View("~/Views/TrackCode/Module/DataItem.cshtml", model);

        }


    }
}