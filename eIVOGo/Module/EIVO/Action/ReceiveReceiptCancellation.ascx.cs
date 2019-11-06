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
    public partial class ReceiveReceiptCancellation : ReceiveInvoice
    {

        protected override void signContext_BeforeSign(object sender, EventArgs e)
        {
            if (_docID != null && _docID.Count() > 0)
            {
                var receipts = dsEntity.CreateDataManager().EntityList.Where(i => _docID.Contains(i.DocID)).Select(i => i.DerivedDocument.ParentDocument.ReceiptItem);

                StringBuilder sb = new StringBuilder("您欲接收的作廢收據資料如下\r\n");
                sb.Append("營業人登入帳號:").Append(_userProfile.PID).Append("\r\n");
                sb.Append("營業人名稱:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.CompanyName).Append("\r\n");
                sb.Append("營業人統編:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo).Append("\r\n");
                sb.Append("接收時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                sb.Append("收據號碼\t\t收據日期\t\t收據開立人\r\n");

                foreach (var receipt in receipts)
                {
                    sb.Append(receipt.No).Append("\t")
                        .Append(ValueValidity.ConvertChineseDateString(receipt.ReceiptDate)).Append("\t")
                        .Append(receipt.Seller.CompanyName).Append("\r\n");
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
                _userProfile.ReceiveReceiptCancellation(mgr, item);
            }
        }

    }
}