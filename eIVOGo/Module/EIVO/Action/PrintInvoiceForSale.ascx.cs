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
using Model.InvoiceManagement;
using eIVOGo.Module.UI;

namespace eIVOGo.Module.EIVO.Action
{
    public partial class PrintInvoiceForSale : ReceiveInvoice
    {
        protected override void signContext_BeforeSign(object sender, EventArgs e)
        {
            if (_docID != null && _docID.Count() > 0)
            {
                var invoices = dsEntity.CreateDataManager().EntityList.Where(i => _docID.Contains(i.DocID)).Select(i => i.InvoiceItem);

                StringBuilder sb = new StringBuilder("您欲下載列印的發票資料如下\r\n");
                sb.Append("營業人登入帳號:").Append(_userProfile.PID).Append("\r\n");
                sb.Append("營業人名稱:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.CompanyName).Append("\r\n");
                sb.Append("營業人統編:").Append(_userProfile.CurrentUserRole.OrganizationCategory.Organization.ReceiptNo).Append("\r\n");
                sb.Append("列印時間:").Append(DateTime.Now.ToString()).Append("\r\n");
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
            //_userProfile.EnqueueInvoicePrint(new InvoiceManager(dsEntity.CreateDataManager()), _docID);
            _userProfile.EnqueueDocumentPrint(new InvoiceManager(dsEntity.CreateDataManager()), _docID);
            //ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "open",
            //    String.Format("window.open('{0}?printBack={1}','prnWin','toolbar=no,location=no,status=no,menubar=no,scrollbars=auto,resizable=yes,alwaysRaised,dependent,titlebar=no,width=64,height=48');", VirtualPathUtility.ToAbsolute("~/SAM/PrintInvoiceAsPDF.aspx"), Request["printBack"])
            //    , true);
            string printAll = "0";
            LiteralControl lc;
            if (rdbAll != null)
            {
                if (!this.rdbAll.Checked)
                {
                    printAll = "1";
                }
                lc = new LiteralControl(String.Format("<iframe src='{0}?printBack={1}&printAll={2}' height='0' width='0'></iframe>"
                        , VirtualPathUtility.ToAbsolute("~/SAM/PrintInvoiceAsPDF.aspx"),Request["printBack"], printAll));
            }
            else
            {
                lc = new LiteralControl(String.Format("<iframe src='{0}?printBack={1}' height='0' width='0'></iframe>"
                        , VirtualPathUtility.ToAbsolute("~/SAM/PrintInvoiceAsPDF.aspx"), Request["printBack"]));
            }
            
            this.Controls.Add(lc);

            PopupModal modal = (PopupModal)this.LoadControl("~/Module/UI/PopupModal.ascx");
            modal.InitializeAsUserControl(this.Page);
            modal.TitleName = _successfulMsg;
            _successfulMsg = null;
            this.Controls.Add(modal);
            modal.Show();
        }


    }
}