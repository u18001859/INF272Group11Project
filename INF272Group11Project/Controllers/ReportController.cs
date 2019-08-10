using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INF272Group11Project.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProvincialReport()
        {
            return View();
        }

        public ActionResult MakeProvincialReport()
        {
            return View();
        }

        public ActionResult NationalReport()
        {
            return View();
        }

        public ActionResult MakeNationalReport()
        {
            return View();
        }
    }
}