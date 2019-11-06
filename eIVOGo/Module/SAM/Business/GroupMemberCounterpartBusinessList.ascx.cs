using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Module.Base;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Uxnet.Web.Module.Common;
using Uxnet.Web.Module.DataModel;

namespace eIVOGo.Module.SAM.Business
{
    public partial class GroupMemberCounterpartBusinessList : EntityItemList<EIVOEntityDataContext, BusinessRelationship>
    {
        protected UserProfileMember _userProfile;        
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            doActivate.DoAction = arg =>
            {
                var dataKey = arg.GetKeyValue();
                var mgr = dsEntity.CreateDataManager();
                var item = mgr.EntityList.Where(m => m.MasterID == dataKey[0] && m.RelativeID == dataKey[1] && m.BusinessID == dataKey[2]).First();
                item.CurrentLevel = (int)Naming.MemberStatusDefinition.Checked;
                mgr.SubmitChanges();
            };

            doDelete.DoAction = arg =>
            {
                delete(arg);
            };

            doEntrust.DoAction = arg =>
                {
                    var dataKey = arg.GetKeyValue();
                    var mgr = dsEntity.CreateDataManager();
                    var item = mgr.EntityList.Where(m => m.MasterID == dataKey[0] && m.RelativeID == dataKey[1] && m.BusinessID == dataKey[2]).First();
                    item.Counterpart.OrganizationStatus.Entrusting = true;
                    mgr.SubmitChanges();
                };

            doDisableEntrusting.DoAction = arg =>
            {
                var dataKey = arg.GetKeyValue();
                var mgr = dsEntity.CreateDataManager();
                var item = mgr.EntityList.Where(m => m.MasterID == dataKey[0] && m.RelativeID == dataKey[1] && m.BusinessID == dataKey[2]).First();
                item.Counterpart.OrganizationStatus.Entrusting = false;
                mgr.SubmitChanges();
            };
            
            doPrintOff.DoAction = arg =>
            {
                var datakey = arg.GetKeyValue();
                var mgr = dsEntity.CreateDataManager();
                var item = mgr.EntityList.Where(m => m.MasterID == datakey[0] && m.RelativeID == datakey[1] && m.BusinessID == datakey[2]).First();

                //item.Counterpart.OrganizationStatus.PrintStatus = (int)Naming.MemberStatusDefinition.停用列印;
                item.Counterpart.OrganizationStatus.EntrustToPrint = false;

                mgr.SubmitChanges();
            };

            doPrintOn.DoAction = arg =>
            {
                var datakey = arg.GetKeyValue();
                var mgr = dsEntity.CreateDataManager();
                var item = mgr.EntityList.Where(m => m.MasterID == datakey[0] && m.RelativeID == datakey[1] && m.BusinessID == datakey[2]).First();

                //item.Counterpart.OrganizationStatus.PrintStatus = (int)Naming.MemberStatusDefinition.主動列印;
                item.Counterpart.OrganizationStatus.EntrustToPrint = true;

                mgr.SubmitChanges();
            };
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _userProfile = WebPageUtility.UserProfile;
        }

        protected void delete(string keyValue)
        {
            var dataKey = keyValue.GetKeyValue();
            var mgr = dsEntity.CreateDataManager();
            var item = mgr.EntityList.Where(m => m.MasterID == dataKey[0] && m.RelativeID == dataKey[1] && m.BusinessID == dataKey[2]).First();
           item.CurrentLevel = (int)Naming.MemberStatusDefinition.Mark_To_Delete;           
            mgr.SubmitChanges();
        }
    }    
}