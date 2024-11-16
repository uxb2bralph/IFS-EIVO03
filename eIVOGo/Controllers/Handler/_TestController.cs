using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Security.MembershipManagement;
using ModelExtension.Helper;
using Utility;
using DataAccessLayer;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace eIVOGo.Controllers.Handler
{
    public class _TestController : SampleController<InvoiceItem>
    {
        // GET: _Testing
        protected override void HandleUnknownAction(string actionName)
        {
            if (!String.IsNullOrEmpty(actionName))
            {
                this.View(actionName).ExecuteResult(this.ControllerContext);
            }
        }

        public ActionResult Test01()
        {
            NameValueCollection formValues = new NameValueCollection(Request.Form);
            formValues.Add("test01", "hello...");
            return Content(formValues.ToString());
        }

        public ActionResult ReloadSettings()
        {
            AppSettings.Reload();
            Model.Properties.AppSettings.Reload();
            return Content(AppSettings.AllSettings.JsonStringify(), "application/json");
        }

        public ActionResult SaveSettings()
        {
            //AppSettings.SaveAll();
            AppSettings.Default.Save();
            return Content(AppSettings.AllSettings?.ToString(), "application/json");
        }

        public ActionResult AllSettings()
        {
            return Content(AppSettings.AllSettings?.ToString(), "application/json");
        }

    }
}