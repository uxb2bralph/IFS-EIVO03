using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Models;
using Model.DataEntity;
using Model.Security.MembershipManagement;

namespace eIVOGo.Controllers
{
    public class OrganizationQueryController : SampleController<InvoiceItem>
    {
        protected ModelSourceInquiry<Organization> createModelInquiry()
        {
            UserProfileMember userProfile = WebPageUtility.UserProfile;

            return (ModelSourceInquiry<Organization>)(new InquireOrganizationReceiptNo { CurrentController = this })
                .Append(new InquireOrganizationStatus { CurrentController = this })
                .Append(new InquireCompanyName { CurrentController = this });

        }
        // GET: OrganizationQuery
        public ActionResult Index()
        {
            return View("~/Views/OrganizationQuery/Index.cshtml");
        }

        //public ActionResult Inquire()
        //{
        //    //ViewBag.HasQuery = true;
        //    //ViewBag.PrintAction = "PrintResult";
        //    ModelSource<Organization> models = new ModelSource<Organization>();
        //    TempData.SetModelSource(models);
        //    models.Items = models.Items.Where(o => o.OrganizationStatus != null);
        //    models.Inquiry = createModelInquiry();
        //    models.BuildQuery();

        //    return View("~/Views/OrganizationQuery/InquiryResult.cshtml", models.Inquiry);
        //}

        public ActionResult InquireCompany(int? pageIndex)
        {
            //ViewBag.HasQuery = true;
            //ViewBag.PrintAction = "PrintResult";
            ModelSource<Organization> tmpModels = new ModelSource<Organization>(models);
            tmpModels.Items = tmpModels.Items.Where(o => o.OrganizationStatus != null);
            tmpModels.Inquiry = createModelInquiry();
            tmpModels.BuildQuery();
            models.InquiryHasError = tmpModels.InquiryHasError;
            if (pageIndex.HasValue)
            {
                ViewBag.PageIndex = pageIndex - 1;
                return View("~/Views/OrganizationQuery/Module/CompanyList.cshtml", tmpModels.Items);
            }
            else
            {
                ViewBag.PageIndex = 0;
                return View("~/Views/OrganizationQuery/Module/CompanyResult.cshtml", tmpModels.Items);
            }
        }


        //public ActionResult GridPage(int index, int size)
        //{
        //    //ViewBag.HasQuery = true;
        //    ModelSource<Organization> models = new ModelSource<Organization>();
        //    TempData.SetModelSource(models);
        //    models.Inquiry = createModelInquiry();
        //    models.BuildQuery();

        //    if (index > 0)
        //        index--;
        //    else
        //        index = 0;

        //    return View(models.Items.OrderByDescending(d => d.ReceiptNo)
        //        .Skip(index * size).Take(size)
        //        .ToArray());
        //}

    }
}
