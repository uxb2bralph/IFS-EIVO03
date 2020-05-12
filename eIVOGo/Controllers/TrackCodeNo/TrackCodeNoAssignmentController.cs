using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Business.Helper;
using ClosedXML.Excel;
using eIVOGo.Helper;
using eIVOGo.Models;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;
using eIVOGo.Properties;
using Model.DataEntity;
using Model.Locale;
using Model.Schema.TurnKey.E0402;
using Model.Security.MembershipManagement;
using Utility;

namespace eIVOGo.Controllers.TrackCodeNo
{
    [Authorize]
    public class TrackCodeNoAssignmentController : SampleController<InvoiceItem>
    {
        // GET: TrackCodeNoAssignment
        public ActionResult Index()
        {
            return View();
        }
    }
}