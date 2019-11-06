namespace Uxnet.Web.Module.Common
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using Utility;

	/// <summary>
	///		ExceptionHandler 的摘要描述。
	/// </summary>
	public partial class ExceptionHandler : System.Web.UI.UserControl
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// 在這裡放置使用者程式碼以初始化網頁
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
			this.Page.Error += new EventHandler(ExceptionHandler_Error);
		}
		#endregion

		private void ExceptionHandler_Error(object sender, EventArgs e)
		{
			Exception ex = Server.GetLastError();
			if(!(ex is System.Threading.ThreadAbortException))
				Logger.Error(ex);
		}
	}
}
