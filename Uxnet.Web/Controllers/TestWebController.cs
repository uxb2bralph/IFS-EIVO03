using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Uxnet.Web.Controllers
{
    public class TestWebController : Controller
    {
        // GET: TestWeb
        public ActionResult Index()
        {
            return Content("Hello, Test Web!!");
        }
    }
}