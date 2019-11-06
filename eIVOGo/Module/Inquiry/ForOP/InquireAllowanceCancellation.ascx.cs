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
    public partial class InquireAllowanceCancellation : InquireInvoiceItem
    {
        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceAllowanceCancellation, bool>> queryExpr = i => true;

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.CancelDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.CancelDate < DateTo.DateTimeValue.AddDays(1));
            }

            String cancelNo = this.txtAllowanceCancelNO.Text.Trim();
            if (!string.IsNullOrEmpty(cancelNo))
            {
                queryExpr = queryExpr.And(i => i.InvoiceAllowance.AllowanceNumber == cancelNo);
            }

            if (!String.IsNullOrEmpty(this.txtReceiptNo.Text))
            {
                queryExpr = queryExpr.And(i => i.InvoiceAllowance.InvoiceAllowanceBuyer.ReceiptNo.Equals(this.txtReceiptNo.Text.Trim()));
            }

            if (!String.IsNullOrEmpty(this.ddlAttach.SelectedValue))
            {
                if (this.ddlAttach.SelectedValue.Equals("0"))
                {
                    queryExpr = queryExpr.And(i => i.InvoiceAllowance.CDS_Document.Attachment.Count() > 0);
                }
                else
                {
                    queryExpr = queryExpr.And(i => i.InvoiceAllowance.CDS_Document.Attachment.Count() <= 0);
                }
            }

            itemList.BuildQuery = table =>
            {
                var allowance = table.Context.GetTable<InvoiceAllowance>()
                    .Join(table.Context.GetTable<InvoiceAllowanceCancellation>().OrderByDescending(ac=>ac.CancelDate).Where(queryExpr), a => a.AllowanceID, c => c.AllowanceID, (a, c) => a);

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
                        return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(allowance.Where(i => i.InvoiceAllowanceCancellation.CancelDate <= DateTime.Today & i.InvoiceAllowanceCancellation.CancelDate >= DateTime.Today.AddMonths(-5))
                            , d => d.SourceID, i => i.AllowanceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d).OrderByDescending(d => d.DocID);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(allowance, d => d.SourceID, i => i.AllowanceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d).OrderByDescending(d => d.DocID); ;
                }
                else
                {
                    if (!setdayrange)
                        return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(allowance.Where(i => i.InvoiceAllowanceCancellation.CancelDate <= DateTime.Today & i.InvoiceAllowanceCancellation.CancelDate >= DateTime.Today.AddMonths(-5))
                            , d => d.SourceID, i => i.AllowanceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d).OrderByDescending(d => d.DocID); 
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_AllowanceCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(allowance, d => d.SourceID, i => i.AllowanceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d).OrderByDescending(d => d.DocID); ;
                }
            };

            if (itemList.Select().Count() > 0)
            {
                tblAction.Visible = true;
            }

        }
    }    
}
