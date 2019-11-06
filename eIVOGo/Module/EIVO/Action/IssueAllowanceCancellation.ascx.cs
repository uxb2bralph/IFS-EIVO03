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
    public partial class IssueAllowanceCancellation : ReceiveInvoice
    {

        protected override void signContext_BeforeSign(object sender, EventArgs e)
        {
            if (_docID != null && _docID.Count() > 0)
            {
                var allowanceList = dsEntity.CreateDataManager().EntityList.Where(i => _docID.Contains(i.DocID)).Select(i => i.DerivedDocument.ParentDocument.InvoiceAllowance);

                StringBuilder sb = new StringBuilder("您欲開立的作廢發票折讓資料如下\r\n");
                sb.Append("營業人登入帳號:").Append(_userProfile.PID).Append("\r\n");
                sb.Append("營業人名稱:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.CompanyName).Append("\r\n");
                sb.Append("營業人統編:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo).Append("\r\n");
                sb.Append("接收時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                sb.Append("折讓單號碼\t\t折讓日期\t\t接收折讓單營業人\r\n");

                foreach (var item in allowanceList)
                {
                    sb.Append(item.AllowanceNumber).Append("\t")
                        .Append(ValueValidity.ConvertChineseDateString(item.AllowanceDate.Value)).Append("\t")
                        .Append(item.InvoiceAllowanceSeller.Organization.CompanyName).Append("\r\n");
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
                _userProfile.IssueAllowanceCancellation(mgr, item);
            }
        }


    }
}