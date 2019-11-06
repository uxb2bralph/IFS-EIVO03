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
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.Module.Common;

namespace eIVOGo.Module.Inquiry
{
    public partial class InquireInvoiceItemForBuyer : InquireEntity
    {
        protected UserProfileMember _userProfile;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
            btnPrint.PrintControls.Add(itemList);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnPrint.BeforeClick += new EventHandler(btnPrint_BeforeClick);
        }

        void btnPrint_BeforeClick(object sender, EventArgs e)
        {
            itemList.AllowPaging = false;
            buildQueryItem();
        }

        protected override UserControl _itemList
        {
            get { return itemList; }
        }

        protected override void btnQuery_Click(object sender, EventArgs e)
        {
            base.btnQuery_Click(sender, e);
            tblAction.Visible = true;
        }

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceItem, bool>> queryExpr = i => i.InvoiceCancellation == null;

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate < DateTo.DateTimeValue.AddDays(1));
            }

            itemList.BuildQuery = table =>
            {
                var invoices = table.Context.GetTable<InvoiceItem>().Where(queryExpr)
                    .Join(table.Context.GetTable<InvoiceBuyer>().Where(i => i.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);

                if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
                {
                    int companyID = int.Parse(CompanyID.SelectedValue);
                    invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>().Where(i => i.SellerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                }

                return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice && (d.CurrentStep == (int)Naming.InvoiceStepDefinition.已傳送 || d.CurrentStep == (int)Naming.InvoiceStepDefinition.已接收))
                    .Join(invoices, d => d.DocID, i => i.InvoiceID, (d, i) => d);
            };

            base.buildQueryItem();

        }

        protected void rbChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            Server.Transfer(rbChange.SelectedValue);
        }

    }    
}
