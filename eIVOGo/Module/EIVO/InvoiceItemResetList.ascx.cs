using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Model.Locale;
using Model.Security.MembershipManagement;
using Model.InvoiceManagement;
using Business.Helper;
using Model.DataEntity;
using Uxnet.Web.Module.Common;
using System.Linq.Expressions;
using Utility;
using eIVOGo.Module.Inquiry;

namespace eIVOGo.Module.EIVO
{
    public partial class InvoiceItemResetList : InvoiceItemQueryList
    {
        public override void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("R:"))
            {
                confirmDownload.InvoiceID = int.Parse(eventArgument.Substring(2));
                confirmDownload.Show();
            }
            else
            {
                base.RaisePostBackEvent(eventArgument);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            confirmDownload.Done += new EventHandler(confirmDownload_Done);
        }

        void confirmDownload_Done(object sender, EventArgs e)
        {
            gvEntity.DataBind();
        }
    }    
}