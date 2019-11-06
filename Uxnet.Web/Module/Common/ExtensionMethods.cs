using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Uxnet.Web.Module.Common
{
    public static partial class ExtensionMethods
    {
        public static String TextToUpper(this TextBox box)
        {
            return box.Text.Trim().ToUpper();
        }

        public static String ValueToUpper(this HtmlInputText box)
        {
            return box.Value.Trim().ToUpper();
        }

        public static String ValueToUpper(this HtmlTextArea box)
        {
            return box.Value.Trim().ToUpper();
        }

        public static void SetPageIndex(this GridView gvEntity, String pagingControlID, int recordCount)
        {
            if (gvEntity.BottomPagerRow != null)
            {
                PagingControl paging = (PagingControl)gvEntity.BottomPagerRow.Cells[0].FindControl(pagingControlID);
                paging.RecordCount = recordCount;
                paging.CurrentPageIndex = gvEntity.PageIndex;
            }
        }

    }
}
