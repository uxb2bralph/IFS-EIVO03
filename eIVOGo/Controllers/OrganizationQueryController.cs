using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Business.Helper;
using DocumentFormat.OpenXml.Spreadsheet;
using eIVOGo.Helper;
using eIVOGo.Models;
using Model.DataEntity;
using Model.Models.ViewModel;
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
        public ActionResult Index(OrganizationQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
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

        public ActionResult InquireCompany(OrganizationQueryViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            //ViewBag.HasQuery = true;
            //ViewBag.PrintAction = "PrintResult";
            ModelSource<Organization> tmpModels = new ModelSource<Organization>(models);
            tmpModels.Items = tmpModels.Items.Where(o => o.OrganizationStatus != null);
            tmpModels.Inquiry = createModelInquiry();
            tmpModels.BuildQuery();
            models.InquiryHasError = tmpModels.InquiryHasError;

            if(viewModel.CategoryID.HasValue)
            {
                var categoryItems = models.GetTable<OrganizationCategory>().Where(c => c.CategoryID == (int)viewModel.CategoryID);
                tmpModels.Items = tmpModels.Items.Where(o => categoryItems.Any(c => c.CompanyID == o.CompanyID));
            }

            if(viewModel.AgentID.HasValue)
            {
                var agentItems = models.GetTable<InvoiceIssuerAgent>().Where(a => a.AgentID == viewModel.AgentID);
                if (viewModel.BranchRelation == true)
                {
                    agentItems = agentItems.Where(a => a.RelationType == (int)InvoiceIssuerAgent.Relationship.MasterBranch);
                }

                tmpModels.Items = tmpModels.Items.Where(o => agentItems.Any(c => c.IssuerID == o.CompanyID));
            }


            viewModel.RecordCount = tmpModels.Items.Count();
            if (viewModel.PageIndex.HasValue)
            {
                viewModel.PageIndex--;
                if(viewModel.CategoryID == CategoryDefinition.CategoryEnum.發票開立營業人
                    || viewModel.CategoryID == CategoryDefinition.CategoryEnum.境外電商
                    || viewModel.CategoryID == CategoryDefinition.CategoryEnum.經銷商)
                {
                    return View("~/Views/OrganizationQuery/Module/SellerItemList.cshtml", tmpModels.Items);
                }
                else
                {
                    return View("~/Views/OrganizationQuery/Module/CompanyItemList.cshtml", tmpModels.Items);
                }
            }
            else
            {
                viewModel.PageIndex = 0;
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
