using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.DataEntity;
using Model.Models.ViewModel;
using Utility;
using Uxnet.Com.DataAccessLayer.Models;

namespace eIVOGo.Models
{
    public partial class InquireOrganizationReceiptNo : CommonInquiry<Organization>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, Organization> models)
        {
            OrganizationQueryViewModel viewModel = CurrentController.ViewBag.ViewModel as OrganizationQueryViewModel;
            var receiptNo = viewModel?.ReceiptNo.GetEfficientString();
            if (receiptNo!=null)
            {
                models.Items = models.Items.Where(i => i.ReceiptNo.StartsWith(receiptNo));
                this.HasSet = true;
            }

            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireCompanyName : CommonInquiry<Organization>
    {

        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, Organization> models)
        {
            OrganizationQueryViewModel viewModel = CurrentController.ViewBag.ViewModel as OrganizationQueryViewModel;
            var companyName = viewModel?.CompanyName.GetEfficientString();
            if (companyName!=null)
            {
                models.Items = models.Items.Where(i => i.CompanyName.Contains(companyName));
                this.HasSet = true;
            }

            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireOrganizationStatus : CommonInquiry<Organization>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, Organization> models)
        {
            OrganizationQueryViewModel viewModel = CurrentController.ViewBag.ViewModel as OrganizationQueryViewModel;
            if (viewModel?.OrganizationStatus.HasValue == true)
            {
                models.Items = models.Items.Where(i => i.OrganizationStatus.CurrentLevel == (int)viewModel.OrganizationStatus);
                this.HasSet = true;
            }

            base.BuildQueryExpression(models);
        }
    }

}