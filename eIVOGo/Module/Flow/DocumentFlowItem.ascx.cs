using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Model.DocumentFlowManagement;
using eIVOGo.Module.Base;

namespace eIVOGo.Module.Flow
{
    public partial class DocumentFlowItem : EditEntityItem<FlowEntityDataContext,DocumentFlow>
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void createEntity(DetailsViewInsertEventArgs e)
        {
            var mgr = dsEntity.CreateDataManager();
            DocumentFlow item = new DocumentFlow
            {
                FlowName = (String)e.Values["FlowName"]
            };
            mgr.EntityList.InsertOnSubmit(item);
            mgr.SubmitChanges();

        }

        protected override void updateEntity(DetailsViewUpdateEventArgs e)
        {
            var mgr = dsEntity.CreateDataManager();
            var item = mgr.EntityList.Where(r => r.FlowID == (int)e.Keys[0]).First();
            item.FlowName = (String)e.NewValues["FlowName"];
            mgr.SubmitChanges();
        }
    }
}