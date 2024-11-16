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
using System.Web.Mvc;

namespace Uxnet.Web.Module.SiteAction
{
    public partial class SiteMenuBar : ViewUserControl
    {
        private bool _authorized = false;
        private XmlDocument _menuDoc;
        private string _menuDataPath;

        public string MenuDataPath
        {
            get { return _menuDataPath; }
            set { _menuDataPath = value; }
        }

        public bool AutoAddWorkItem { get; set; }

        public event EventHandler Logout;
        public event MenuEventHandler DataPathBound;

        private static UserMenuManager _MenuManager = new UserMenuManager(HttpContext.Current.Server.MapPath("~/resource"));

        protected void Page_Load(object sender, System.EventArgs e)
        {

        }

        public void BindData(string menuPath,string menuDataPath)
        {
            dsHandler.DataFile = String.Empty;

            _menuDoc = _MenuManager.GetMenuDocument(menuPath);
            _menuDataPath = menuDataPath;

            if (_menuDoc != null)
            {
                dsHandler.Data = _menuDoc.OuterXml;
            }
        }

        public void BindData(XmlDocument menuDoc, string menuDataPath)
        {
            dsHandler.DataFile = String.Empty;

            _menuDoc = menuDoc;
            _menuDataPath = menuDataPath;

            if (_menuDoc != null)
            {
                dsHandler.Data = _menuDoc.OuterXml;
            }
        }

        public static UserMenuManager MenuManager
        {
            get
            {
                return _MenuManager;
            }
        }


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu.MenuItemDataBound += new MenuEventHandler(mainMenu_MenuItemDataBound);
            this.mainMenu.PreRender += new EventHandler(mainMenu_PreRender);
        }

        #endregion
        protected void mainMenu_MenuItemDataBound(object sender, MenuEventArgs e)
        {
            if (!_authorized)
            {
                _authorized = e.Item.NavigateUrl.StartsWith(Request.AppRelativeCurrentExecutionFilePath);
                if (_authorized)
                {
                    _menuDataPath = e.Item.DataPath;
                    if (DataPathBound != null)
                    {
                        DataPathBound(this, e);
                    }
                }
            }
        }


        protected void mainMenu_PreRender(object sender, EventArgs e)
        {
            if (!_authorized)
            {
                if (_menuDoc != null)
                {
                    XmlNode node = _menuDataPath != null ? _menuDoc.SelectSingleNode(_menuDataPath) : _menuDoc.DocumentElement;
                    if (node != null)
                    {
                        XmlNodeList nodeList = node.SelectNodes(String.Format("workItem[@url='{0}']", Request.AppRelativeCurrentExecutionFilePath));
                        if (nodeList.Count > 0)
                        {
                            return;
                        }
                        else if (AutoAddWorkItem)
                        {
                            XmlElement workItem = _menuDoc.CreateElement("workItem");
                            node.AppendChild(workItem);
                            workItem.SetAttribute("value", "");
                            workItem.SetAttribute("url", Request.AppRelativeCurrentExecutionFilePath);

                            _MenuManager.Save(_menuDoc);
                            return;
                        }
                    }
                }

                if (Logout != null)
                {
                    Logout(this, new EventArgs());
                }
            }
        }
    }
}