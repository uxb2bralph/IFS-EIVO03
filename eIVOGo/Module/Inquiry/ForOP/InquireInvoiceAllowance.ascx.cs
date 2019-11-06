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

namespace eIVOGo.Module.Inquiry.ForOP
{
    public partial class InquireInvoiceAllowance : InquireInvoiceItem
    {
        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceAllowance, bool>> queryExpr = i => i.InvoiceAllowanceCancellation == null;

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.AllowanceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.AllowanceDate < DateTo.DateTimeValue.AddDays(1));
            }

            String no = this.txtAllowanceNO.Text.Trim();
            if (!string.IsNullOrEmpty(no))
            {
                queryExpr = queryExpr.And(i => i.AllowanceNumber == no);
            }

            if (!String.IsNullOrEmpty(this.txtReceiptNo.Text))
            {
                queryExpr = queryExpr.And(i => i.InvoiceAllowanceBuyer.ReceiptNo.Equals(this.txtReceiptNo.Text.Trim()));
            }

            if (!String.IsNullOrEmpty(this.ddlAttach.SelectedValue))
            {
                if (this.ddlAttach.SelectedValue.Equals("0"))
                {
                    queryExpr = queryExpr.And(i => i.CDS_Document.Attachment.Count() > 0);
                }
                else
                {
                    queryExpr = queryExpr.And(i => i.CDS_Document.Attachment.Count() <= 0);
                }
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
                var allowance = table.Context.GetTable<InvoiceAllowance>().OrderByDescending(a => a.AllowanceID).Where(queryExpr);

                if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
                {
                    int companyID = int.Parse(CompanyID.SelectedValue);

                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            allowance = allowance.Join(table.Context.GetTable<InvoiceAllowanceBuyer>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID).Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                , i => i.AllowanceID, s => s.AllowanceID, (i, s) => i);
                        }
                        else
                        {
                            allowance = allowance.Join(table.Context.GetTable<InvoiceAllowanceSeller>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID).Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
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
                            allowance = allowance.Join(table.Context.GetTable<InvoiceAllowanceBuyer>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                , i => i.AllowanceID, s => s.AllowanceID, (i, s) => i);
                        }
                        else
                        {
                            allowance = allowance.Join(table.Context.GetTable<InvoiceAllowanceSeller>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
                                , i => i.AllowanceID, s => s.AllowanceID, (i, s) => i);
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
                        .Join(allowance, d => d.DocID, i => i.AllowanceID, (d, i) => d).OrderByDescending(d => d.DocID); 
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

        }
    }    
}
