using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;
using System.Drawing;
using Model.Locale;

namespace eIVOGo.Module.EIVO
{
    public partial class ConfirmInvoiceDownload : System.Web.UI.UserControl
    {
        protected UserProfileMember _userProfile;
        public event EventHandler Done;

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
            signContext.Launcher = btnOK;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            signContext.BeforeSign += new EventHandler(signContext_BeforeSign);
            this.PreRender += new EventHandler(ConfirmInvoiceDownload_PreRender);
        }

        void ConfirmInvoiceDownload_PreRender(object sender, EventArgs e)
        {
            prepareData();
        }

        void signContext_BeforeSign(object sender, EventArgs e)
        {
            signContext.DataToSign = dataToSign.Text;
        }


        public int? InvoiceID
        {
            get
            {
                return (int?)ViewState["invoiceID"];
            }
            set
            {
                ViewState["invoiceID"] = value;
            }
        }

        protected virtual void prepareData()
        {
            if (InvoiceID.HasValue)
            {
                var item = dsEntity.CreateDataManager().EntityList.Where(a => a.InvoiceID == InvoiceID).FirstOrDefault();
                if (item != null)
                {
                    StringBuilder sb = new StringBuilder("您欲授權發票資料可再下載成大平台傳輸格式:\r\n");
                    sb.Append("作業時間:").Append(DateTime.Now.ToString()).Append("\r\n");
                    sb.Append("發票號碼:").Append(item.TrackCode).Append(item.No).Append("\r\n");

                    dataToSign.Text = sb.ToString();
                }
            }
        }

        public void Show()
        {
            signContext.RegisterAutoSign();
            this.ModalPopupExtender.Show();
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            if (signContext.Verify())
            {
                save();
                //ModalPopupExtender.Hide();
                this.AjaxAlert("重設資料完成!!");
                if (Done != null)
                {
                    Done(this, new EventArgs());
                }
            }
            else
            {
                WebMessageBox.AjaxAlert(this, "驗簽失敗!!");
            }
        }

        protected virtual void save()
        {
            var mgr = dsEntity.CreateDataManager();
            var item = dsEntity.CreateDataManager().EntityList.Where(a => a.InvoiceID == InvoiceID).FirstOrDefault();
            if (item != null)
                item.CDS_Document.ResetDocumentDispatch(Naming.DocumentTypeDefinition.E_Invoice);
            mgr.SubmitChanges();
        }


        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.ModalPopupExtender.Hide();
        }
    }
}