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
using eIVOGo.Module.Common;
using eIVOGo.Module.UI;
using eIVOGo.Properties;
using eIVOGo.template;
using MessagingToolkit.QRCode.Codec;
using Model.DataEntity;
using Model.Locale;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Web.Controllers;
using Uxnet.Web.WebUI;

namespace eIVOGo.Helper
{
    public static class MvcExtensions
    {
        public static GenericManager<EIVOEntityDataContext> DataSource(this ControllerBase controller)
        {
            return ((SampleController<EIVOEntityDataContext>)controller).DataSourceBase;
        }
    }
}