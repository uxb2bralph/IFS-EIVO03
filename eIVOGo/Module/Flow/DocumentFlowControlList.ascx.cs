using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using eIVOGo.Module.Base;
using Model.DocumentFlowManagement;
using Uxnet.Web.WebUI;
using Utility;

namespace eIVOGo.Module.Flow
{
    public partial class DocumentFlowControlList : EntityItemActionList<FlowEntityDataContext, DocumentFlowControl>
    {
        protected DocumentFlow _item;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.QueryExpr = f => f.FlowID == (int?)modelItem.DataItem;
            _item = dsEntity.CreateDataManager().GetTable<DocumentFlow>().Where(f => f.FlowID == (int?)modelItem.DataItem).FirstOrDefault();
        }

        protected override void create()
        {
            editItem.Show();
        }

        protected override void edit(string keyValue)
        {
            editItem.QueryExpr = r => r.StepID == int.Parse(keyValue);
            editItem.BindData();
        }

        protected override void delete(string keyValue)
        {
            try
            {
                dsEntity.CreateDataManager().DeleteAny(r => r.StepID == int.Parse(keyValue));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                this.AjaxAlert("刪除資料失敗,原因:" + ex.Message);
            }
        }
    }
}