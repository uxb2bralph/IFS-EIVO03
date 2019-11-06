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

namespace Uxnet.Web.Module.Common
{
    public partial class ROCCalendarInput : CalendarInput
    {

        protected override void parseInputDateTime(string dateTimeStr)
        {
            string[] ts = dateTimeStr.Split('.');
            int year, month, day;
            if (ts.Length == 3 && int.TryParse(ts[0], out year) && int.TryParse(ts[1], out month) && int.TryParse(ts[2], out day))
            {
                _dateTime = new DateTime(year + 1911, month, day);
                _isValid = true;
            }
        }

        protected override string getDateTimeString()
        {
            return ValueValidity.ConvertChineseDate(_dateTime);
        }

        protected override string getCalendarUrl()
        {
            return VirtualPathUtility.AppendTrailingSlash(this.TemplateSourceDirectory) + "ROC_calendar.htm";
        }

    }
}