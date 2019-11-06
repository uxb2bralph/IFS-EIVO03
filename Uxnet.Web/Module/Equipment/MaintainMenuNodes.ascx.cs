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
using Uxnet.Com.Providers;
using System.IO;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Uxnet.Web.Module.Equipment
{
    public partial class MaintainMenuNodes : System.Web.UI.UserControl
    {
        private static UserMenuManager _MenuManager ;
        private XmlDocument _menuDoc;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (_MenuManager == null)
            {
                if (!String.IsNullOrEmpty(ResourcePath))
                {
                    lock (typeof(MaintainMenuNodes))
                    {
                        if (_MenuManager == null)
                        {
                            _MenuManager = new UserMenuManager(ResourcePath);
                        }
                    }
                }
            }

            if (!this.IsPostBack)
            {
                initializeData();
            }

            if (Session["menuDoc"] != null)
            {
                _menuDoc = new XmlDocument();
                _menuDoc.LoadXml((String)Session["menuDoc"]);
            }

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.webPageTree.PageTree.SelectedNodeChanged += new EventHandler(PageTree_SelectedNodeChanged);
        }

        void PageTree_SelectedNodeChanged(object sender, EventArgs e)
        {
            menuUrl.Text = "~" + webPageTree.PageTree.SelectedNode.ValuePath;
        }


        private void initializeData()
        {
            this.dsMenu.DataFile = String.Empty;

            if (!String.IsNullOrEmpty(SiteMenuName))
            {
                XmlDocument menuDoc = _MenuManager.GetMenuDocument(SiteMenuName);
                if (menuDoc != null)
                {
                    menuDoc = (XmlDocument)menuDoc.Clone();
                    Session["menuDoc"] = menuDoc.OuterXml;
                    dsMenu.Data = menuDoc.OuterXml;
                }
            }
        }

        protected void btnDeleteNode_Click(object sender, EventArgs e)
        {
            if (menuTree.CheckedNodes.Count > 0)
            {
                List<XmlNode> nodes = new List<XmlNode>();

                foreach (TreeNode node in menuTree.CheckedNodes)
                {
                    XmlNode menuNode = _menuDoc.SelectSingleNode(node.DataPath);
                    if (menuNode != null)
                        nodes.Add(menuNode);
                }

                foreach (XmlNode menuNode in nodes)
                    menuNode.ParentNode.RemoveChild(menuNode);

                dsMenu.Data = _menuDoc.OuterXml;
            }
        }

        protected void menuTree_SelectedNodeChanged(object sender, EventArgs e)
        {
            TreeNode node = menuTree.SelectedNode;
            if (node != null)
            {
                this.menuName.Text = node.Text;
                XmlNode menuNode = _menuDoc.SelectSingleNode(node.DataPath);
                this.menuUrl.Text = menuNode.Attributes["url"].Value;
            }
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            TreeNode node = menuTree.SelectedNode;
            if (node != null)
            {
                XmlNode menuNode = _menuDoc.SelectSingleNode(node.DataPath);
                menuNode.Attributes["value"].Value = menuName.Text;
                menuNode.Attributes["url"].Value = menuUrl.Text;

                dsMenu.Data = _menuDoc.OuterXml;
            }
        }

        protected void btnInsert_Click(object sender, EventArgs e)
        {
            TreeNode node = menuTree.SelectedNode;
            if (node == null)
            {
                node = menuTree.Nodes[0];
            }

            XmlNode menuNode = _menuDoc.SelectSingleNode(node.DataPath);
            XmlNode newNode;
            if (rbMenuItem.Checked || rbRoot.Checked)
            {
                newNode = _menuDoc.CreateElement("menuItem");
            }
            else
            {
                newNode = _menuDoc.CreateElement("workItem");
            }
            XmlAttribute attr = _menuDoc.CreateAttribute("value");
            attr.Value = menuName.Text;
            newNode.Attributes.Append(attr);
            attr = _menuDoc.CreateAttribute("url");
            attr.Value = menuUrl.Text;
            newNode.Attributes.Append(attr);
            menuNode.AppendChild(newNode);

            dsMenu.Data = _menuDoc.OuterXml;

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            _MenuManager.Save(SiteMenuName, _menuDoc);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            initializeData();
        }

        public String SiteMenuName
        {
            get
            {
                return (string)this.ViewState["menuName"];
            }
            set
            {
                this.ViewState["menuName"] = value;
            }
        }

        public String ResourcePath
        {
            get
            {
                return (string)this.ViewState["resourcePath"];
            }
            set
            {
                this.ViewState["resourcePath"] = value;
            }
        }

    }
}