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
    using System.Text;
    using System.Collections.Generic;

	/// <summary>
	///		PrintingButton 的摘要描述。
	/// </summary>
    public partial class OpenPrintingButton : System.Web.UI.UserControl
	{
		private bool _autoPrint;

        public event EventHandler BeforeClick;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// 在這裡放置使用者程式碼以初始化網頁
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

        public Button btnPrint
        {
            get
            {
                return this._btnPrintIt;
            }
        }

        public string UrlToPrint
        {
            get
            {
                return (string)this.ViewState["urlToPrn"];
            }
            set
            {
                this.ViewState["urlToPrn"] = value;
            }
        }

        private List<Control> _printControls  = new List<Control>();
        public List<Control> PrintControls
        {
            get
            {
                return _printControls;
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

		}
		#endregion

		protected void _btnPrintIt_Click(object sender, System.EventArgs e)
		{
            if (BeforeClick != null)
            {
                BeforeClick(this, new EventArgs());
            }

            if (this.ViewState["urlToPrn"] != null)
            {
                Page.Items["PrintDoc"] = _printControls;
                Server.Transfer((string)this.ViewState["urlToPrn"]);
                //				Response.Redirect((string)this.ViewState["urlToPrn"]);
            }
        }

		private void PrintingButton_PreRender(object sender, EventArgs e)
		{
            if (!Page.ClientScript.IsClientScriptBlockRegistered(typeof(OpenPrintingButton), "printScript"))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(@"
	                var mainForm;
                    var prnWin;

	                function printDocument(elmt)
	                {
                        prnWin = window.open('','prnWin','toolbar=no,location=no,status=no,menubar=no,scrollbars=auto,resizable=no,alwaysRaised,dependent,titlebar=no,width=64,height=48');
                        
		                mainForm = elmt.form;
		                mainForm.target = 'prnWin';
		                prnWin.focus();
                        prnWin.onload = startPrint;
		                return true;
	                }

                    function startPrint()
                    {
                        prnWin.onafterprint = afterPrint;
                        prnWin.print();
                        afterPrint();
                    }
                	
	                function afterPrint()
	                {
                        if(prnWin!=null)
                        {
                            prnWin.close();
                        }
		                if(mainForm!=null)
		                {
			                mainForm.target = """";
		                }
	                }
				");

                Page.ClientScript.RegisterClientScriptBlock(typeof(OpenPrintingButton), "printScript", sb.ToString(), true);
            }


			if(!this.IsPostBack)
			{
				if(_autoPrint)
				{
                    if (!this.Page.ClientScript.IsClientScriptBlockRegistered("autoPrint"))
                    {
                        this.Page.ClientScript.RegisterStartupScript(typeof(String), "autoPrint",
                            String.Format("document.all('{0}').click();", this._btnPrintIt.ClientID), true);
                    }
				}

			}


		}
	}
}
