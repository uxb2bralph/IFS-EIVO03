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
using Uxnet.Web.WebUI;
namespace eIVOGo.Module.Inquiry.ForPrint
{
    public partial class InquireInvoiceItemForSale : InquireInvoiceForReceiving
    {

        protected override void buildQueryItem()
        {
            int _startNo = 0, _endNo = 0;
            string _startNoTC = string.Empty, _endNoTC = string.Empty;

            if (StartNo.Text != "")
            {
                _startNoTC = StartNo.Text.Substring(0, 2);
                _startNo = int.Parse(StartNo.Text.Substring(2));
                if (StartNo.Text.Trim().Length != 10)
                {
                    this.AjaxAlert("發票起始號碼長度非10碼!!");
                    return;
                }
            }
            if (EndNo.Text != "")
            {
                _endNoTC = EndNo.Text.Substring(0, 2);
                _endNo = int.Parse(EndNo.Text.Substring(2));
                if (EndNo.Text.Trim().Length != 10)
                {
                    this.AjaxAlert("迄號長度非10碼!!");
                    return;
                }
            }
            Expression<Func<InvoiceItem, bool>> queryExpr = i => i.SellerID == _userProfile.CurrentUserRole.OrganizationCategory.CompanyID && i.InvoiceCancellation == null;


            if (DateFrom.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate >= DateFrom.DateTimeValue);
            }
            if (DateTo.HasValue)
            {
                queryExpr = queryExpr.And(i => i.InvoiceDate < DateTo.DateTimeValue.AddDays(1));
            }
            if (!String.IsNullOrEmpty(MasterID.SelectedValue))
            {
                queryExpr = queryExpr.And(i => i.InvoiceBuyer.BuyerID == int.Parse(MasterID.SelectedValue));
            }
            if (!String.IsNullOrEmpty(this.EntrustToPrint.SelectedValue))
            {
                if (this.EntrustToPrint.SelectedValue.Equals("1"))
                {
                    queryExpr = queryExpr.And(i => (bool)i.InvoiceBuyer.Organization.OrganizationStatus.EntrustToPrint);
                }
                else
                {
                    queryExpr = queryExpr.And(i => !(bool)i.InvoiceBuyer.Organization.OrganizationStatus.EntrustToPrint ||! i.InvoiceBuyer.Organization.OrganizationStatus.EntrustToPrint.HasValue);
                }
            }

            if (!string.IsNullOrEmpty(_startNoTC))
                queryExpr = queryExpr.And(i => i.TrackCode.Equals(_startNoTC));
            if (!string.IsNullOrEmpty(_endNoTC))
                queryExpr = queryExpr.And(i => i.TrackCode.Equals(_endNoTC));
            if (_startNo > 0)
                 queryExpr = queryExpr.And(i=>Convert.ToInt32(i.No)>=_startNo);
              
            if (_endNo > 0)
                 queryExpr = queryExpr.And(i=>Convert.ToInt32(i.No)<= _endNo);

            itemList.BuildQuery = table =>
            {
                return table.Where(d => d.DocType == (int)Naming.DocumentTypeDefinition.E_Invoice && (d.CurrentStep == (int)Naming.B2BInvoiceStepDefinition.已接收))
                    .Join(table.Context.GetTable<InvoiceItem>().Where(queryExpr), d => d.DocID, i => i.InvoiceID, (d, i) => d).OrderByDescending(d => d.DocID);
            };

            if (itemList.Select().Count() > 0)
            {
                OnDone(null);
            }

        }
    }
}