using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Model.DocumentFlowManagement;
using eIVOGo.Module.Base;
using Uxnet.Web.Module.DataModel;

namespace eIVOGo.Module.Flow
{
    public partial class DocumentFlowControlItem : EditEntityItem<FlowEntityDataContext, DocumentFlowControl>
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void createEntity(DetailsViewInsertEventArgs e)
        {
            var mgr = dsEntity.CreateDataManager();

            DocumentFlowControl item = new DocumentFlowControl
            {
                LevelID = int.Parse(((EnumSelector)dvEntity.Rows[1].FindControl("LevelID")).SelectedValue),
                FlowID = ((int?)modelItem.DataItem).Value,
            };

            DocumentFlowControlSelector selector = (DocumentFlowControlSelector)dvEntity.Rows[1].FindControl("NextStep");
            if (!String.IsNullOrEmpty(selector.SelectedValue))
            {
                item.NextStep = int.Parse(selector.SelectedValue);
            }

            selector = (DocumentFlowControlSelector)dvEntity.Rows[1].FindControl("PrevStep");
            if (!String.IsNullOrEmpty(selector.SelectedValue))
            {
                item.PrevStep = int.Parse(selector.SelectedValue);
            }

            if (Request["InitStep"] != null)
            {
                var flow = mgr.GetTable<DocumentFlow>().Where(f => f.FlowID == (int?)modelItem.DataItem).First();
                flow.InitStepItem = item;
            }

            mgr.EntityList.InsertOnSubmit(item);
            mgr.SubmitChanges();

        }

        protected override void updateEntity(DetailsViewUpdateEventArgs e)
        {
            var mgr = dsEntity.CreateDataManager();
            var item = mgr.EntityList.Where(r => r.StepID == (int)e.Keys[0]).First();

            DocumentFlowControlSelector selector = (DocumentFlowControlSelector)dvEntity.Rows[1].FindControl("NextStep");
            item.NextStep = !String.IsNullOrEmpty(selector.SelectedValue) ? int.Parse(selector.SelectedValue) : (int?)null;
            selector = (DocumentFlowControlSelector)dvEntity.Rows[1].FindControl("PrevStep");
            item.PrevStep = !String.IsNullOrEmpty(selector.SelectedValue) ? int.Parse(selector.SelectedValue) : (int?)null;
            item.LevelID = int.Parse(((EnumSelector)dvEntity.Rows[1].FindControl("LevelID")).SelectedValue);

            if (Request["InitStep"] != null)
            {
                var flow = mgr.GetTable<DocumentFlow>().Where(f => f.FlowID == (int?)modelItem.DataItem).First();
                flow.InitialStep = item.StepID;
            }

            mgr.SubmitChanges();
        }

        protected void dvEntity_ItemCreated(object sender, EventArgs e)
        {
            //if (dvEntity.DataItem != null)
            //{
            //    DocumentFlowControl item = (DocumentFlowControl)dvEntity.DataItem;
            //    DocumentFlowControlSelector selector = (DocumentFlowControlSelector)dvEntity.Rows[1].FindControl("PrevStep");
            //    selector.QueryExpr = f => f.FlowID == (int?)modelItem.DataItem;
            //    selector.BindData();
            //    if (item.PrevStep.HasValue)
            //        selector.SelectedValue = item.PrevStep.ToString();
            //    selector = (DocumentFlowControlSelector)dvEntity.Rows[1].FindControl("NextStep");
            //    selector.QueryExpr = f => f.FlowID == (int?)modelItem.DataItem;
            //    selector.BindData();
            //    if (item.NextStep.HasValue)
            //        selector.SelectedValue = item.NextStep.ToString();
            //}
        }
    }
}