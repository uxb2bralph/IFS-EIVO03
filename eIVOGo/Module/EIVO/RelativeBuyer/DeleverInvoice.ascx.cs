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

namespace eIVOGo.Module.EIVO.RelativeBuyer
{
    public partial class DeleverInvoice : System.Web.UI.UserControl
    {
        private int[] _invoiceID;
        private UserProfileMember _userProfile;

        protected void Page_Load(object sender, EventArgs e)
        {
            var items = Request.GetItemSelection();
            if (items != null && items.Count() > 0)
            {
                _invoiceID = items.Select(s => int.Parse(s)).ToArray();
            }

            _userProfile = WebPageUtility.UserProfile;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            inquiryAction.Done += new EventHandler(inquiryAction_Done);
            inquiryAction.afterQuery += new EventHandler(inquiryAction_afterQuery);
            signContext.BeforeSign += new EventHandler(signContext_BeforeSign);
            signContext.Launcher = btnReceive;
        }

        void inquiryAction_afterQuery(object sender, EventArgs e)
        {
            if (inquiryAction.dataCount > 0)
            {
                tblAction.Visible = true;
            }
            else
            {
                tblAction.Visible = false;
            }
        }

        void signContext_BeforeSign(object sender, EventArgs e)
        {
            if (_invoiceID != null && _invoiceID.Count() > 0)
            {
                //var invoices = dsEntity.CreateDataManager().EntityList.Where(i => _invoiceID.Contains(i.InvoiceID));
                var ds=dsEntity.CreateDataManager();

                StringBuilder sb = new StringBuilder();
                if (inquiryAction.isInvoice)
                {
                    var invoices = ds.GetTable<CDS_Document>().Where(i => _invoiceID.Contains(i.InvoiceItem.InvoiceID));
                    sb.Append("您欲開立的資料如下\r\n");
                    sb.Append("開立營業人ID:").Append(_userProfile.UID).Append("\r\n");
                    sb.Append("開立營業人名稱:").Append(_userProfile.UserName).Append("\r\n");
                    sb.Append("開立時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                    sb.Append("發票號碼\t\t發票日期\t\t發票接收人\r\n");

                    foreach (var invoice in invoices)
                    {
                        sb.Append(invoice.InvoiceItem.TrackCode).Append(invoice.InvoiceItem.No).Append("\t")
                            .Append(ValueValidity.ConvertChineseDateString(invoice.InvoiceItem.InvoiceDate.Value)).Append("\t")
                            .Append(invoice.InvoiceItem.InvoiceBuyer.Organization.CompanyName).Append("\r\n");
                    }
                }
                else
                {
                    var invoices = ds.GetTable<CDS_Document>().Where(i => _invoiceID.Contains(i.InvoiceAllowance.AllowanceID));
                    sb.Append("您欲接收的資料如下\r\n");
                    sb.Append("接收營業人ID:").Append(_userProfile.UID).Append("\r\n");
                    sb.Append("接收營業人名稱:").Append(_userProfile.UserName).Append("\r\n");
                    sb.Append("接收時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                    sb.Append("折讓號碼\t\t折讓日期\t\t折讓開立人\r\n");

                    foreach (var invoice in invoices)
                    {
                        sb.Append(invoice.InvoiceAllowance.AllowanceNumber).Append("\t")
                            .Append(ValueValidity.ConvertChineseDateString(invoice.InvoiceAllowance.AllowanceDate.Value)).Append("\t")
                            .Append(invoice.InvoiceAllowance.InvoiceAllowanceSeller.Organization.CompanyName).Append("\r\n");
                    }
                }

                signContext.DataToSign = sb.ToString();
            }
        }

        void inquiryAction_Done(object sender, EventArgs e)
        {
            if (inquiryAction.DoQuery)
            {
                //tblAction.Visible = true;
                if (inquiryAction.isInvoice)
                    this.btnReceive.Text = "開立";
                else
                    this.btnReceive.Text = "接收";
            }
            else
            {
                tblAction.Visible = false;
            }
        }

        protected void btnReceive_Click(object sender, EventArgs e)
        {
            if (_invoiceID != null && _invoiceID.Count() > 0)
            {
                if (signContext.Verify())
                {
                    doReceive();
                    this.AjaxAlert("開立/接收完成!!");
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

        private void doReceive()
        {
            var mgr = dsEntity.CreateDataManager();
            //var items = mgr.EntityList.Where(i => _invoiceID.Contains(i.InvoiceID));

            if (inquiryAction.isInvoice)
            {
                var cds = mgr.GetTable<CDS_Document>().Where(i => _invoiceID.Contains(i.InvoiceItem.InvoiceID));
                foreach (var item in cds)
                {
                    //_userProfile.ReceiveInvoiceItem(mgr, item);
                    item.MoveToNextStep(mgr);
                }
            }
            else
            {
                var cds = mgr.GetTable<CDS_Document>().Where(i => _invoiceID.Contains(i.InvoiceAllowance.AllowanceID));
                foreach (var item in cds)
                {
                    //_userProfile.ReceiveInvoiceItem(mgr, item);
                    item.MoveToNextStep(mgr);
                }
            }
        }
    }
}