using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;

namespace eIVOGo.Module.EIVO.Action
{
    public partial class IssueInvoice : ReceiveInvoice
    {
        protected override void signContext_BeforeSign(object sender, EventArgs e)
        {
            if (_docID != null && _docID.Count() > 0)
            {
                var invoices = dsEntity.CreateDataManager().EntityList.Where(i => _docID.Contains(i.DocID)).Select(i => i.InvoiceItem);

                StringBuilder sb = new StringBuilder("您欲開立的發票資料如下\r\n");
                sb.Append("營業人登入帳號:").Append(_userProfile.PID).Append("\r\n");
                sb.Append("營業人名稱:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.CompanyName).Append("\r\n");
                sb.Append("營業人統編:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo).Append("\r\n");
                sb.Append("接收時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                sb.Append("發票號碼\t\t發票日期\t\t接收發票營業人\r\n");

                foreach (var invoice in invoices)
                {
                    sb.Append(invoice.TrackCode).Append(invoice.No).Append("\t")
                        .Append(ValueValidity.ConvertChineseDateString(invoice.InvoiceDate.Value)).Append("\t")
                        .Append(invoice.InvoiceBuyer.CustomerName).Append("\r\n");
                }

                signContext.DataToSign = sb.ToString();
            }
        }


        protected override void doJob()
        {
            var mgr = dsEntity.CreateDataManager();
            var items = mgr.EntityList.Where(i => _docID.Contains(i.DocID));
            foreach (var item in items)
            {
                _userProfile.IssueInvoiceItem(mgr, item.InvoiceItem);
            }
        }


    }
}