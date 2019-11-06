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
    public partial class InquireReceiptCancellationItem : InquireInvoiceItem
    {
        protected override void buildQueryItem()
        {
            Expression<Func<ReceiptCancellation, bool>> queryExpr = i => true;

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.ReceiptItem.ReceiptDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.ReceiptItem.ReceiptDate < DateTo.DateTimeValue.AddDays(1));
            }

            if (!string.IsNullOrEmpty(this.txtInvoiceNO.Text.Trim()))
            {
                queryExpr = queryExpr.And(i => i.ReceiptItem.No.Trim().Equals(this.txtInvoiceNO.Text.Trim()));
            }

            if (!String.IsNullOrEmpty(this.txtReceiptNo.Text))
            {
                queryExpr = queryExpr.And(i => i.ReceiptItem.Buyer.ReceiptNo.Equals(this.txtReceiptNo.Text.Trim()));
            }

            if (!String.IsNullOrEmpty(this.ddlAttach.SelectedValue))
            {
                if (this.ddlAttach.SelectedValue.Equals("0"))
                {
                    queryExpr = queryExpr.And(i => i.ReceiptItem.CDS_Document.Attachment.Count() > 0);
                }
                else
                {
                    queryExpr = queryExpr.And(i => i.ReceiptItem.CDS_Document.Attachment.Count() <= 0);
                }
            }

            itemList.BuildQuery = table =>
            {
                var receipts = table.Context.GetTable<ReceiptCancellation>().OrderByDescending(i => i.ReceiptID).Where(queryExpr);

                if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
                {
                    int companyID = int.Parse(CompanyID.SelectedValue);

                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            receipts = receipts.Where(i => i.ReceiptItem.Buyer.CompanyID == companyID);
                        }
                        else
                        {
                            receipts = receipts.Where(i => i.ReceiptItem.Seller.CompanyID == companyID);
                        }
                    }
                    else
                    {
                        receipts = receipts.Where(i => i.ReceiptItem.Buyer.CompanyID == companyID | i.ReceiptItem.Seller.CompanyID == companyID);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            receipts = receipts.Where(i => i.ReceiptItem.Buyer.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                        else
                        {
                            receipts = receipts.Where(i => i.ReceiptItem.Seller.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(LevelID.SelectedValue))
                {
                     if (!setdayrange)
                     return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_ReceiptCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(receipts.Where(i => i.ReceiptItem.ReceiptDate <= DateTime.Today & i.ReceiptItem.ReceiptDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.ReceiptID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_ReceiptCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(receipts, d => d.SourceID, i => i.ReceiptID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                }
                else
                {
                     if (!setdayrange)
                         return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_ReceiptCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(receipts.Where(i => i.ReceiptItem.ReceiptDate <= DateTime.Today & i.ReceiptItem.ReceiptDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.ReceiptID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_ReceiptCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(receipts, d => d.SourceID, i => i.ReceiptID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                }
            };

            if (itemList.Select().Count() > 0)
            {
                tblAction.Visible = true;
            }

        }

    }    
}
