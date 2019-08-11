using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INF272Group11Project.Controllers
{
    public class ResultsController : Controller
    {
        // GET: Results
        public ActionResult TotalVotes()
        {
            return View();
        }

        public ActionResult NationalResults()
        {
            return View();
        }

        public ActionResult ProvincialResults(string DropdownList)
        {
            //dropdownlist selection reloads form with results by province
            return View();
        }
    }
}