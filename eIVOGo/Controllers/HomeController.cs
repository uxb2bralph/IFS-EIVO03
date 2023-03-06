using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Model.DataEntity;
using eIVOGo.Helper;
using Utility;
using Model.Locale;
using Model.Security.MembershipManagement;
using Business.Helper;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using Newtonsoft.Json;
using Model.Helper;

namespace eIVOGo.Controllers
{
    public class HomeController : SampleController<InvoiceItem>
    {
        // GET: Home
        public ActionResult MainPage()
        {
            return View();
        }

        public ActionResult SearchCompany(String term,bool? encrypt)
        {
            IQueryable<Organization> items = models.GetTable<Organization>();

            if (!String.IsNullOrEmpty(term))
            {
                items = items
                    .Where(f => f.ReceiptNo.StartsWith(term) || f.CompanyName.Contains(term));
            }
            else
            {
                items = items.Where(f => false);
            }

            return Json(items.OrderBy(o => o.ReceiptNo).ToArray()
                .Select(o => new
                {
                    label = $"{o.ReceiptNo} {o.CompanyName}",
                    value = encrypt==true ? o.CompanyID.EncryptKey() : o.CompanyID.ToString()
                }), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetCompany(String term)
        {
            IQueryable<Organization> items = models.GetTable<Organization>();

            if (!String.IsNullOrEmpty(term))
            {
                items = items
                    .Where(f => f.ReceiptNo.StartsWith(term) || f.CompanyName.Contains(term));
            }
            else
            {
                items = items.Where(f => false);
            }

            var item = items.FirstOrDefault();

            if (item != null)
            {
                return Content(JsonConvert.SerializeObject(item), "application/json");
            }
            else
            {
                return new EmptyResult();
            }
        }

        [Authorize]
        public ActionResult SearchCounterpart(String term, int? sellerID)
        {
            GetCounterpart(term);

            IQueryable<Organization> items = (IQueryable<Organization>)ViewBag.DataItems;

            if(sellerID.HasValue)
            {
                var dataItems = items
                    .OrderBy(o => o.ReceiptNo)
                    .Select(o => new { C = o, R = o.RelativeRelation.Where(b => b.MasterID == sellerID).FirstOrDefault() })
                    .ToArray();
                return Json(dataItems
                    .Select(o => new
                    {
                        label = $"{o.C.ReceiptNo} {o.R?.CompanyName ?? o.C.CompanyName}",
                        value = o.C.CompanyID
                    }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(items.OrderBy(o => o.ReceiptNo).ToArray()
                    .Select(o => new
                    {
                        label = $"{o.ReceiptNo} {o.CompanyName}",
                        value = o.CompanyID
                    }), JsonRequestBehavior.AllowGet);

            }
        }

        [Authorize]
        public ActionResult GetCounterpart(String term)
        {
            var profile = HttpContext.GetUser();

            IQueryable<Organization> items = models.GetTable<Organization>();

            if (!String.IsNullOrEmpty(term))
            {
                items = items
                    .Where(f => f.ReceiptNo.StartsWith(term) || f.CompanyName.Contains(term));
            }
            else
            {
                items = items.Where(f => false);
            }

            ViewBag.DataItems = items;

            if (profile.IsSystemAdmin())
            {
                var item = items.FirstOrDefault();

                if (item != null)
                {
                    return Content(JsonConvert.SerializeObject(item), "application/json");
                }
                else
                {
                    return new EmptyResult();
                }
            }
            else
            {
                var item = models.GetTable<BusinessRelationship>().Where(b => b.MasterID == profile.CurrentUserRole.OrganizationCategory.CompanyID)
                    .Join(items, b => b.RelativeID, o => o.CompanyID, (b, o) => b).FirstOrDefault();

                if (item != null)
                {
                    return Json(new { item.Counterpart.ReceiptNo, item.CompanyName, item.Addr, item.Phone, item.ContactEmail, item.CustomerNo }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return new EmptyResult();
                }
            }

        }

        public ActionResult ReportError(ActionResultViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/Home/Module/ReportError.cshtml");
        }

        [Authorize]
        public ActionResult Download(QueryViewModel viewModel)
        {
            if (viewModel.KeyID != null)
            {
                String fileName = viewModel.KeyID.DecryptData();
                if (System.IO.File.Exists(fileName))
                {
                    return File(fileName, "application/octet-stream");
                }
            }

            ViewBag.CloseWindow = true;
            return View("~/Views/Shared/AlertMessage.cshtml", model: "檔案錯誤!!");
        }

        public ActionResult SystemInfo()
        {
            return Json(new
            {
                Version = "2023-02-13",
            }, JsonRequestBehavior.AllowGet);
        }

    }
}