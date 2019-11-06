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
using eIVOGo.Properties;

namespace eIVOGo.Module.EIVO.Action
{
    public partial class ReceiveInvoice : System.Web.UI.UserControl
    {
        protected int[] _docID;
        protected UserProfileMember _userProfile;
        protected InquireEntity inquiryAction;
        protected String _successfulMsg = "作業完成!!";


        protected void Page_Load(object sender, EventArgs e)
        {
            var items = Request.GetItemSelection();
            if (items != null && items.Count() > 0)
            {
                _docID = items.Select(s => int.Parse(s)).ToArray();
            }

            _userProfile = WebPageUtility.UserProfile;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            inquiryAction.Done += new EventHandler(inquiryAction_Done);
            signContext.BeforeSign += new EventHandler(signContext_BeforeSign);
            signContext.Launcher = btnReceive;
        }

        protected virtual void signContext_BeforeSign(object sender, EventArgs e)
        {
            if (_docID != null && _docID.Count() > 0)
            {
                var invoices = dsEntity.CreateDataManager().EntityList.Where(i => _docID.Contains(i.DocID)).Select(i => i.InvoiceItem);

                StringBuilder sb = new StringBuilder("您欲接收的發票資料如下\r\n");
                sb.Append("營業人登入帳號:").Append(_userProfile.PID).Append("\r\n");
                sb.Append("營業人名稱:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.CompanyName).Append("\r\n");
                sb.Append("營業人統編:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo).Append("\r\n");
                sb.Append("接收時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                sb.Append("發票號碼\t\t發票日期\t\t發票開立人\r\n");

                foreach (var invoice in invoices)
                {
                    sb.Append(invoice.TrackCode).Append(invoice.No).Append("\t")
                        .Append(ValueValidity.ConvertChineseDateString(invoice.InvoiceDate.Value)).Append("\t")
                        .Append(invoice.InvoiceSeller.CustomerName).Append("\r\n");
                }

                signContext.DataToSign = sb.ToString();
            }
        }

        void inquiryAction_Done(object sender, EventArgs e)
        {
            tblAction.Visible = true;
        }

        protected void btnReceive_Click(object sender, EventArgs e)
        {
            if (_docID != null && _docID.Count() > 0)
            {
                bool bSigned = false;
                if (signContext.Verify())
                {
                    bSigned = signContext.Entrusting == true ? true : _userProfile.CheckTokenIdentity(dsEntity.CreateDataManager(), signContext);
                }

                if (bSigned)
                {
                    doJob();
                    if (!String.IsNullOrEmpty(_successfulMsg))
                    {
                        this.AjaxAlert(_successfulMsg);
                    }
                }
                else
                {
                    this.AjaxAlert("簽章失敗!!");
                }
            }
            else
            {
                this.AjaxAlert("請選擇項目!!");
            }
        }

        protected virtual void doJob()
        {
            var mgr = dsEntity.CreateDataManager();
            var items = mgr.EntityList.Where(i => _docID.Contains(i.DocID));
            foreach (var item in items)
            {
                _userProfile.ReceiveInvoiceItem(mgr, item.InvoiceItem);
            }
        }


    }
}