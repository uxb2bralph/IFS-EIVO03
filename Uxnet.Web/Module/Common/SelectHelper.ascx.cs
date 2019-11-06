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

namespace Uxnet.Web.Module.Common
{
    public partial class SelectHelper : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Put user code to initialize the page here

            _helper.EnableViewState = false;

            if (Page.Items["__SelectHelper"] == null)
            {
                Page.Items["__SelectHelper"] = this.Page.ToString();
            }

            if (!this.IsPostBack)
            {
                initializeData();
            }
        }

        public bool IsSelectorOnly
        {
            get
            {
                return !_helper.Visible;
            }
            set
            {
                _helper.Visible = !value;
            }
        }

        private void initializeData()
        {
        }

        public HtmlInputText Helper
        {
            get
            {
                return _helper;
            }
        }

        public HtmlSelect Select
        {
            get
            {
                return _select;
            }
        }

        public string CssClass
        {
            get
            {
                return _helper.Attributes["class"];
            }
            set
            {
                if (_helper.Attributes["class"] == null)
                {
                    _helper.Attributes.Add("class", value);
                }
                else
                {
                    _helper.Attributes["class"] = value;
                }
                if (_select.Attributes["class"] == null)
                {
                    _select.Attributes.Add("class", value);
                }
                else
                {
                    _select.Attributes["class"] = value;
                }
            }
        }

        public string Value
        {
            get
            {
                return _select.Value;
            }
            set
            {
                _select.Value = value;
            }
        }

        public string Text
        {
            get
            {
                return (_select.SelectedIndex >= 0) ? _select.Items[_select.SelectedIndex].Text : null;
            }
        }

        protected override void Render(System.Web.UI.HtmlTextWriter writer)
        {
            base.Render(writer);

            if (Page.Items["__SelectHelper"]!=null)
            {
                Page.Items.Remove("__SelectHelper");

                writer.WriteLine(@"
<script language=""javascript"">
<!--
	function selectIndex(textObj,selectObj)
	{
		for(i=0;i<selectObj.options.length;i++)
		{
			opt = selectObj.options[i];
			if(opt.text.length>=textObj.value.length)
			{
				if(opt.text.indexOf(textObj.value)>=0)
				{
					selectObj.selectedIndex = i;
					return;
				}
			}
		}
	}
//-->
	</script>
");
            }
        }



        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PreRender += new EventHandler(SelectHelper_PreRender);

        }
        #endregion

        private void SelectHelper_PreRender(object sender, EventArgs e)
        {
            _helper.Attributes.Add("onkeyup", "javascript:selectIndex(this," + _select.ClientID + ");");
        }
    }
}