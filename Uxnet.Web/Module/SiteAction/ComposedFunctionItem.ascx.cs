using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.ComponentModel;

namespace Uxnet.Web.Module.SiteAction
{
    public partial class ComposedFunctionItem : FunctionItem
    {
        public override XElement Save(XElement menuItem)
        {
            XElement element = base.Save(menuItem);
            if (element != null && childItem != null)
            {
                childItem.Save(element);
            }

            return element;
        }

        public override void RestoreCheckItem(IEnumerable<XElement> elements)
        {
            base.RestoreCheckItem(elements);
            if (childItem != null)
            {
                childItem.RestoreCheckItem(elements);
            }
        }
    }
}