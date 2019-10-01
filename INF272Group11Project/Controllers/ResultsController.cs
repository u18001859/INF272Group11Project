using INF272Group11Project.Models;
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
            VotingSystemProjectEntities2 Resultsdb = new VotingSystemProjectEntities2();
            List<Election> electionList = Resultsdb.Elections.ToList();
            //Theres supposed to be stuff here but my brain is literal garbage rn and I cant figure it out
            return View();
        }

        public ActionResult NationalResults()
        {
            //Im gonna start working on this tomorrow
            return View();
        }

        public ActionResult ProvincialResults(string DropdownList)
        {
            //dropdownlist selection reloads form with results by province
            return View();
            //same with this one
        }

        public void DBTotalVotes() {
            VotingSystemProjectEntities2 Resultsdb = new VotingSystemProjectEntities2();
            List <Election> elections = Resultsdb.Elections.ToList();
            for (int i = 0; i < elections.Count; i++)
            {
                ViewBag.Message = elections.ElementAt(i) + "\n";
            }
        }
    }
}