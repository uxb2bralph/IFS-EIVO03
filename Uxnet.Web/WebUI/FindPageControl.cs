using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.UI;

namespace Uxnet.Web.WebUI
{
    public class FindPageControl
    {
        public static T FindControl<T>(Page page, string id) where T : Control
        {
            return IFindControl<T>(page, id);
        }


        public static T IFindControl<T>(Control startingControl, string id) where T : Control
        {
            // this is null by default
            T found = default(T);

            int controlCount = startingControl.Controls.Count;

            if (controlCount > 0)
            {
                for (int i = 0; i < controlCount; i++)
                {
                    Control activeControl = startingControl.Controls[i];
                    if (activeControl is T)
                    {
                        found = startingControl.Controls[i] as T;
                        if (string.Compare(id, found.ID, true) == 0) break;
                        else found = IFindControl<T>(activeControl, id);
                    }
                    else
                    {
                        found = IFindControl<T>(activeControl, id);
                        if (found != null) break;
                    }
                    if (found != null) break;
                }
            }
            return found;
        }
    }
}
