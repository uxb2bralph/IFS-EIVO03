using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.Security.MembershipManagement;
using Business.Helper;

namespace eIVOGo.Module.EIVO.TodoCheck
{
    public partial class CheckBuyerInvoice : System.Web.UI.UserControl
    {
        protected UserProfileMember _userProfile;
        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            doLink.DoAction = arg =>
            {
                Response.Redirect(LinkAction.RedirectTo);
            };
        }
    }
}