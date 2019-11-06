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
    public partial class InquireReceiptItem : InquireInvoiceItem
    {
        protected override void buildQueryItem()
        {
            Expression<Func<ReceiptItem, bool>> queryExpr = i => i.ReceiptCancellation == null;

            if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
            {
                queryExpr = queryExpr.And(d => d.Seller.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID || d.Buyer.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
            }

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.ReceiptDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.ReceiptDate < DateTo.DateTimeValue.AddDays(1));
            }

            if (!string.IsNullOrEmpty(this.txtInvoiceNO.Text.Trim()))
            {
                queryExpr = queryExpr.And(i => i.No.Trim().Equals(this.txtInvoiceNO.Text.Trim()));
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
                    queryExpr = queryExpr.And(i => (bool)i.Buyer.OrganizationStatus.EntrustToPrint);
                }
                else
                {
                    queryExpr = queryExpr.And(i => !(bool)i.Buyer.OrganizationStatus.EntrustToPrint || !i.Buyer.OrganizationStatus.EntrustToPrint.HasValue);
                }
            }
            itemList.BuildQuery = table =>
            {
                var receipts = table.Context.GetTable<ReceiptItem>().OrderByDescending(i => i.ReceiptID).Where(queryExpr);

                if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
                {
                    int companyID = int.Parse(CompanyID.SelectedValue);

                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            receipts = receipts.Where(i => i.SellerID == companyID);
                            //receipts = receipts.Join(table.Context.GetTable<InvoiceBuyer>()
                            //    .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID).Select(r => r.MasterID).Distinct(), b => b.BuyerID, r => r, (b, r) => b)
                            //    , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                        }
                        else
                        {
                            receipts = receipts.Where(i => i.BuyerID == companyID);
                            //receipts = receipts.Join(table.Context.GetTable<InvoiceSeller>()
                            //    .Join(table.Context.GetTable<BusinessRelationship>().Where(r => r.MasterID == companyID).Select(r => r.MasterID).Distinct(), b => b.SellerID, r => r, (b, r) => b)
                            //    , i => i.InvoiceID, s => s.InvoiceID, (i, s) => i);
                        }
                    }
                    else
                    {
                        //receipts = receipts.Join(table.Context.GetTable<InvoiceSeller>().Where(i => i.SellerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i)
                        //    .Concat(receipts.Join(table.Context.GetTable<InvoiceBuyer>().Where(i => i.BuyerID == companyID), i => i.InvoiceID, s => s.InvoiceID, (i, s) => i));
                        receipts = receipts.Where(i => i.BuyerID == companyID || i.SellerID == companyID);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            receipts = receipts.Where(i => i.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                        else
                        {
                            receipts = receipts.Where(i => i.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(LevelID.SelectedValue))
                {
                    if (!setdayrange)
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Receipt && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(receipts.Where(i => i.ReceiptDate <= DateTime.Today & i.ReceiptDate >= DateTime.Today.AddMonths(-5)), d => d.DocID, i => i.ReceiptID, (d, i) => d).OrderByDescending(d => d.DocID);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Receipt && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(receipts, d => d.DocID, i => i.ReceiptID, (d, i) => d).OrderByDescending(d=>d.DocID);
                }
                else
                {
                     if (!setdayrange)
                         return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Receipt)
                        .Join(receipts.Where(i => i.ReceiptDate <= DateTime.Today & i.ReceiptDate >= DateTime.Today.AddMonths(-5)), d => d.DocID, i => i.ReceiptID, (d, i) => d).OrderByDescending(d => d.DocID);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Receipt)
                        .Join(receipts, d => d.DocID, i => i.ReceiptID, (d, i) => d).OrderByDescending(d => d.DocID);
                }
            };

            if (itemList.Select().Count() > 0)
            {
                tblAction.Visible = true;
            }

        }

    }    
}
