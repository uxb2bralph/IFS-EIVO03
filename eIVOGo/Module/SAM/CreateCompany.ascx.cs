using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.Security.MembershipManagement;
using Model.DataEntity;

using Business.Helper;
using Model.Locale;
using Uxnet.Web.WebUI;
using System.Text.RegularExpressions;
using Utility;

namespace eIVOGo.Module.SAM
{
    public partial class CreateCompany : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            EditItem.Done += new EventHandler(EditItem_Done);
        }

        protected virtual void EditItem_Done(object sender, EventArgs e)
        {
            Response.Redirect("~/SAM/register_msg.aspx?" + String.Format("backUrl=CompanyManager.aspx&action={0}&back={1}", Server.UrlEncode(actionItem.ItemName), Server.UrlEncode("回店家資料維護")));
        }


        //protected virtual void btnOK_Click(object sender, EventArgs e)
        //{
        //    UserProfile profile = EditItem.Update();
        //    if (profile == null)
        //        return;

        //    try
        //    {
        //        if (EditItem.Validate())
        //        {
        //            var mgr = dsUserProfile.CreateDataManager();
        //            UserProfileManager userMgr = new UserProfileManager(mgr);
        //            userMgr.CreateConsumerProfile(profile);
        //            Response.Redirect("register_msg.aspx?backUrl=memberManager.aspx");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Error(ex);
        //        btnOK.AjaxAlert("系統發生錯誤,錯誤原因:" + ex.Message);
        //    }
        //}
    }
}