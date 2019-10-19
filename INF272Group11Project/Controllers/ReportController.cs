using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//Apparently I need help with this one according to Seb so Im going to leave it for now
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

        public ActionResult PrintReport()
        {
            return View();
        }

        public FileResult GetProvinceReport()
        {
            string URL = "C:\\Users\\sebas\\Source\\Repos\\INF272Group11Project9\\INF272Group11Project\\Provincial Results.pdf";
            byte[] ReadFile = System.IO.File.ReadAllBytes(URL);
            return File(ReadFile, "application/pdf");
        }
        public FileResult GetNationalReport()
        {
            string URL = "C:\\Users\\sebas\\Source\\Repos\\INF272Group11Project9\\INF272Group11Project\\National Results.pdf";
            byte[] ReadFile = System.IO.File.ReadAllBytes(URL);
            return File(ReadFile, "application/pdf");
        }
    }
}