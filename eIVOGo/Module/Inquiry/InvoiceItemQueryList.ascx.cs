using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.Module.Common;
using Business.Helper;

namespace eIVOGo.Module.Inquiry
{
    public partial class InvoiceItemQueryList : InvoiceItemList,IPostBackEventHandler
    {
        protected UserProfileMember _userProfile = WebPageUtility.UserProfile;

        //public string transforData(object o)
        //{
        //    string data = "";
        //    int id = (int)o;            
        //    if (_userProfile.CurrentUserRole.RoleID != ((int)Naming.RoleID.ROLE_SYS))
        //    {
        //        if (id == _userProfile.CurrentUserRole.OrganizationCategory.Organization.CompanyID)
        //            data = "銷項";
        //        else
        //            data = "進項";
        //    }
        //    else
        //    {
        //        var mgr = this.dsEntity.CreateDataManager();
        //        data = mgr.GetTable<BusinessRelationship>().Where(b => b.MasterID == id).FirstOrDefault().BusinessType.Business;
        //    }
        //    return data;
        //}

        public override void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument.StartsWith("S:"))
            {
                this.PNewInvalidInvoicePreview1.setDetail = eventArgument.Substring(2).Trim();
                this.PNewInvalidInvoicePreview1.Popup.Show();
            }
            if (eventArgument.StartsWith("C:"))
            {
                //this.PNewInvalidInvoicePreview1.setCompany = eventArgument.Substring(2).Trim();
                this.PNewInvalidInvoicePreview1.Popup.Show();
            }
        }
    }    
}