using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.Locale;
using Model.Security.MembershipManagement;
using Model.InvoiceManagement;
using Business.Helper;
using Model.DataEntity;
using Uxnet.Web.Module.Common;
using System.Linq.Expressions;
using Utility;
using eIVOGo.Helper;

namespace eIVOGo.Module.Inquiry
{
    public partial class queryInvoiceAndAllowance : System.Web.UI.UserControl
    {
        protected UserProfileMember _userProfile;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
            if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
            {
                this.trGroupMember.Visible = false;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnSearch.Load += new EventHandler(btnSearch_Load);
        }

        void btnSearch_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                foreach (Naming.InvoiceStepDefinition step in Enum.GetValues(typeof(Naming.InvoiceStepDefinition)))
                {
                    this.ddlStep.Items.Add(new ListItem(step.ToString(), Convert.ToInt32(step).ToString()));
                }

                foreach (Naming.InvoiceCenterBusinessType type in Enum.GetValues(typeof(Naming.InvoiceCenterBusinessType)))
                {
                    this.ddlType.Items.Add(new ListItem(type.ToString(), Convert.ToInt32(type).ToString()));
                }
            }

            if (Request.Form[btnSearch.UniqueID] == null)
            {
                initializeData();
            }
        }

        protected virtual void initializeData()
        {
            if (btnSearch.CommandArgument == "Query")
            {
                //InvoiceAllowanceQueryList allowanceListView;
                InvoiceItemQueryList invoiceListView;
                switch (rdbSearchItem.SelectedIndex)
                {
                    case 0:
                        invoiceListView = (InvoiceItemQueryList)this.LoadControl("InvoiceItemQueryList.ascx");
                        invoiceListView.InitializeAsUserControl(this.Page);
                        invoiceListView.QueryExpr = buildInvoiceItemQuery(i => i.InvoiceItem.InvoiceCancellation == null & i.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice);
                        plResult.Controls.Add(invoiceListView);
                        break;
                    case 1:
                        //allowanceListView = (InvoiceAllowanceQueryList)this.LoadControl("InvoiceAllowanceQueryList.ascx");
                        //allowanceListView.InitializeAsUserControl(this.Page);
                        //allowanceListView.QueryExpr = buildInvoiceAllowanceQuery(i => i.InvoiceAllowance.InvoiceAllowanceCancellation == null);
                        //plResult.Controls.Add(allowanceListView);
                        invoiceListView = (InvoiceItemQueryList)this.LoadControl("InvoiceItemQueryList.ascx");
                        invoiceListView.InitializeAsUserControl(this.Page);
                        invoiceListView.QueryExpr = buildInvoiceItemQuery(i => i.InvoiceAllowance.InvoiceAllowanceCancellation == null & i.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Allowance);
                        plResult.Controls.Add(invoiceListView);
                        break;
                    case 2:
                        invoiceListView = (InvoiceItemQueryList)this.LoadControl("InvoiceItemQueryList.ascx");
                        invoiceListView.InitializeAsUserControl(this.Page);
                        invoiceListView.QueryExpr = buildInvoiceItemQuery(i => i.InvoiceItem.InvoiceCancellation != null & i.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_InvoiceCancellation);
                        plResult.Controls.Add(invoiceListView);
                        break;
                    case 3:
                        //allowanceListView = (InvoiceAllowanceQueryList)this.LoadControl("InvoiceAllowanceQueryList.ascx");
                        //allowanceListView.InitializeAsUserControl(this.Page);
                        //allowanceListView.QueryExpr = buildInvoiceAllowanceQuery(i => i.InvoiceAllowance.InvoiceAllowanceCancellation != null);
                        //plResult.Controls.Add(allowanceListView);
                        invoiceListView = (InvoiceItemQueryList)this.LoadControl("InvoiceItemQueryList.ascx");
                        invoiceListView.InitializeAsUserControl(this.Page);
                        invoiceListView.QueryExpr = buildInvoiceItemQuery(i => i.InvoiceAllowance.InvoiceAllowanceCancellation != null & i.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_AllowanceCancellation);
                        plResult.Controls.Add(invoiceListView);
                        break;
                }
            }
        }
        
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.divResult.Visible = true;
            btnSearch.CommandArgument = "Query";
            initializeData();
        }
        

        #region "Search Data"

        protected virtual Expression<Func<CDS_Document, bool>> buildInvoiceItemQuery(Expression<Func<CDS_Document, bool>> queryExpr)
        {

            if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
            {
                queryExpr = queryExpr.And(d => d.InvoiceItem.InvoiceSeller.Organization.ReceiptNo.Equals(_userProfile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo) | d.InvoiceItem.InvoiceBuyer.Organization.ReceiptNo.Equals(_userProfile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo));
            }

            //if (SellerID.Selector.SelectedIndex > 0)
            //{
            //    queryExpr = queryExpr.And(d => d.CDS_Document.DocumentOwner.OwnerID == int.Parse(SellerID.Selector.SelectedValue));
            //}

            if (this.DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceDate < DateTo.DateTimeValue.AddDays(1));
            }

            if (this.ddlType.SelectedIndex != 0)
            {
                //todo
                //queryExpr = queryExpr.And(i => i.DocType == int.Parse(this.ddlType.SelectedValue));
                //queryExpr=queryExpr.And(i=>i.InvoiceItem.CheckBusinessType(
            }

            if (this.ddlStep.SelectedIndex != 0)
            {
                queryExpr = queryExpr.And(i => i.CurrentStep == int.Parse(this.ddlStep.SelectedValue));
            }

            return queryExpr;
        }

        protected virtual Expression<Func<CDS_Document, bool>> buildInvoiceAllowanceQuery(Expression<Func<CDS_Document, bool>> queryExpr)
        {
            //一般使用者僅能查詢屬於自己卡號的發票資訊,系統管理者則可以查詢全部
            //if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
            //{
            //    if (_userProfile.CurrentUserRole.RoleID == (int)Naming.RoleID.ROLE_SELLER || _userProfile.CurrentUserRole.RoleID == (int)Naming.RoleID.ROLE_GOOGLETW)
            //    {
            //        queryExpr = queryExpr.And(d => d.CDS_Document.DocumentOwner.OwnerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
            //    }
            //    else
            //    {
            //        queryExpr = queryExpr.And(i => i.InvoiceByHousehold.InvoiceUserCarrier.UID == _userProfile.UID);
            //    }
            //}

            //if (SellerID.Selector.SelectedIndex > 0)
            //{
            //    queryExpr = queryExpr.And(d => d.CDS_Document.DocumentOwner.OwnerID == int.Parse(SellerID.Selector.SelectedValue));
            //}

            if (this.DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceAllowance.AllowanceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceAllowance.AllowanceDate < DateTo.DateTimeValue.AddDays(1));
            }

            if (this.ddlStep.SelectedIndex != 0)
            {
                queryExpr = queryExpr.And(i => i.CurrentStep == int.Parse(this.ddlStep.SelectedValue));
            }
            return queryExpr;
        }

        #endregion
        
    }    
}