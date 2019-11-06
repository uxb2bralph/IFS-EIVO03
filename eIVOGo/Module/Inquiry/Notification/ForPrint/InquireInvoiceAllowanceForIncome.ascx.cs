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
    public partial class InquireInvoiceAllowanceForIncome : InquireInvoiceForReceiving
    {

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceAllowance, bool>> queryExpr = i => i.InvoiceAllowanceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID && i.InvoiceAllowanceCancellation == null;

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.AllowanceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.AllowanceDate < DateTo.DateTimeValue.AddDays(1));
            }
            if (!String.IsNullOrEmpty(MasterID.SelectedValue))
            {
                queryExpr = queryExpr.And(i => i.InvoiceAllowanceSeller.SellerID == int.Parse(MasterID.SelectedValue));
            }
            if (!String.IsNullOrEmpty(this.ddPrint.SelectedValue))
            {
                if (this.ddPrint.SelectedValue.Equals("1"))
                {
                    queryExpr = queryExpr.And(i => i.CDS_Document.DocumentPrintLog.Any());
                }
                else
                {
                    queryExpr = queryExpr.And(i => !i.CDS_Document.DocumentPrintLog.Any());
                }
            }

            itemList.BuildQuery = table =>
            {
                return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Allowance && (d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.已接收))
                    .Join(table.Context.GetTable<InvoiceAllowance>().Where(queryExpr), d => d.DocID, i => i.AllowanceID, (d, i) => d).OrderByDescending(d => d.DocID);
            };

            if (itemList.Select().Count() > 0)
            {
                OnDone(null);
            }

        }
    }
}