using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.WebUI;
using eIVOGo.Helper;

namespace eIVOGo.Module.EIVO
{
    public partial class InvoiceAllowancePrintList : InvoiceAllowanceCheckList
    {
        private UserProfileMember _userProfile;

        protected void btnShow_Click(object sender, EventArgs e)
        {
            String[] ar = GetItemSelection();
            if (ar!=null && ar.Count() > 0)
            {
                //Session["PrintDoc"] = ar.Select(s => int.Parse(s)).ToArray();
                _userProfile.EnqueueDocumentPrint(new InvoiceManager(dsEntity.CreateDataManager()), ar.Select(s => int.Parse(s)));

                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "open",
                    String.Format("window.open('{0}','prnWin','toolbar=no,location=no,status=no,menubar=no,scrollbars=auto,resizable=yes,alwaysRaised,dependent,titlebar=no,width=64,height=48');", VirtualPathUtility.ToAbsolute("~/SAM/PrintAllowancePage.aspx"))
                    , true);
            }
            else
            {
                this.AjaxAlert("請選擇列印資料!!");
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.PreRender += new EventHandler(InvoiceItemPrintList_PreRender);
            this.Load += new EventHandler(InvoiceAllowancePrintList_Load);
        }

        void InvoiceAllowancePrintList_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
        }

        void InvoiceItemPrintList_PreRender(object sender, EventArgs e)
        {
            btnShow.Visible = dsEntity.CurrentView.LastSelectArguments.TotalRowCount > 0;
        }

    }    
}