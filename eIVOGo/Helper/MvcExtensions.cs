using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

using DataAccessLayer.basis;

using eIVOGo.Properties;
using MessagingToolkit.QRCode.Codec;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.Controllers;

namespace eIVOGo.Helper
{
    public static class MvcExtensions
    {
        public static GenericManager<EIVOEntityDataContext> DataSource(this ControllerBase controller)
        {
            return ((SampleController<EIVOEntityDataContext>)controller).DataSourceBase;
        }

        public static void RenderJsonResult(this ViewUserControl control, object data)
        {
            JsonResult result = new JsonResult()
            {
                ContentType = "application/json",
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
            };
            result.ExecuteResult(control.ViewContext.Controller.ControllerContext);
            control.Response.End();
        }

        public static void RenderJsonResult(this HttpResponse response, object data)
        {
            response.Clear();
            response.ContentType = "application/json";
            response.Write(data.JsonStringify());
            response.End();
        }

        public static void RenderJsonResult(this HttpResponseBase response, object data)
        {
            response.Clear();
            response.ContentType = "application/json";
            response.Write(data.JsonStringify());
            response.End();
        }
    }
}