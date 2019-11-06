namespace Uxnet.Web.Module.Common
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    ///		PrintingButton 的摘要描述。
    /// </summary>
    public partial class PrintingButton : System.Web.UI.UserControl
    {

        protected List<Control> _printControls;
        protected bool _autoPrint;
        protected bool _isValid = true;

        /// <summary>
        /// prnFrame control.
        /// </summary>
        /// <remarks>
        /// Auto-generated field.
        /// To modify move field declaration from designer file to code-behind file.
        /// </remarks>
        protected Control prnFrame;


        public event EventHandler BeforeClick;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // 在這裡放置使用者程式碼以初始化網頁
            if (this.Page.Items["__Printing"] == null)
            {
                this.Page.Items["__Printing"] = this.Page.ToString();
            }

            String script = @"
	                        var mainForm;
	                        function printDocument(elmt)
	                        {
		                        mainForm = elmt.form;
		                        mainForm.target = 'prnFrame';
                                if(typeof(prnFrame)!='undefined')
                                {
		                            prnFrame.focus();
                                    //		prnFrame.onafterprint = window.onafterprint;
                                }
                                else
                                {
                                    var newWin = window.open('','prnFrame','toolbar=no,location=no,status=no,menubar=no,scrollbars=auto,resizable=yes,alwaysRaised,dependent,titlebar=no,width=64,height=48');
                                    newWin.onload = afterprint;
                                    setTimeout('afterprint()', 10000);
                                }
		                        return true;
	                        }
	
	                        function afterprint()
	                        {
		                        if(mainForm!=null)
		                        {
			                        mainForm.target = '';
		                        }
	                        }

                            window.onafterprint = afterprint;
                            ";

            if (ScriptManager.GetCurrent(Page) != null)
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "printDoc", script, true);
                ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this);
            }
            else
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "printDoc", script, true);
            }

            _btnPrintIt.OnClientClick = "printDocument(this);";

        }

        public bool AutoPrint
        {
            get
            {
                return _autoPrint;
            }
            set
            {
                _autoPrint = value;
            }
        }

        public bool IsValid
        {
            get
            {
                return _isValid;
            }
            set
            {
                _isValid = value;
            }
        }

        public Button btnPrint
        {
            get
            {
                return this._btnPrintIt;
            }
        }

        public List<Control> PrintControls
        {
            get
            {
                return _printControls;
            }
        }

        public string ContentGeneratorUrl
        {
            get
            {
                return (string)this.ViewState["Url"];
            }
            set
            {
                this.ViewState["Url"] = value;
            }
        }


        #region Web Form 設計工具產生的程式碼
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: 此為 ASP.NET Web Form 設計工具所需的呼叫。
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		此為設計工具支援所必須的方法 - 請勿使用程式碼編輯器修改
        ///		這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.PreRender += new EventHandler(PrintingButton_PreRender);
            this._printControls = new List<Control>();

        }
        #endregion

        protected void OnBeforeClick(EventArgs e)
        {
            if (BeforeClick != null)
            {
                BeforeClick(this, e);
            }
        }

        protected virtual void _btnPrintIt_Click(object sender, System.EventArgs e)
        {
            OnBeforeClick(new EventArgs());

            if (_isValid && _printControls.Count > 0 && !String.IsNullOrEmpty(ContentGeneratorUrl))
            {

                Session["PrintDoc"] = _printControls;
                Server.Transfer(ContentGeneratorUrl);
                //				Response.Redirect(PageConfigHandler.PrintGeneratorUrl);
            }
        }

        private void PrintingButton_PreRender(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                if (_autoPrint)
                {
                    if (!this.Page.ClientScript.IsClientScriptBlockRegistered("autoPrint"))
                    {
                        this.Page.ClientScript.RegisterStartupScript(typeof(String), "autoPrint",
                            String.Format("document.all('{0}').click();", this._btnPrintIt.ClientID), true);
                    }
                }
            } 
            if (this.Page.Items["__Printing"] != null)
            {
                prnFrame.Visible = true;
                this.Page.Items.Remove("__Printing");
            }
            else
            {
                prnFrame.Visible = false;
            }
        }
    }
}
