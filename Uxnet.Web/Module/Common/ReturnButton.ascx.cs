namespace Uxnet.Web.Module.Common
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;
using System.ComponentModel;

	/// <summary>
	///		ReturnButton 的摘要描述。
	/// </summary>
	public partial class ReturnButton : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// 在這裡放置使用者程式碼以初始化網頁
		}

        [Bindable(true)]
        public bool UseReferrer
        {
            get;
            set;
        }

        [Bindable(true)]
        public String GoBackUrl
        {
            get
            {
                return btnGoBack.CommandArgument;
            }
            set
            {
                btnGoBack.CommandArgument = value;
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
			this.PreRender += new EventHandler(ReturnButton_PreRender);

		}
		#endregion

		private void ReturnButton_PreRender(object sender, EventArgs e)
		{
            if (UseReferrer)
			{
                if (!String.IsNullOrEmpty(GoBackUrl))
                {
                    btnGoBack.OnClientClick = String.Format("window.location.href = '{0}';", GoBackUrl);
                }
                else if (null != Request.UrlReferrer)
                {
                    btnGoBack.OnClientClick = String.Format("window.location.href = '{0}';", Request.UrlReferrer.AbsolutePath);
                }
                else
                {
                    btnGoBack.OnClientClick = "window.history.go(-1);";
                }
			}
			else
			{
                btnGoBack.OnClientClick = "window.history.go(-1);";
            }
		}
	}
}
