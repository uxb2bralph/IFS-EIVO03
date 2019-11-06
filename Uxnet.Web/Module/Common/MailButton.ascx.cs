namespace Uxnet.Web.Module.Common
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using System.Collections;

    using Utility;

    /// <summary>
    ///		DownloadingButton 的摘要描述。
    /// </summary>
    public partial class MailButton : System.Web.UI.UserControl
    {

        private static Hashtable _Hash = new Hashtable();
        private IList _controlList = new ArrayList();

        public event EventHandler BeforeClick;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // 在這裡放置使用者程式碼以初始化網頁
            this._btnDownload.OnClientClick = "return sendMail();";
        }

        public static IEnumerable GetMailControlsBySessionID(string sessionID)
        {
            object obj = _Hash[sessionID];
            return (obj != null) ? (IEnumerable)obj : null;
        }

        public static void RemoveMailControlsBySessionID(string sessionID)
        {
            _Hash.Remove(sessionID);
        }

        public IList MailControls
        {
            get
            {
                return _controlList;
            }
        }

        public string MailSubject
        {
            get
            {
                return _btnDownload.CommandArgument;
            }
            set
            {
                _btnDownload.CommandArgument = value;
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

        public string SmtpServer
        {
            get
            {
                return (string)this.ViewState["smtpServer"];
            }
            set
            {
                this.ViewState["smtpServer"] = value;
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

        }
        #endregion

        protected void _btnDownload_Click(object sender, System.EventArgs e)
        {
            if (BeforeClick != null)
            {
                BeforeClick(this, new EventArgs());
            }

            if (_controlList.Count > 0 && !String.IsNullOrEmpty(Request.Form["mailButtonAddr"]))
            {

                _Hash.Remove(Session.SessionID);
                _Hash.Add(Session.SessionID, _controlList);
                sendMail();
                Response.Redirect(Request.ApplicationPath + "/goBack.htm");
                //				Server.Transfer(Request.Path);
            }
        }

        private void sendMail()
        {
            if (!String.IsNullOrEmpty(ContentGeneratorUrl) && !String.IsNullOrEmpty(SmtpServer))
            {
                using (PageMailGenerator pageMail = new Utility.PageMailGenerator())
                {
                    pageMail.MailMessage = String.Format("CDS金融服務中心 {0}({0:yyyy/M/d})",
                        _btnDownload.CommandArgument, DateTime.Now);
                    pageMail.SMTP_Server = SmtpServer;
                    pageMail.From = "cds_service@uxb2b.com";
                    pageMail.Sender = "cds_service@uxb2b.com";

                    ArrayList al = new ArrayList();
                    foreach (string addr in Request.Form["mailButtonAddr"].Split(',', ';'))
                    {
                        al.Add(addr);
                    }

                    pageMail.Recipient = al;
                    pageMail.PageUrl = String.Format("{0}?sessionID={1}", ContentGeneratorUrl, Session.SessionID);
                    pageMail.Publish();

                }
            }

        }

    }
}
