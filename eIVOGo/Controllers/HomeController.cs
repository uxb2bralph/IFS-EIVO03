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
        public ActionResult SearchCounterpart(String term)
        {
            GetCounterpart(term);

            IQueryable<Organization> items = (IQueryable<Organization>)ViewBag.DataItems;

            return Json(items.OrderBy(o => o.ReceiptNo).ToArray()
                .Select(o => new
                {
                    label = $"{o.ReceiptNo} {o.CompanyName}",
                    value = o.CompanyID
                }), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetCounterpart(String term)
        {
            var profile = HttpContext.GetUser();

            IQueryable<Organization> items;

            if (profile.IsSystemAdmin())
            {
                items = models.GetTable<Organization>();
            }
            else
            {
                items = models.GetTable<BusinessRelationship>().Where(b => b.MasterID == profile.CurrentUserRole.OrganizationCategory.CompanyID)
                    .Join(models.GetTable<Organization>(), b => b.RelativeID, o => o.CompanyID, (b, o) => o);
            }

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

        public ActionResult ReportError(ActionResultViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            return View("~/Views/Home/Module/ReportError.cshtml");
        }

    }
}