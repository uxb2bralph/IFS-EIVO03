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

namespace eIVOGo.Module.Inquiry
{
    public partial class InquireAllowanceCancellationForReceiving : InquireInvoiceForReceiving
    {

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceAllowance, bool>> queryExpr = i => i.InvoiceAllowanceSeller.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID;

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
                queryExpr = queryExpr.And(i => i.InvoiceAllowanceBuyer.BuyerID == int.Parse(MasterID.SelectedValue));
            }

            itemList.BuildQuery = table =>
            {
                return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation && d.CurrentStep == (int)Naming.InvoiceStepDefinition.待接收)
                    .Join(table.Context.GetTable<DerivedDocument>()
                        .Join(table.Context.GetTable<InvoiceAllowance>().Where(queryExpr), d => d.SourceID, i => i.AllowanceID, (d, i) => d),
                    d => d.DocID, v => v.DocID, (d, v) => d).OrderByDescending(d => d.DocID);
            };

            if (itemList.Select().Count() > 0)
            {
                OnDone(null);
            }

        }
    }
}