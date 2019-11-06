using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Utility;
using System.Text;

namespace Uxnet.Web.Module.Common
{
    public partial class CalendarInputDatePicker : CalendarInput
    {
        protected override void initializeData()
        {
            String dateTimeStr = Request[txtDate.UniqueID];
            if (!String.IsNullOrEmpty(dateTimeStr))
            {
                parseInputDateTime(dateTimeStr);
                if (_isValid)
                {
                    txtDate.Text = getDateTimeString();
                }
            }
        }

       
        protected override void registerDatePickerScript()
        {

        }
               
    }
}