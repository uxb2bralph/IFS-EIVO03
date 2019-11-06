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
    public partial class InquireInvoiceCancellation : InquireInvoiceItem
    {

        protected override void buildQueryItem()
        {
            Expression<Func<InvoiceCancellation, bool>> queryExpr = i => true;

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.CancelDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.CancelDate < DateTo.DateTimeValue.AddDays(1));
            }

            String cancelNo = this.txtCancellationNO.Text.Trim();
            if (!string.IsNullOrEmpty(cancelNo))
            {
                queryExpr = queryExpr.And(i => i.CancellationNo == cancelNo);
            }

            if (!String.IsNullOrEmpty(this.txtReceiptNo.Text))
            {
                queryExpr = queryExpr.And(i => i.InvoiceItem.InvoiceBuyer.ReceiptNo.Equals(this.txtReceiptNo.Text.Trim()));
            }

            if (!String.IsNullOrEmpty(this.ddlAttach.SelectedValue))
            {
                if (this.ddlAttach.SelectedValue.Equals("0"))
                {
                    queryExpr = queryExpr.And(i => i.InvoiceItem.CDS_Document.Attachment.Count() > 0);
                }
                else
                {
                    queryExpr = queryExpr.And(i => i.InvoiceItem.CDS_Document.Attachment.Count() <= 0);
                }
            }

            itemList.BuildQuery = table =>
            {
                var invoices = table.Context.GetTable<InvoiceItem>()
                    .Join(table.Context.GetTable<InvoiceCancellation>().OrderByDescending(ic=>ic.CancelDate).Where(queryExpr), i => i.InvoiceID, c => c.InvoiceID, (i, c) => i);

                if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
                {
                    int companyID = int.Parse(CompanyID.SelectedValue);

                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                            {
                                invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                    .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID && r.BusinessMaster.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue))
                                    .Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                    , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            }
                            else
                            {
                                invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                    .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID)
                                    .Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                    , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            }
                        }
                        else
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID).Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                                invoices = invoices.Where(i => i.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue));
                       
                        }
                    }
                    else
                    {
                        invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>().Where(i => i.SellerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i)
                            .Concat(invoices.Join(table.Context.GetTable<InvoiceBuyer>().Where(i => i.BuyerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i));
                        if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                            invoices = invoices.Where(i => i.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue) ||
                                i.InvoiceBuyer.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue));
                    //不嚴謹需修改
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                            {
                                invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                         .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.BusinessMaster.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue))
                                         .Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                         , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            }
                            else
                            {
                                invoices = invoices.Join(table.Context.GetTable<InvoiceBuyer>()
                                    .Join(table.Context.GetTable<BusinessRelationship>()
                                    .Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                                    , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            }
                        }
                        else
                        {
                            invoices = invoices.Join(table.Context.GetTable<InvoiceSeller>()
                                .Join(table.Context.GetTable<BusinessRelationship>().Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
                                , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                            if (!String.IsNullOrEmpty(EnterpriseID.SelectedValue))
                                invoices = invoices.Where(i => i.Organization.EnterpriseGroupMember.FirstOrDefault().EnterpriseID.Equals(EnterpriseID.SelectedValue));

                        }
                    }
                }

                if (!String.IsNullOrEmpty(LevelID.SelectedValue))
                {
                    if (!setdayrange)
                         return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(invoices.Where(i => i.InvoiceDate <= DateTime.Today & i.InvoiceDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.InvoiceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d).OrderByDescending(d => d.DocID);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(invoices, d => d.SourceID, i => i.InvoiceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d).OrderByDescending(d => d.DocID);
                }
                else
                {
                     if (!setdayrange)
                         return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(invoices.Where(i => i.InvoiceDate <= DateTime.Today & i.InvoiceDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.InvoiceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d).OrderByDescending(d => d.DocID);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_InvoiceCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(invoices, d => d.SourceID, i => i.InvoiceID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d).OrderByDescending(d => d.DocID);
                }
            };

            if (itemList.Select().Count() > 0)
            {
                tblAction.Visible = true;
            }

        }
    }    
}
