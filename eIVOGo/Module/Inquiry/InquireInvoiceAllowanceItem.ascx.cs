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
    public partial class InquireInvoiceAllowanceItem : InquireInvoiceItem
    {

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceAllowance, bool>> queryExpr = i => i.InvoiceAllowanceCancellation == null;

            if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
            {
                queryExpr = queryExpr.And(d => d.InvoiceAllowanceSeller.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID || d.InvoiceAllowanceBuyer.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
            }

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.AllowanceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.AllowanceDate < DateTo.DateTimeValue.AddDays(1));
            }

            if (!string.IsNullOrEmpty(this.txtAllowanceNO.Text.Trim()))
            {
                queryExpr = queryExpr.And(i => i.AllowanceNumber.Equals(this.txtAllowanceNO.Text.Trim()));
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
            if (!String.IsNullOrEmpty(this.EntrustToPrint.SelectedValue))
            {
                if (this.EntrustToPrint.SelectedValue.Equals("1"))
                {
                    queryExpr = queryExpr.And(i => (bool)i.InvoiceAllowanceBuyer.Organization.OrganizationStatus.EntrustToPrint);
                }
                else
                {
                    queryExpr = queryExpr.And(i => !(bool)i.InvoiceAllowanceBuyer.Organization.OrganizationStatus.EntrustToPrint || !i.InvoiceAllowanceBuyer.Organization.OrganizationStatus.EntrustToPrint.HasValue);
                }
            }
            itemList.BuildQuery = table =>
            {
                var allowance = table.Context.GetTable<InvoiceAllowance>().OrderByDescending(ia=>ia.AllowanceID).Where(queryExpr);

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
                        return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Allowance && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(allowance.Where(i => i.AllowanceDate <= DateTime.Today & i.AllowanceDate >= DateTime.Today.AddMonths(-5)), d => d.DocID, i => i.AllowanceID, (d, i) => d).OrderByDescending(d => d.DocID);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Allowance && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(allowance, d => d.DocID, i => i.AllowanceID, (d, i) => d).OrderByDescending(d=>d.DocID);
                }
                else
                {
                     if (!setdayrange)
                          return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Allowance)
                        .Join(allowance.Where(i => i.AllowanceDate <= DateTime.Today & i.AllowanceDate >= DateTime.Today.AddMonths(-5)), d => d.DocID, i => i.AllowanceID, (d, i) => d).OrderByDescending(d => d.DocID);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Allowance)
                        .Join(allowance, d => d.DocID, i => i.AllowanceID, (d, i) => d).OrderByDescending(d => d.DocID);
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
