﻿using System;
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

namespace eIVOGo.Module.Inquiry
{
    public partial class InquireReceiptForReceiving : InquireInvoiceForReceiving
    {

        protected override void buildQueryItem()
        {
            Expression<Func<ReceiptItem, bool>> queryExpr = i => i.BuyerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID && i.ReceiptCancellation == null;

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

            itemList.BuildQuery = table =>
            {
                return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Receipt && d.CurrentStep == (int)Naming.InvoiceStepDefinition.待接收)
                    .Join(table.Context.GetTable<ReceiptItem>().Where(queryExpr), d => d.DocID, i => i.ReceiptID, (d, i) => d).OrderByDescending(d => d.DocID);
            };

            if (itemList.Select().Count() > 0)
            {
                OnDone(null);
            }

        }
    }
}