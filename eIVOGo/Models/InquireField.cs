using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;
using eIVOGo.Helper;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;

using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Com.DataAccessLayer.Models;
using DataAccessLayer.basis;

namespace eIVOGo.Models
{
    public partial class CommonInquiry<TEntity> : ModelSourceInquiry<TEntity>
        where TEntity : class,new()
    {
        protected InquireInvoiceViewModel _viewModel;
        protected Controller _currentController;
        public CommonInquiry() : base()
        {

        }
        public Controller CurrentController
        {
            get
            {
                return _currentController;
            }
            set
            {
                _currentController = value;
                _viewModel = null;
                if (_currentController != null)
                {
                    _viewModel = (InquireInvoiceViewModel)_currentController.ViewBag.ViewModel;
                }
            }
        }

        public void Render(HtmlHelper Html)
        {
            if (this.ControllerName != null && this.ActionName != null)
            {
                Html.RenderAction(this.ActionName, this.ControllerName);
            }
            else if(this.ViewName!=null)
            {
                Html.RenderPartial(this.ViewName);
            }

            if (_chainedInquiry != null)
            {
                foreach (var inquiry in _chainedInquiry)
                {
                    ((CommonInquiry<TEntity>)inquiry).Render(Html);
                }
            }
        }

        public void RenderAlert(HtmlHelper Html)
        {
            if (this.HasError)
                Html.RenderPartial("../Module/InquiryAlert", this);
            if (_chainedInquiry != null)
            {
                foreach (var inquiry in _chainedInquiry)
                {
                    ((CommonInquiry<TEntity>)inquiry).RenderAlert(Html);
                }
            }
        }

        public void RenderQueryMessage(HtmlHelper Html)
        {
            if (!String.IsNullOrEmpty(QueryMessage))
                Html.RenderPartial("../Module/QueryMessage", this);
            if (_chainedInquiry != null)
            {
                foreach (var inquiry in _chainedInquiry)
                {
                    ((CommonInquiry<TEntity>)inquiry).RenderQueryMessage(Html);
                }
            }
        }

    }


    public partial class InquireCustomerID : CommonInquiry<InvoiceItem>
    {
        
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryByCustomerID(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireAllowanceCustomerID : CommonInquiry<InvoiceAllowance>
    {

        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            if (!String.IsNullOrEmpty(CurrentController.Request["customerID"]))
            {
                effective = true;
                models.Items = models.Items.Where(i => i.InvoiceAllowanceBuyer.CustomerID == CurrentController.Request["customerID"].Trim());
            }

            base.BuildQueryExpression(models);
        }
    }


    public partial class InquireInvoiceSeller : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            if (!String.IsNullOrEmpty(CurrentController.Request["sellerID"]))
            {
                effective = true;
                models.Items = models.Items.Where(d => d.InvoiceSeller.SellerID == int.Parse(CurrentController.Request["sellerID"]));
            }

            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireAllowanceSeller : CommonInquiry<InvoiceAllowance>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            models.Items = models.Items.QueryBySeller(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireInvoiceBuyer : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryByBuyerReceiptNo(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireAllowanceBuyer : CommonInquiry<InvoiceAllowance>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            models.Items = models.Items.QueryByBuyerReceiptNo(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireInvoiceBuyerByName : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryByBuyerName(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireAllowanceBuyerByName : CommonInquiry<InvoiceAllowance>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            models.Items = models.Items.QueryByBuyerName(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireInvoiceDate : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryByInvoiceDate(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }

    }

    public partial class InquireAllowanceDate : CommonInquiry<InvoiceAllowance>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            models.Items = models.Items.QueryByAllowanceDate(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }

    }

    public partial class InquireInvoiceConsumption : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryByProcessType(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }

        //public bool QueryForB2C
        //{
        //    get
        //    {
        //        return CurrentController.Request["cc1"] == "B2C";
        //    }
        //}
    }

    public partial class InquireAllowanceConsumption : CommonInquiry<InvoiceAllowance>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            models.Items = models.Items.QueryByProcessType(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }

        //public bool QueryForB2C
        //{
        //    get
        //    {
        //        return CurrentController.Request["cc1"] == "B2C";
        //    }
        //}
    }

    public partial class InquireInvoiceConsumptionExtensionToPrint : CommonInquiry<InvoiceItem>
    {
        //private InquireInvoiceConsumption _inquiry;

        //public InquireInvoiceConsumptionExtensionToPrint(InquireInvoiceConsumption inquiry)
        //{
        //    _inquiry = inquiry;
        //}

        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.Where(i => i.PrintMark == "Y" || (i.PrintMark == "N" && i.InvoiceWinningNumber != null)
                /*&& (!i.CDS_Document.DocumentPrintLog.Any(l => l.TypeID == (int)Naming.DocumentTypeDefinition.E_Invoice) || i.CDS_Document.DocumentAuthorization != null)*/);
        }
    }

    public partial class InquireInvoiceByRole : CommonInquiry<InvoiceItem>
    {
        protected UserProfileMember _userProfile;

        public InquireInvoiceByRole(UserProfileMember profile)
        {
            _userProfile = profile;
        }

        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.GetDataContext().FilterInvoiceByRole(_userProfile, models.Items);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireAllowanceByRole : CommonInquiry<InvoiceAllowance>
    {
        protected UserProfileMember _userProfile;

        public InquireAllowanceByRole(UserProfileMember profile)
        {
            _userProfile = profile;
        }

        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            switch ((Naming.RoleID)_userProfile.CurrentUserRole.RoleID)
            {
                case Naming.RoleID.ROLE_GUEST:
                case Naming.RoleID.ROLE_BUYER:
                case Naming.RoleID.ROLE_SYS:
                    break;

                default:
                    models.Items = models.GetDataContext()
                                .GetAllowanceByAgent(models.Items, _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                    break;
            }


            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireEffectiveInvoice : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryEffective(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireEffectiveAllowance : CommonInquiry<InvoiceAllowance>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            models.Items = models.Items.QueryEffective(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }

    }

    public partial class InquireWinningInvoice : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryByWinning(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireInvoicePeriod : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            if (CurrentController.Request["period"] != null)
            {
                String[] period = CurrentController.Request["period"].Split(',');
                if (period != null && period.Length == 2)
                {
                    int year, p;
                    if (int.TryParse(period[0], out year) && int.TryParse(period[1], out p) && p >= 1 && p <= 6)
                    {
                        DateTime dateFrom = new DateTime(year, p * 2 - 1, 1);
                        models.Items = models.Items.Where(i => i.InvoiceDate >= dateFrom && i.InvoiceDate < dateFrom.AddMonths(2));
                        effective = true;
                        QueryMessage = (dateFrom.Year - 1911) + "年 " + dateFrom.Month + "~" + (dateFrom.Month + 1) + "月";
                    }
                }
            }

            base.BuildQueryExpression(models);
        }

    }

    public partial class InquireDonatedInvoice : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.Where(i => i.InvoiceCancellation == null
                && i.InvoiceDonation != null);

            if (CurrentController.Request["donation"] == "winning")
            {
                models.Items = models.Items.Where(i => i.InvoiceWinningNumber != null);
                effective = true;
            }

            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireDonatory : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryByWelfare(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireInvoiceAttachment : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryByAttachment(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }
    }


    public partial class InquireInvoiceNo : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            models.Items = models.Items.QueryByInvoiceNo(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }

    }

    public partial class InquireAllowanceNo : CommonInquiry<InvoiceAllowance>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            models.Items = models.Items.QueryByDataNo(_viewModel, models, ref effective);
            base.BuildQueryExpression(models);
        }

    }

    public partial class InquireInvoiceAgent : CommonInquiry<InvoiceItem>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceItem> models)
        {
            if (!String.IsNullOrEmpty(CurrentController.Request["agentID"]))
            {
                effective = true;
                models.Items = models.GetDataContext().GetInvoiceByAgent(models.Items, int.Parse(CurrentController.Request["agentID"]));
            }

            base.BuildQueryExpression(models);
        }
    }

    public partial class InquireAllowanceAgent : CommonInquiry<InvoiceAllowance>
    {
        public override void BuildQueryExpression(ModelSource<EIVOEntityDataContext, InvoiceAllowance> models)
        {
            if (_viewModel!=null && _viewModel.AgentID.HasValue)
            {
                effective = true;
                models.Items = models.GetDataContext().GetAllowanceByAgent(models.Items, _viewModel.AgentID.Value);
            }

            base.BuildQueryExpression(models);
        }
    }



}