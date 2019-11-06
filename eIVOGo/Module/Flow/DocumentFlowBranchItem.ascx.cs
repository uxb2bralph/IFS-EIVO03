using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Module.Base;
using Model.DocumentFlowManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using Uxnet.Web.Module.DataModel;
using Uxnet.Web.WebUI;

namespace eIVOGo.Module.Flow
{
    public partial class DocumentFlowBranchItem : EditEntityItemModal<FlowEntityDataContext, DocumentFlowBranch>
    {

    }
}