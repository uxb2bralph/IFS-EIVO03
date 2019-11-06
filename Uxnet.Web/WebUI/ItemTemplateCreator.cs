using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace Uxnet.Web.WebUI
{
    public partial class ItemTemplateCreator : ITemplate
    {
        public Func<Control> BuildControl
        { get; set; }

        public void InstantiateIn(Control container)
        {
            if (BuildControl != null)
            {
                container.Controls.Add(BuildControl());
            }
        }
    }
}