using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eIVOGo.Module.Base;
using Model.DocumentFlowManagement;
using Uxnet.Web.WebUI;

namespace eIVOGo.Module.Flow
{
    public partial class DocumentFlowList : EntityItemActionList<FlowEntityDataContext, DocumentFlow>
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            doApply.DoAction = arg =>
                {
                    modelItem.DataItem = int.Parse(arg);
                    Server.Transfer(ApplyFlowControl.TransferTo);
                };
        }

        protected override void create()
        {
            editItem.Show();
        }

        protected override void edit(string keyValue)
        {
            editItem.QueryExpr = r => r.FlowID == int.Parse(keyValue);
            editItem.BindData();
        }

        protected override void delete(string keyValue)
        {
            dsEntity.CreateDataManager().DeleteAny(r => r.FlowID == int.Parse(keyValue));
        }
    }
}