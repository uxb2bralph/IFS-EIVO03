﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Business.Helper;
using eIVOGo.Helper;
using eIVOGo.Models.ViewModel;
using Model.Models.ViewModel;

using eIVOGo.Properties;
using Model.DataEntity;
using Model.DocumentManagement;
using Model.Helper;
using Model.InvoiceManagement;
using Model.Locale;
using Model.Schema.EIVO.B2B;
using Model.Schema.TurnKey;
using Model.Schema.TXN;
using Model.Security.MembershipManagement;
using Utility;
using Uxnet.Com.Security.UseCrypto;
using Newtonsoft.Json;
using System.Data;
using ModelExtension.Helper;
using DataAccessLayer;
using DataAccessLayer.basis;


namespace eIVOGo.Controllers.SAM
{
    public class SystemExceptionLogController : SampleController<InvoiceItem>
    {
        // GET: SystemExceptionLog
        public ActionResult Index()
        {
            return View();
        }
    }
}