using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Model.DataEntity;
using Model.Locale;
using eIVOGo.Helper;
using Model.Models.ViewModel;
using Business.Helper;
using Utility;

namespace eIVOGo.Controllers
{
    public class DataFlowController : SampleController<InvoiceItem>
    {
        // GET: DataFlow
        public ActionResult SellerSelector(SellerSelectorViewModel viewModel)
        {
            ViewBag.ViewModel = viewModel;
            if ((viewModel.FieldName = viewModel.FieldName.GetEfficientString()) == null)
            {
                viewModel.FieldName = "SellerID";
            }

            if (viewModel.SelectAll==true && (viewModel.SelectorIndication = viewModel.SelectorIndication.GetEfficientString()) == null)
            {
                viewModel.SelectorIndication = "全部";
            }

            var userProfile = HttpContext.GetUser();
            IQueryable<Organization> orgItems = userProfile.InitializeOrganizationQuery(models);

            return View("~/Views/DataFlow/SellerSelector.cshtml", orgItems);

        }
    }
}