namespace Uxnet.Web.Module.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.ComponentModel;
    using System.IO;
    using System.Text;

    /// <summary>
    ///		DownloadingButton 的摘要描述。
    /// </summary>
    public partial class DownloadingButton2 : System.Web.UI.UserControl
    {

        protected List<Control> _controlList;

        public event EventHandler BeforeClick;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            // 在這裡放置使用者程式碼以初始化網頁
            if (ScriptManager.GetCurrent(this.Page) != null)
            {
                ScriptManager.GetCurrent(this.Page).RegisterPostBackControl(this);
            }
        }

        public List<Control> DownloadControls
        {
            get
            {
                if (_controlList == null)
                    _controlList = new List<Control>();
                return _controlList;
            }
        }

        [Bindable(true)]
        public string ContentGeneratorUrl
        {
            get;
            set;
        }

        [Bindable(true)]
        public String OutputFileName
        {
            get;
            set;
        }

        [Bindable(true)]
        public String OnClientClick
        {
            get
            {
                return _btnDownload.OnClientClick;
            }
            set
            { 
                _btnDownload.OnClientClick=value;
            }
        }


        public Button TheButton
        {
            get
            {
                return _btnDownload;
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
                if (!String.IsNullOrEmpty(ContentGeneratorUrl))
                {
                    Response.Clear();
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.ContentType = "message/rfc822";
                    Response.AddHeader("Content-Disposition", !String.IsNullOrEmpty(OutputFileName) ? String.Format("attachment;filename={0}", Server.UrlEncode(OutputFileName))
                        : String.Format("attachment;filename={0:yyyy-MM-dd}.htm", DateTime.Today));

                    Page.Items["contentList"] = _controlList;

                    using (StreamWriter sw = new StreamWriter(Response.OutputStream, Encoding.UTF8))
                    {
                        Server.Execute(ContentGeneratorUrl, sw, true);
                        sw.Flush();
                    }

                    Response.End();
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
