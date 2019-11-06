using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Helper;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;

namespace eIVOGo.Module.SAM.Business
{
    public partial class ImportCounterpartBusinessCsv : System.Web.UI.UserControl
    {
        private BusinessCounterpartUploadManager _mgr;
        protected UserProfileMember _userProfile;

        protected void Page_Load(object sender, EventArgs e)
        {
            //_userProfile = WebPageUtility.UserProfile;
            //btnConfirm.AttachWaitingMessage("正在處理中，請稍後...", true);
            //if (!this.IsPostBack)
            //{
            //    itemList.UploadManager = null;
            //}
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(ImportCounterpartBusiness_PreRender);
        }

        void ImportCounterpartBusiness_PreRender(object sender, EventArgs e)
        {
            //tblAction.Visible = itemList.UploadManager != null ? itemList.UploadManager.ItemCount > 0 : false ;
            //tblAction.Visible = itemList.UploadManager.ItemCount > 0;
        }

        protected virtual void btnConfirm_Click(object sender, EventArgs e)
        {
            //if (String.IsNullOrEmpty(MasterID.SelectedValue))
            //{
            //    this.AjaxAlert("請先建立集團成員!!");
            //}
            //else if (String.IsNullOrEmpty(BusinessType.SelectedValue))
            //{
            //    this.AjaxAlert("請選擇相對營業人類別!!");
            //}
            //else if (csvFile.HasFile)
            //{
            //    String fileName = Path.Combine(Logger.LogDailyPath, Path.GetFileName(csvFile.PostedFile.FileName));
            //    csvFile.SaveAs(fileName);

            //    _mgr = new B2BBusinessCounterpartUploadManager();
            //    _mgr.BusinessType = (Naming.InvoiceCenterBusinessType)int.Parse(BusinessType.SelectedValue);
            //    _mgr.MasterID = int.Parse(MasterID.SelectedValue);
            //    _mgr.ParseData(_userProfile, fileName, Encoding.GetEncoding(950));

            //    btnSave.Visible = _mgr.IsValid;
            //    itemList.UploadManager = _mgr;
            //}
            //else
            //{
            //    this.AjaxAlert("請匯入檔案!!");
            //}
        }

        protected virtual void btnCancel_Click(object sender, EventArgs e)
        {
            //_mgr = (B2BBusinessCounterpartUploadManager)itemList.UploadManager;
            //_mgr.Dispose();
            //itemList.UploadManager = null;
            //_mgr = null;
        }

        protected virtual void btnSave_Click(object sender, EventArgs e)
        {
            //_mgr = (B2BBusinessCounterpartUploadManager)itemList.UploadManager;
            //if (_mgr.IsValid)
            //{
            //    _mgr.Save();
            //    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('匯入資料完成!!'); location.href='MaintainBusinessRelationship.aspx';", true);
            //    //this.AjaxAlert("匯入資料完成!!");
            //}
            //else
            //{
            //    this.AjaxAlert("資料檔有錯,無法匯入!!");
            //}
        }

        protected void rbChange_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Server.Transfer(rbChange.SelectedValue);
        }
    }
}