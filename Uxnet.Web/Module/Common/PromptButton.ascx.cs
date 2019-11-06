using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;

namespace Uxnet.Web.Module.Common
{
    public partial class PromptButton : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            btnPrompt.OnClientClick = "return promptMessage();";
            StringBuilder sb = new StringBuilder();
            sb.Append(@"
                    function promptMessage() {
		                var promptValue = prompt(""").Append(PromptTitle).Append(@""",'');
                        if (promptValue!=null && promptValue != '') {
                            var form = document.forms[0];
                            var element = document.getElementById('promptValue');
                            if (element == null) {
                                element = document.createElement(""<input type='hidden' name='promptValue'>"");
                                form.appendChild(element);
                            }
                            element.value = promptValue;
                            return true;
                        }
                        return false;
                    }
            ");

            Page.ClientScript.RegisterClientScriptBlock(typeof(PromptButton), "promptValue", sb.ToString(), true);

        }

        public Button @PromtpButton
        {
            get
            {
                return btnPrompt;
            }
        }

        public string PromptTitle
        {
            get
            {
                return (string)ViewState["title"];
            }
            set
            {
                ViewState["title"] = value;
            }
        }

        public string PromptText
        {
            get
            {
                return Request["promptValue"];
            }
        }
    }
}