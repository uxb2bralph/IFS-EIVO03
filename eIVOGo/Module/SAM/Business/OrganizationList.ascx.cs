using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Business.Helper;
using Model.Locale;

namespace eIVOGo.Module.SAM.Business
{
    public partial class OrganizationList : EntityItemList<EIVOEntityDataContext, Organization>
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

    }
}