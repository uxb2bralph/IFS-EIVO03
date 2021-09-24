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
using Uxnet.Com.DataAccessLayer;

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
    }
}