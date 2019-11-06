using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Utility;
using Model.Locale;

namespace eIVOGo.Module.EIVO.RelativeBuyer
{
    public partial class InquireInvoiceForReceiving : InquireEntity
    {
        public event EventHandler afterQuery;
        protected UserProfileMember _userProfile;
        protected int? _dataCount = 0;
        protected bool _doQuery = true;
        protected bool _isInvoice = true;

        public bool DoQuery
        {
            get
            {
                return _doQuery;
            }
            set
            {
                _doQuery = value;
            }
        }

        public bool isInvoice
        {
            get { return _isInvoice; }
            set { _isInvoice = value; }
        }

        public int? dataCount
        {
            get { return _dataCount; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //itemList.Done += new EventHandler(itemList_Done);
        }

        //void itemList_Done(object sender, EventArgs e)
        //{
        //    if (itemList._totalRecordCount > 0)
        //    {
        //        _dataCount = itemList._totalRecordCount;
        //        if (afterQuery != null)
        //        {
        //            afterQuery(this, new EventArgs());
        //        }
        //    }
        //}

        protected override UserControl _itemList
        {
            get { return itemList; }
        }

        protected override void buildQueryItem()
        {
            //Expression<Func<InvoiceItem, bool>> queryExpr = i => i.InvoiceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID;
            //Expression<Func<CDS_Document, bool>> queryExpr = i => i.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.待接收;
            Expression<Func<CDS_Document, bool>> queryExpr = i => true;

            switch (rbChange.SelectedIndex)
            {
                case 0:
                    queryExpr = queryExpr.And(i => i.CurrentStep == (int)Naming.InvoiceStepDefinition.待接收);
                    queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceCancellation == null & i.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Invoice & i.InvoiceItem.InvoiceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                    break;
                case 1:
                    queryExpr = queryExpr.And(i => i.CurrentStep == (int)Naming.InvoiceStepDefinition.待開立);
                    queryExpr = queryExpr.And(i => i.InvoiceAllowance.InvoiceAllowanceCancellation == null & i.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_Allowance & i.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                    _isInvoice = false;
                    break;
                case 2:
                    queryExpr = queryExpr.And(i => i.CurrentStep == (int)Naming.InvoiceStepDefinition.待接收);
                    queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceCancellation != null & i.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_InvoiceCancellation & i.InvoiceItem.InvoiceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                    break;
                case 3:
                    queryExpr = queryExpr.And(i => i.CurrentStep == (int)Naming.InvoiceStepDefinition.待開立);
                    queryExpr = queryExpr.And(i => i.InvoiceAllowance.InvoiceAllowanceCancellation != null & i.DocType == (int)Model.Locale.Naming.DocumentTypeDefinition.E_AllowanceCancellation & i.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                    _isInvoice = false;
                    break;
            }

            if (DateFrom.HasValue)
            {
                if (this.rbChange.SelectedIndex == 0 | this.rbChange.SelectedIndex == 2)
                    queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceDate >= DateFrom.DateTimeValue);
                else if (this.rbChange.SelectedIndex == 1 | this.rbChange.SelectedIndex == 3)
                    queryExpr = queryExpr.And(i => i.InvoiceAllowance.AllowanceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                if (this.rbChange.SelectedIndex == 0 | this.rbChange.SelectedIndex == 2)
                    queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceDate < DateTo.DateTimeValue.AddDays(1));
                else if (this.rbChange.SelectedIndex == 1 | this.rbChange.SelectedIndex == 3)
                    queryExpr = queryExpr.And(i => i.InvoiceAllowance.AllowanceDate < DateTo.DateTimeValue.AddDays(1));
            }
            if (!String.IsNullOrEmpty(MasterID.SelectedValue))
            {
                if (this.rbChange.SelectedIndex == 0 | this.rbChange.SelectedIndex == 2)
                queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceSeller.SellerID == int.Parse(MasterID.SelectedValue));
                else if (this.rbChange.SelectedIndex == 1 | this.rbChange.SelectedIndex == 3)
                    queryExpr = queryExpr.And(i => i.InvoiceAllowance.InvoiceAllowanceSeller.SellerID == int.Parse(MasterID.SelectedValue));
            }

            itemList.BuildQuery = table =>
            {
                //return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice && d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.待接收)
                //    .Join(table.Context.GetTable<InvoiceItem>().Where(queryExpr), d => d.DocID, i => i.InvoiceID, (d, i) => d);
                return table.Where(queryExpr);
            };

            base.buildQueryItem();

        }

        protected void rbChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            this._doQuery = false;
            resultTitle.Visible = false;
            itemList.Visible = false;
        }
    }
}