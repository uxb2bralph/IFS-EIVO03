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
    public partial class DocumentFlowBranchList : EntityItemList<FlowEntityDataContext, DocumentFlowBranch>
    {
        protected DocumentFlowControl _item;

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
    }
}