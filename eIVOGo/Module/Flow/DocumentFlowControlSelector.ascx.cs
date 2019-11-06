using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Uxnet.Web.Module.DataModel;
using Model.DocumentFlowManagement;
using System.ComponentModel;

namespace eIVOGo.Module.Flow
{
    public partial class DocumentFlowControlSelector : ItemSelector<FlowEntityDataContext,DocumentFlowControl>
    {

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.QueryExpr = f => f.FlowID == (int?)modelItem.DataItem;
            selector.DataSource = this.Select()
                .OrderBy(o => o.StepID).Select(o => new
                        {
                            o.LevelExpression.Expression,
                            o.StepID
                        }
                    );
        }

        //protected override void selector_DataBound(object sender, EventArgs e)
        //{
        //    selector.Items.Clear();

        //    selector.Items.AddRange(((DocumentFlowControlDataSource)dsEntity).CreateDataManager().EntityList.Where(QueryExpr)
        //        .OrderBy(o => o.StepID).Select(o => new ListItem(
        //             o.LevelExpression.Expression, o.StepID.ToString())
        //            ).ToArray());

        //    base.selector_DataBound(sender, e);
        //}

    }
}