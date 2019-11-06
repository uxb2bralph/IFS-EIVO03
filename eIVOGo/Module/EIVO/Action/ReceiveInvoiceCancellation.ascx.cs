using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

using eIVOGo.Helper;
using Uxnet.Web.WebUI;
using Utility;
using Model.Security.MembershipManagement;
using Business.Helper;
using Model.DataEntity;

namespace eIVOGo.Module.EIVO.Action
{
    public partial class ReceiveInvoiceCancellation : ReceiveInvoice
    {

        protected override void signContext_BeforeSign(object sender, EventArgs e)
        {
            if (_docID != null && _docID.Count() > 0)
            {
                var invoices = dsEntity.CreateDataManager().EntityList.Where(i => _docID.Contains(i.DocID)).Select(i => i.DerivedDocument.ParentDocument.InvoiceItem);

                StringBuilder sb = new StringBuilder("您欲接收的作廢發票資料如下\r\n");
                sb.Append("營業人登入帳號:").Append(_userProfile.PID).Append("\r\n");
                sb.Append("營業人名稱:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.CompanyName).Append("\r\n");
                sb.Append("營業人統編:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo).Append("\r\n");
                sb.Append("接收時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                sb.Append("發票號碼\t\t作廢發票日期\t\t發票開立人\r\n");

                foreach (var invoice in invoices)
                {
                    sb.Append(invoice.TrackCode).Append(invoice.No).Append("\t")
                        .Append(ValueValidity.ConvertChineseDateString(invoice.InvoiceCancellation.CancelDate)).Append("\t")
                        .Append(invoice.InvoiceSeller.CustomerName).Append("\r\n");
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
                _userProfile.ReceiveInvoiceCancellation(mgr, item);
            }
        }

    }
}