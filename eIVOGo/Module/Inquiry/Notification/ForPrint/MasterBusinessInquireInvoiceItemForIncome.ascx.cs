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

namespace eIVOGo.Module.Inquiry.ForPrint
{
    public partial class MasterBusinessInquireInvoiceItemForIncome : InquireInvoiceForReceiving
    {

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceItem, bool>> queryExpr = i => i.InvoiceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID && i.InvoiceCancellation == null
                && i.Organization.EnterpriseGroupMember.Count > 0;

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate < DateTo.DateTimeValue.AddDays(1));
            }
            if (!String.IsNullOrEmpty(MasterID.SelectedValue))
            {
                queryExpr = queryExpr.And(i => i.SellerID == int.Parse(MasterID.SelectedValue));
            }

            itemList.BuildQuery = table =>
            {
                return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice && (d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.已開立 || d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.已接收))
                    .Join(table.Context.GetTable<InvoiceItem>().Where(queryExpr), d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(d => d.DocID);
            };

            if (itemList.Select().Count() > 0)
            {
                OnDone(null);
            }

        }
    }
}