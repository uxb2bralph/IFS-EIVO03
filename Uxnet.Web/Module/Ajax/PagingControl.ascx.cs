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
using System.ComponentModel;

namespace Uxnet.Web.Module.Ajax
{
    public partial class PagingControl : System.Web.UI.UserControl
    {
        protected const int __PAGING_SIZE = 10;
        protected int _pageSize = Settings.Default.PageSize;
        protected int _currentPageIndex = 0;
        protected int _recordCount = 0;

        protected void Page_Load(object sender, System.EventArgs e)
        {

        }

        [Bindable(true)]
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

        [Bindable(true)]
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

        [Bindable(true)]
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
    }
}