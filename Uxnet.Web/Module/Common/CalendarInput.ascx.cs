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
using System.ComponentModel;

namespace Uxnet.Web.Module.Common
{
    public partial class CalendarInput : System.Web.UI.UserControl, IValidator, IPostBackEventHandler
    {
        protected DateTime _dateTime;
        protected bool _isValid = false;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            this.Page.Validators.Add(this);
            initializeData();
        }

        protected virtual void initializeData()
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
            imgCalendar.Attributes.Add("onclick", String.Format("javascript:calendar(document.all('{0}'));", txtDate.ClientID));
        }

        protected virtual void parseInputDateTime(String dateTimeStr)
        {
            if (!String.IsNullOrEmpty(dateTimeStr))
            {
                _isValid = DateTime.TryParse(dateTimeStr, out _dateTime);
            }
        }

        public event EventHandler DateTimeValueChanged;

        public TextBox @TextBox
        {
            get
            {
                return txtDate;
            }
        }

        public bool Required
        {
            get
            {
                return (ViewState["required"] != null) ? (bool)ViewState["required"] : false;
            }
            set
            {
                ViewState["required"] = value;
            }
        }

        [Bindable(true)]
        public DateTime DateTimeValue
        {
            get
            {
                return _dateTime;
            }
            set
            {
                _dateTime = value;
                _isValid = true;
                txtDate.Text = getDateTimeString();
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
            this.PreRender += new EventHandler(CalendarInput_PreRender);
        }

        void CalendarInput_PreRender(object sender, EventArgs e)
        {
            registerDatePickerScript();

            if (_isValid)
            {
                this.txtDate.Text = getDateTimeString();
            }
            else
            {
                this.txtDate.Text = "";
            }
        }

        #endregion

        protected virtual string getDateTimeString()
        {
            return _dateTime.ToString("yyyy/M/d");
        }

        protected virtual void registerDatePickerScript()
        {
            //if (ScriptManager.GetCurrent(Page) != null)
            //{
            //    ScriptManager.RegisterStartupScript(this, typeof(Calendar), "__Calendar", buildScript().ToString(), true);
            //}
            //else
            {
                if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(CalendarInput), "__Calendar"))
                {
                    StringBuilder sb = buildScript();
                    Page.ClientScript.RegisterClientScriptBlock(typeof(CalendarInput), "__Calendar", sb.ToString(), true);
                }
            }
        }

        private StringBuilder buildScript()
        {
            String postBackScript = "";
            //if (DateTimeValueChanged != null)
            //{
            //    postBackScript = Page.ClientScript.GetPostBackEventReference(this, "");
            //}

            StringBuilder sb = new StringBuilder();

            sb.Append(@"function calendar(objDate)  { 
                           var val;");
            sb.Append(String.Format("val = window.showModalDialog('{0}','','dialogwidth=150pt;dialogheight=170pt;status=no;help=no;');", getCalendarUrl()));
            sb.Append(@"if (val  != -1 && val  != null) {  
                              objDate.value = val;");
            sb.Append(postBackScript);
            sb.Append(@"
                           } 
                        }");
            return sb;
        }

        protected virtual string getCalendarUrl()
        {
            return VirtualPathUtility.AppendTrailingSlash(this.TemplateSourceDirectory) + "calendar.htm";
        }

        public bool HasValue
        {
            get
            {
                return _isValid;    // txtDate.Text.Length > 0;
            }
        }

        public void Reset()
        {
            _isValid = false;
            txtDate.Text = "";
        }

        #region IValidator 成員

        public string ErrorMessage
        {
            get
            {
                return this.errorMsg.Text;
            }
            set
            {
                this.errorMsg.Text = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid || !Required;
            }
            set
            {
                //_isValid = value;
            }
        }

        public void Validate()
        {
            this.errorMsg.Visible = !IsValid;
        }

        #endregion

        #region IPostBackEventHandler 成員

        public void RaisePostBackEvent(string eventArgument)
        {
            if (DateTimeValueChanged != null)
            {
                DateTimeValueChanged(this, new EventArgs());
            }
        }

        #endregion



        public void ScrollIntoViewAndFocus()
        {
            if (ScriptManager.GetCurrent(this.Page) != null)
            {
                ScriptManager.RegisterStartupScript(this.txtDate, this.txtDate.GetType(), "focus",
                    String.Format("document.all('{0}').scrollIntoView();\r\n", this.txtDate.ClientID), true);
                this.txtDate.Focus();
            }
            else
            {
                this.Page.ClientScript.RegisterStartupScript(this.txtDate.GetType(), "focus",
                    String.Format("document.all('{0}').scrollIntoView();\r\n", this.txtDate.ClientID), true);
                this.txtDate.Focus();
            }
        }
    }
}