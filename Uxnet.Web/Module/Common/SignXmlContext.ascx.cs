namespace Uxnet.Web.Module.Common
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Web.UI;

    using Utility;
    using System.Text;
    using Uxnet.Com.Security.UseCrypto;
    using System.Xml;
    using Uxnet.Web.Properties;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    ///		Summary description for Page_CA.
    /// </summary>
    public partial class SignXmlContext : System.Web.UI.UserControl, ICallbackEventHandler
    {

        private Control _launcher;
        private string _subject;
        private bool _slientAsSignin;

        public event EventHandler BeforeSign;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Put user code to initialize the page here
        }

        private void initializePage()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("var signableObj = '{0}';\r\n", VirtualPathUtility.ToAbsolute(Settings.Default.SignableObjectUrl)));
            sb.Append("function startSigning() {\r\n")
                .Append("__theFormPostData = '';WebForm_InitCallback();\r\n");
            if (_slientAsSignin)
            {
                sb.Append(Page.ClientScript.GetCallbackEventReference(this, "''", "signContextSilently", ""));
            }
            else
            {
                sb.Append(Page.ClientScript.GetCallbackEventReference(this, "''", "signContext", ""));
            }
            sb.Append(";\r\n return false;\r\n}\r\n");

            Page.ClientScript.RegisterClientScriptBlock(typeof(SignContext), "callback", sb.ToString(), true);
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
            this.PreRender += new System.EventHandler(this.Page_CA_PreRender);
        }
        #endregion

        public Control Launcher
        {
            get
            {
                return _launcher;
            }
            set
            {
                _launcher = value;
            }
        }

        public string DataSignature
        {
            get
            {
                return Request["dataSignature"];
            }
        }

        public XmlDocument DataToSign
        {
            get;
            set;
        }

        public bool AutoSign { get; set; }

        public X509Certificate2 SignerCertificate { get; set; }

        public bool SilentlyAsSigning
        {
            get
            {
                return _slientAsSignin;
            }
            set
            {
                _slientAsSignin = value;
            }
        }

        public bool Verify()
        {
            if (!this.Visible)
            {
                return true;
            }
            else
            {
                return doVerify();
            }
        }

        private bool doVerify()
        {
            String dataSignature = Request["dataSignature"];

            if (String.IsNullOrEmpty(dataSignature))
            {
                return false;
            }

            CryptoUtility ca = new CryptoUtility();
            if (ca.VerifyXmlSignature(dataSignature))
            {
                _subject = ca.CA_Log.Subject;
                SignerCertificate = new X509Certificate2(ca.SignerCertificate);
                return true;
            }
            return false;
        }


        protected void Page_CA_PreRender(object sender, EventArgs e)
        {
            initializePage();

            if (null != this._launcher && this.Visible)
            {
                String signing = String.Format("document.all('{0}').onclick = startSigning;\r\n",_launcher.ClientID);
                Page.ClientScript.RegisterStartupScript(typeof(SignContext), "doCA", signing, true);

                StringBuilder sb = new StringBuilder();
                sb.Append("function afterSigned() {\r\n")
                    .Append(Page.ClientScript.GetPostBackEventReference(_launcher, ""))
                    .Append(";\r\n}\r\n");

                Page.ClientScript.RegisterClientScriptBlock(typeof(SignContext), "afterSigned", sb.ToString(), true);

                if (!this.IsPostBack && AutoSign)
                {
                    Page.ClientScript.RegisterStartupScript(typeof(SignContext), "autoSign",
                       "startSigning();", true);
                }
            }

        }

        #region ICallbackEventHandler жин√

        public string GetCallbackResult()
        {
            return DataToSign.OuterXml;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            if (BeforeSign != null)
            {
                BeforeSign(this, new EventArgs());
            }
        }

        #endregion
    }
}
