using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Uxnet.Web.Module.DataModel
{
    public partial class MasterEntity : DetailEntity
    {

        internal String _searchTerm
        {
            get
            {
                return ViewState["search"] as string;
            }
            set
            {
                ViewState["search"] = value;
            }
        }

    }
}