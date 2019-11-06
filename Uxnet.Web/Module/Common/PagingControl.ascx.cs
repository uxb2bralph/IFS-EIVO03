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
using System.Linq;
using System.Collections.Generic;

using Uxnet.Web.Properties;
using Utility;

namespace Uxnet.Web.Module.Common
{
    public partial class PagingControl : System.Web.UI.UserControl, IPostBackDataHandler
    {
        protected const int __PAGING_SIZE = 10;
        private int _pageSize = Settings.Default.PageSize;
        private int _currentPageIndex = -1;
        private int _recordCount = 0;

        public event PageChangedEventHandler PageIndexChanged;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            // 在這裡放置使用者程式碼以初始化網頁
            //if (this.IsPostBack)
            //    OnPageIndexChanged(new PageChangedEventArgs(this, _currentPageIndex));
        }

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
                return _pageSize;
            }
            set
            {
                if (value > 0)
                {
                    _pageSize = value;
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
                return _recordCount;
            }
            set
            {
                if (value > 0)
                {
                    _recordCount = value;
                }
                else
                {
                    _recordCount = 0;
                }
            }
        }

        public int CurrentPageIndex
        {
            get
            {
                if (_currentPageIndex < 0 && _recordCount > 0)
                {
                    _currentPageIndex = 0;
                }
                return _currentPageIndex;
            }
            set
            {
                _currentPageIndex = (value >= 0) ? value : 0;
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

        protected virtual String renderPageIndex(int pageIndex)
        {
            return pageIndex != CurrentPageIndex ? String.Format("<a href=\"#\" onclick=\"document.all('{0}').value='{1}';{2}\">{3}</a>", PageNum.ClientID, pageIndex + 1, Page.ClientScript.GetPostBackEventReference(this, ""), pageIndex + 1) : String.Format("{0}", pageIndex + 1);
        }

        protected void PagingList_PreRender(object sender, EventArgs e)
        {
            lbnPrev.Visible = CurrentPageIndex >= __PAGING_SIZE;
            lbnPrev.Text = String.Format("上{0}頁", __PAGING_SIZE);
            lbnPrev.OnClientClick = String.Format("document.all(\"{0}\").value=\"{1}\";{2}", PageNum.ClientID, Math.Max(0, CurrentPageIndex - __PAGING_SIZE) + 1, Page.ClientScript.GetPostBackEventReference(this, ""));
            lbnNext.Visible = (PageCount - ((int)(CurrentPageIndex / __PAGING_SIZE)) * __PAGING_SIZE) > __PAGING_SIZE;
            lbnNext.Text = String.Format("下{0}頁", __PAGING_SIZE);
            lbnNext.OnClientClick = String.Format("document.all(\"{0}\").value=\"{1}\";{2}", PageNum.ClientID, Math.Min(PageCount - 1, CurrentPageIndex + __PAGING_SIZE) + 1, Page.ClientScript.GetPostBackEventReference(this, ""));

            PageNum.Text = _currentPageIndex >= 0 ? (_currentPageIndex + 1).ToString() : "";
            bindPaging();
        }

        private void bindPaging()
        {
            if (PageCount > 0)
            {
                if (_currentPageIndex >= PageCount)
                {
                    _currentPageIndex = PageCount - 1;
                }
                else if (_currentPageIndex < 0)
                {
                    _currentPageIndex = 0;
                }
            }
            else
            {
                _currentPageIndex = -1;
            }

            lblSummary.Text = String.Format("總筆數：{0} &nbsp;&nbsp;&nbsp;總頁數：{1}", RecordCount, PageCount);
            lblSummary.Visible = true;

            int startIndex = _currentPageIndex >= 0 ? _currentPageIndex / __PAGING_SIZE * __PAGING_SIZE : 0;

            rpList.DataSource = startIndex.GenerateArray(Math.Min(PageCount - startIndex, __PAGING_SIZE));
            rpList.DataBind();
        }

        public static int GetCurrentPageIndex(Control owner, int defaultIndex)
        {
            int currIndex = defaultIndex;
            String pName = owner.Page.Request.Form.AllKeys.Where(k => k != null && k.StartsWith(owner.UniqueID) && k.EndsWith("PageNum")).FirstOrDefault();
            if (!String.IsNullOrEmpty(pName))
            {
                int.TryParse(owner.Page.Request[pName], out currIndex);
                currIndex--;
            }
            return currIndex;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            Page.ClientScript.RegisterForEventValidation(this.UniqueID, "");
            base.Render(writer);
            writer.Write("<input type=\"hidden\" name=\"{0}\" value=\"{1};{2};{3}\"/>", this.UniqueID, _pageSize, _recordCount,_currentPageIndex);
        }

        #region IPostBackDataHandler Members

        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            int lastPageIndex = -1;
            if (!String.IsNullOrEmpty(postCollection[postDataKey]))
            {
                String[] values = postCollection[postDataKey].Split(';');
                int.TryParse(values[0], out _pageSize);
                int.TryParse(values[1], out _recordCount);
                int.TryParse(values[2], out lastPageIndex);
            }
            else
            {
                return false;
            }

            if (!String.IsNullOrEmpty(postCollection[PageNum.UniqueID]))
            {
                if (int.TryParse(postCollection[PageNum.UniqueID], out _currentPageIndex))
                {
                    _currentPageIndex--;
                }
            }

            return _currentPageIndex != lastPageIndex;
        }

        public void RaisePostDataChangedEvent()
        {
            OnPageIndexChanged(new PageChangedEventArgs(this, _currentPageIndex));
        }

        #endregion
    }
}