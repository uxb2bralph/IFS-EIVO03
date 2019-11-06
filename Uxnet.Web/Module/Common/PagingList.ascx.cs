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
using Uxnet.Web.Properties;

namespace Uxnet.Web.Module.Common
{
    public partial class PagingList : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // 在這裡放置使用者程式碼以初始化網頁

        }


        public event PageChangedEventHandler PageIndexChanged;

        protected virtual void OnPageIndexChanged(PageChangedEventArgs e)
        {
            if (PageIndexChanged != null)
            {
                PageIndexChanged(this, e);
            }
        }

        public int PageSize
        {
            get
            {
                return ViewState["size"] != null ? (int)ViewState["size"] : Settings.Default.PageSize;
            }
            set
            {
                if (value > 0)
                {
                    ViewState["size"] = value;
                }
            }
        }

        public int PageCount
        {
            get
            {
                return (RecordCount + PageSize - 1) / PageSize;
            }
        }

        public int RecordCount
        {
            get
            {
                return ViewState["recordCount"] != null ? (int)ViewState["recordCount"] : 0;
            }
            set
            {
                ViewState["recordCount"] = value >= 0 ? value : 0;
                bindPaging();
            }
        }

        public int CurrentPageIndex
        {
            get
            {
                return dlPage.SelectedIndex;
            }
            set
            {
                if (dlPage.SelectedIndex >= 0)
                    dlPage.SelectedIndex = (value >= 0) ? value : 0;
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
            this.PreRender += new System.EventHandler(this.PagingList_PreRender);

        }
        #endregion

        protected void PagingList_PreRender(object sender, EventArgs e)
        {
            lbnPrev.Visible = dlPage.SelectedIndex > 0;
            lbnNext.Visible = dlPage.SelectedIndex < dlPage.Items.Count - 1;
        }

        private void bindPaging()
        {
            //1.先保留目前的頁次資料
            int currentPageIndex = dlPage.SelectedIndex < PageCount ? dlPage.SelectedIndex : PageCount - 1;

            //2.重設頁碼;
            dlPage.Items.Clear();

            if (PageCount > 0)
            {
                for (int i = 0; i < PageCount; i++)
                {
                    dlPage.Items.Add(new ListItem(String.Format("第{0}頁", i + 1), i.ToString()));
                }

                dlPage.SelectedIndex = currentPageIndex >= 0 ? currentPageIndex : 0;
                dlPage.Visible = true;

                lblSummary.Text = String.Format(",共{0}頁,{1}筆", PageCount, RecordCount);
                lblSummary.Visible = true;

            }
            else
            {
                lblSummary.Visible = false;
                lbnPrev.Visible = false;
                lbnNext.Visible = false;
                dlPage.Visible = false;
            }
        }

        protected void lbnPrev_Click(object sender, System.EventArgs e)
        {
            dlPage.SelectedIndex--;
            OnPageIndexChanged(new PageChangedEventArgs(sender, dlPage.SelectedIndex));
        }

        protected void lbnNext_Click(object sender, System.EventArgs e)
        {
            dlPage.SelectedIndex++;
            OnPageIndexChanged(new PageChangedEventArgs(sender, dlPage.SelectedIndex));
        }

        protected void dlPage_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            OnPageIndexChanged(new PageChangedEventArgs(sender, dlPage.SelectedIndex));
        }
    }

    public delegate void PageChangedEventHandler(object source, PageChangedEventArgs e);

    public sealed class PageChangedEventArgs : EventArgs
    {
        private int _pageIndex;
        private object _cmdSource;

        public int NewPageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                _pageIndex = value;
            }
        }

        public object CommandSource
        {
            get
            {
                return _cmdSource;
            }
            set
            {
                _cmdSource = value;
            }
        }

        public PageChangedEventArgs(object commandSource, int newPageIndex)
        {
            _cmdSource = commandSource;
            _pageIndex = newPageIndex;
        }

    }

}