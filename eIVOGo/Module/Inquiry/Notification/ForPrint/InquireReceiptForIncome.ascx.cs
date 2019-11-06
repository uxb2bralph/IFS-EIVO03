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

namespace eIVOGo.Module.Inquiry.ForPrint
{
    public partial class InquireReceiptForIncome : InquireReceiptForReceiving
    {

        protected override void buildQueryItem()
        {
            Expression<Func<ReceiptItem, bool>> queryExpr = i => i.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID & i.ReceiptCancellation == null;

            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.ReceiptDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.ReceiptDate < DateTo.DateTimeValue.AddDays(1));
            }
            if (!String.IsNullOrEmpty(MasterID.SelectedValue))
            {
                queryExpr = queryExpr.And(i => i.SellerID == int.Parse(MasterID.SelectedValue));
            }

            //若為收據列印功能,則查尋條件要根據"列印狀態"下拉選單的選項
            switch (this.ddPrint.SelectedValue)
            {
                case "全部":
                    break;
                case "已列印":
                    queryExpr = queryExpr.And(i => i.CDS_Document.DocumentPrintLog.Any());
                    break;
                case "未列印":
                    queryExpr = queryExpr.And(i => !i.CDS_Document.DocumentPrintLog.Any());
                    break;
                default:
                    break;
            }

            itemList.BuildQuery = table =>
            {
                return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Receipt && d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.已接收)
                    .Join(table.Context.GetTable<ReceiptItem>().Where(queryExpr), d => d.DocID, i => i.ReceiptID, (d, i) => d).OrderByDescending(d => d.DocID);
            };

            if (itemList.Select().Count() > 0)
            {
                OnDone(null);
            }

        }
    }
}