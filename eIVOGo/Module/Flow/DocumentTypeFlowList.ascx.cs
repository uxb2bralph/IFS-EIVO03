using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Module.Base;
using Model.DocumentFlowManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Uxnet.Web.WebUI;

namespace eIVOGo.Module.Flow
{
    public partial class DocumentTypeFlowList : EntityItemList<FlowEntityDataContext, DocumentTypeFlow>
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            doCreate.DoAction = arg =>
            {
                editItem.Show();
            };

            doDelete.DoAction = arg =>
            {
                delete(arg);
            };
        }


        protected void delete(string keyValue)
        {
            try
            {
                var dataKey = keyValue.GetKeyValue();
                dsEntity.CreateDataManager().DeleteAny(f => f.TypeID == dataKey[0] && f.FlowID == dataKey[1] && f.CompanyID == dataKey[2] && f.BusinessID == dataKey[3]);
                this.AjaxAlert("資料已刪除!!");
            }
            catch (Exception ex)
            {
                this.AjaxAlert("刪除失敗!!原因:" + ex.Message);
            }
        }

    }
}