using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Utility;
using Uxnet.Com.Security.UseCrypto;
using Uxnet.Web.Properties;

namespace Uxnet.Web.Module.Common
{

    /// <summary>
    ///		Summary description for Page_CA.
    /// </summary>
    public partial class SignContext : ViewUserControl, ICallbackEventHandler
    {

        private Control _launcher;
        private bool _silentAsSigning;

        public event EventHandler BeforeSign;

        public delegate bool Verification(String dataToSign, String dataSignature, out X509Certificate certSigner);
        public Verification DoVerify { get; set; }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Put user code to initialize the page here
        }

        public bool IsCmsEnveloped { get; set; }

        [Bindable(true)]
        public String Thumbprint
        {
            get;
            set;
        }

        [Bindable(true)]
        public bool Entrusting
        {
            get;
            set;
        }


        private void initializePage()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(String.Format("var signableObj = '{0}';\r\n", VirtualPathUtility.ToAbsolute(Settings.Default.SignableObjectUrl)));
            sb.Append("function startSigning() {\r\n")
                .Append("__theFormPostData = '';WebForm_InitCallback();\r\n");
            if (_silentAsSigning)
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

        public string DataToSign
        {
            get;
            set;
        }

        public bool SilentAsSigning
        {
            get
            {
                return _silentAsSigning;
            }
            set
            {
                _silentAsSigning = value;
            }
        }

        public virtual bool SignAndCheck()
        {
            if (BeforeSign != null)
            {
                BeforeSign(this, new EventArgs());
            }

            return true;
        }

        public bool Verify()
        {
            if (!this.Visible)
            {
                return true;
            }
            else if (Entrusting)
            {
                return SignAndCheck();
            }
            else
            {
                if (DoVerify != null)
                {
                    X509Certificate cert;
                    if (DoVerify(Request["dataToSign"], Request["dataSignature"], out cert))
                    {
                        SignerCertificate = new X509Certificate2(cert);
                        return true;
                    }
                    return false;
                }
                else
                {
                    return doVerify();
                }
            }
        }

        public bool AutoSign { get; set; }
        public Encoding DataToSignEncoding { get; set; }

        public X509Certificate2 SignerCertificate { get; set; }

        protected virtual bool doVerify()
        {
            String dataSignature = Request["dataSignature"];
            String dataToSign = Request["dataToSign"];

            if (String.IsNullOrEmpty(dataSignature))
            {
                return false;
            }

            CryptoUtility ca = new CryptoUtility();

            if (IsCmsEnveloped)
            {
                byte[] data;
                if (ca.VerifyEnvelopedPKCS7(Convert.FromBase64String(DataSignature), out data))
                {
                    SignerCertificate = new X509Certificate2(ca.SignerCertificate);
                    //if(ValueValidity.IsSignificantString(PID))
                    //{
                    //    string subject = ca.CA_Log.Subject;
                    //    return subject.IndexOf(PID)>-1;
                    //}
                    return true;
                }
            }
            else
            {
                if (ca.VerifyPKCS7((DataToSignEncoding ?? Encoding.Default).GetBytes(dataToSign), Convert.FromBase64String(dataSignature)))
                {
                    SignerCertificate = new X509Certificate2(ca.SignerCertificate);

                    //if(ValueValidity.IsSignificantString(PID))
                    //{
                    //    string subject = ca.CA_Log.Subject;
                    //    return subject.IndexOf(PID)>-1;
                    //}

                    return true;
                }
            }
            return false;
        }


        protected void Page_CA_PreRender(object sender, EventArgs e)
        {
            initializePage();

            if (null != this._launcher && this._launcher.Visible && this.Visible)
            {
                if (!Entrusting)
                {
                    String signing = String.Format("document.all('{0}').onclick = startSigning;\r\n", _launcher.ClientID);
                    Page.ClientScript.RegisterStartupScript(typeof(SignContext), "doCA", signing, true);
                }

                StringBuilder sb = new StringBuilder();
                sb.Append("function afterSigned() {\r\n")
                    .Append(Page.ClientScript.GetPostBackEventReference(_launcher, ""))
                    .Append(";\r\n}\r\n");

                Page.ClientScript.RegisterClientScriptBlock(typeof(SignContext), "afterSigned", sb.ToString(), true);

                if (!this.IsPostBack)
                {
                    RegisterAutoSign();
                }

            }

        }

        public void RegisterAutoSign()
        {
            if (AutoSign)
            {
                if (ScriptManager.GetCurrent(Page) != null)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(SignContext), "autoSign",
                       "startSigning();", true);
                }
                else
                {
                    Page.ClientScript.RegisterStartupScript(typeof(SignContext), "autoSign",
                       "startSigning();", true);
                }
            }
        }

        #region ICallbackEventHandler жин√

        public virtual string GetCallbackResult()
        {
            return DataToSign != null ? DataToSign.Replace("\r\n", "\n").Replace("\n", "\r\n") : null;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            EventArgument = eventArgument;
            if (BeforeSign != null)
            {
                BeforeSign(this, new EventArgs());
            }
        }

        #endregion

        public String EventArgument
        {
            get;
            protected set;
        }
    }
}
