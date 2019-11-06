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
    public partial class InquireReceiptCancellationItem : InquireInvoiceItem
    {
        protected override void buildQueryItem()
        {
            Expression<Func<ReceiptCancellation, bool>> queryExpr = i => true;

            if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
            {
                queryExpr = queryExpr.And(d => d.ReceiptItem.Seller.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID || d.ReceiptItem.Buyer.CompanyID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
            }

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

            itemList.BuildQuery = table =>
            {
                var receiptcancellations = table.Context.GetTable<ReceiptCancellation>().OrderByDescending(i => i.ReceiptID).Where(queryExpr);

                if (!String.IsNullOrEmpty(CompanyID.SelectedValue))
                {
                    int companyID = int.Parse(CompanyID.SelectedValue);

                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            receiptcancellations = receiptcancellations.Where(i => i.ReceiptItem.SellerID == companyID);
                        }
                        else
                        {
                            receiptcancellations = receiptcancellations.Where(i => i.ReceiptItem.BuyerID == companyID);
                        }
                    }
                    else
                    {
                        receiptcancellations = receiptcancellations.Where(i => i.ReceiptItem.BuyerID == companyID || i.ReceiptItem.SellerID == companyID);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(BusinessID.SelectedValue))
                    {
                        if (Naming.InvoiceCenterBusinessType.進項 == (Naming.InvoiceCenterBusinessType)int.Parse(BusinessID.SelectedValue))
                        {
                            receiptcancellations = receiptcancellations.Where(i => i.ReceiptItem.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                        else
                        {
                            receiptcancellations = receiptcancellations.Where(i => i.ReceiptItem.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID);
                        }
                    }
                }

                if (!String.IsNullOrEmpty(LevelID.SelectedValue))
                {
                     if (!setdayrange)
                         return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_ReceiptCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(receiptcancellations.Where(i => i.ReceiptItem.ReceiptDate <= DateTime.Today & i.ReceiptItem.ReceiptDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.ReceiptID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_ReceiptCancellation && d.CurrentStep == int.Parse(LevelID.SelectedValue))
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(receiptcancellations, d => d.SourceID, i => i.ReceiptID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                }
                else
                {
                     if (!setdayrange)
                         return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_ReceiptCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(receiptcancellations.Where(i => i.ReceiptItem.ReceiptDate <= DateTime.Today & i.ReceiptItem.ReceiptDate >= DateTime.Today.AddMonths(-5)), d => d.SourceID, i => i.ReceiptID, (d, i) => d)
                        , d => d.DocID, r => r.DocID, (d, r) => d);
                    else
                    return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_ReceiptCancellation)
                        .Join(table.Context.GetTable<DerivedDocument>()
                            .Join(receiptcancellations, d => d.SourceID, i => i.ReceiptID, (d, i) => d)
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
