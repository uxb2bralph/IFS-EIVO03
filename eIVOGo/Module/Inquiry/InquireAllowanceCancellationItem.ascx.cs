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
    public partial class InquireAllowanceCancellationItem : InquireInvoiceItem
    {

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceAllowanceCancellation, bool>> queryExpr = i => true;

            if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
            {
                queryExpr = queryExpr.And(d => d.InvoiceAllowance.InvoiceAllowanceSeller.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID || d.InvoiceAllowance.InvoiceAllowanceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
            }

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceAllowance.AllowanceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceAllowance.AllowanceDate < DateTo.DateTimeValue.AddDays(1));
            }

            if (!string.IsNullOrEmpty(this.txtAllowanceCancelNO.Text.Trim()))
            {
                queryExpr = queryExpr.And(i => i.InvoiceAllowance.AllowanceNumber.Equals(this.txtAllowanceCancelNO.Text.Trim()));
            }

            itemList.BuildQuery = table =>
            {
                var allowance = table.Context.GetTable<InvoiceAllowance>()
                    .Join(table.Context.GetTable<InvoiceAllowanceCancellation>().OrderByDescending(ac => ac.CancelDate).Where(queryExpr), a => a.AllowanceID, c => c.AllowanceID, (a, c) => a);

                if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
                {
                    int companyID = int.Parse(CompanyID.SelectedValue);

                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            allowance = allowance.Join(table.Context.GetTable<InvoiceAllowanceSeller>().Where(s => s.SellerID == companyID)
                                , i => i.AllowanceID, s => s.AllowanceID, (i, s) => i);
                        }
                        else
                        {
                            allowance = allowance.Join(table.Context.GetTable<InvoiceAllowanceBuyer>().Where(b => b.BuyerID == companyID)
                                , i => i.AllowanceID, s => s.AllowanceID, (i, s) => i);
                        }
                    }
                    else
                    {
                        allowance = allowance.Join(table.Context.GetTable<InvoiceAllowanceSeller>().Where(i => i.SellerID == companyID), i => i.AllowanceID, s => s.AllowanceID, (i, s) => i)
                            .Concat(allowance.Join(table.Context.GetTable<InvoiceAllowanceBuyer>().Where(i => i.BuyerID == companyID), i => i.AllowanceID, s => s.AllowanceID, (i, s) => i));
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            allowance = allowance.Where(i => i.InvoiceAllowanceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                        else
                        {
                            allowance = allowance.Where(i => i.InvoiceAllowanceSeller.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(LevelID.SelectedValue))
                {
                    if (!setdayrange)
                        return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(allowance.Where(i => i.InvoiceAllowanceCancellation.CancelDate <= DateTime.Today & i.InvoiceAllowanceCancellation.CancelDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.AllowanceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(allowance, d => d.SourceID, i => i.AllowanceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                }
                else
                {
                    if (!setdayrange)
                        return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(allowance.Where(i => i.InvoiceAllowanceCancellation.CancelDate <= DateTime.Today & i.InvoiceAllowanceCancellation.CancelDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.AllowanceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(allowance, d => d.SourceID, i => i.AllowanceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                }
            };

            if (itemList.Select().Count() > 0)
            {
                tblAction.Visible = true;
            }
            else
            {
                tblAction.Visible = false;
            }
        }
    }    
}
