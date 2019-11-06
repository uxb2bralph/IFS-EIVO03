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
using System.Xml;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Uxnet.Web.Module.Common
{
    public partial class WebPageTreeView : System.Web.UI.UserControl
    {
        private Regex _reg;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                initializeData();
            }

        }

        public TreeView PageTree
        {
            get
            {
                return pageTree;
            }
        }

        public string SearchPattern
        {
            get
            {
                return (string)this.ViewState["sp"];
            }
            set
            {
                this.ViewState["sp"] = value;
            }
        }

        private void initializeData()
        {
            if (!String.IsNullOrEmpty(SearchPattern))
            {
                _reg = new Regex(SearchPattern);
            }

            StringBuilder webDoc = new StringBuilder();
            webDoc.Append("<web>\r\n");
            buildWebPageTree(webDoc, HttpRuntime.AppDomainAppPath);
            webDoc.Append("</web>");

            dsPageTree.DataFile = String.Empty;
            dsPageTree.Data = webDoc.ToString();
        }

        private bool buildWebPageTree(StringBuilder root, string path)
        {
            bool hasFiles = false;
            int length;

            foreach (string dir in Directory.GetDirectories(path))
            {
                length = root.Length;
                root.Append(String.Format("\t<directory name=\"{0}\">\r\n",Path.GetFileName(dir)));
                hasFiles = buildWebPageTree(root, dir);
                root.Append("</directory>");
                if (!hasFiles)
                {
                    root.Remove(length, root.Length - length);
                }
            }

            length = root.Length;
            foreach (string fileName in Directory.GetFiles(path))
            {
                if (_reg == null || _reg.IsMatch(fileName))
                {
                    root.Append(String.Format("\t<file name=\"{0}\"/>\r\n", Path.GetFileName(fileName)));
                }
            }

            return root.Length>length;
        }
    }
}