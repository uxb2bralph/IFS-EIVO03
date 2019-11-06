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
    public partial class InquireInvoiceCancellationItem : InquireInvoiceItem
    {

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceCancellation, bool>> queryExpr = i => true;

            if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
            {
                queryExpr = queryExpr.And(d => d.InvoiceItem.InvoiceSeller.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID || d.InvoiceItem.InvoiceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
            }

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceDate < DateTo.DateTimeValue.AddDays(1));
            }

            if (!string.IsNullOrEmpty(this.txtCancellationNO.Text.Trim()))
            {
                queryExpr = queryExpr.And(i => (i.CancellationNo.Trim()).Equals(this.txtCancellationNO.Text.Trim()));
            }

            itemList.BuildQuery = table =>
            {
                var invoices = table.Context.GetTable<InvoiceItem>()
                    .Join(table.Context.GetTable<InvoiceCancellation>().OrderByDescending(ic => ic.CancelDate).Where(queryExpr), i => i.InvoiceID, c => c.InvoiceID, (i, c) => i);

                if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
                {
                    int companyID = int.Parse(CompanyID.SelectedValue);

                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>().Where(s => s.SellerID == companyID)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                        }
                        else
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>().Where(b => b.BuyerID == companyID)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                        }
                    }
                    else
                    {
                        invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>().Where(i => i.SellerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i)
                            .Concat(invoices.Join(table.Context.GetTable<InvoiceBuyer>().Where(i => i.BuyerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i));
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            invoices = invoices.Where(i => i.InvoiceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID); ;
                        }
                        else
                        {
                            invoices = invoices.Where(i => i.InvoiceSeller.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(LevelID.SelectedValue))
                {
                    if (!setdayrange)
                        return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(invoices.Where(i => i.InvoiceDate <= DateTime.Today & i.InvoiceDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.InvoiceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(invoices, d => d.SourceID, i => i.InvoiceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                }
                else
                {
                    if (!setdayrange)
                        return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(invoices.Where(i => i.InvoiceDate <= DateTime.Today & i.InvoiceDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.InvoiceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(invoices, d => d.SourceID, i => i.InvoiceID, (d, i) => d)
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
