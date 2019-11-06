namespace Uxnet.Web.Module.Common
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
	using System.Collections;

	/// <summary>
	///		DownloadingButton 的摘要描述。
	/// </summary>
	public partial class DownloadingButton : System.Web.UI.UserControl
	{

		protected static Hashtable _Hash = new Hashtable();
		protected	IList _controlList = new ArrayList();

        public event EventHandler BeforeClick;


		protected void Page_Load(object sender, System.EventArgs e)
		{
			// 在這裡放置使用者程式碼以初始化網頁
		}

		public static IEnumerable GetDownloadControlsBySessionID(string sessionID)
		{
			object obj = _Hash[sessionID];
            return (obj!=null)?(IEnumerable)obj:null;
		}

		public static void RemoveDownloadControlsBySessionID(string sessionID)
		{
			_Hash.Remove(sessionID);
		}

		public IList DownloadControls
		{
			get
			{
				return _controlList;
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

		}
		#endregion

        protected virtual void _btnDownload_Click(object sender, System.EventArgs e)
        {
            OnBeforeClick();

            if (_controlList.Count > 0)
            {

                _Hash.Remove(Session.SessionID);
                _Hash.Add(Session.SessionID, _controlList);
                if (!String.IsNullOrEmpty(ContentGeneratorUrl))
                {
                    Server.Transfer(ContentGeneratorUrl);
                }
            }
        }

        protected void OnBeforeClick()
        {
            if (BeforeClick != null)
            {
                BeforeClick(this, new EventArgs());
            }
        }
	}
}
