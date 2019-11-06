using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Uxnet.Web.Module.DataModel;
using System.ComponentModel;
using Model.DataEntity;
using Model.Security.MembershipManagement;
using Business.Helper;

namespace eIVOGo.Module.SAM.Business
{
    public partial class GroupMemberSelector : ItemSelector<EIVOEntityDataContext, EnterpriseGroupMember>
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var mgr = ((EnterpriseGroupMemberDataSource)dsEntity).CreateDataManager();
            selector.DataSource = mgr.EntityList.Select(o => o.CompanyID)
                .Distinct().Join(mgr.GetTable<Organization>(), d => d, o => o.CompanyID, (d, o) => o).OrderBy(o => o.ReceiptNo)
                .Select(o => new
                        {
                            Expression = String.Format("{0} {1}",o.ReceiptNo,o.CompanyName),
                            o.CompanyID
                        }
                    );

            this.PreRender += new EventHandler(GroupMemberSelector_PreRender);
        }

        void GroupMemberSelector_PreRender(object sender, EventArgs e)
        {
            if (!_isBound)
            {
                selector.DataBind();
            }
        }
    }
}