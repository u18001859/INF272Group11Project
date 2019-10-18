using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INF272Group11Project.Controllers
{
    public class ResultsController : Controller
    {
        VotingSystemProjectEntities2 Resultsdb = new VotingSystemProjectEntities2();
        ResultsVM results = new ResultsVM();
        // GET: Results
        public ActionResult TotalVotes()
        {
            
            //Theres supposed to be stuff here but my brain is literal garbage rn and I cant figure it out
            return View();
        }

        public ActionResult NationalResults()
        {
            ViewBag.PartyID = new SelectList(Resultsdb.Parties, "PartyID", "PartyName");
            ViewBag.CandidateID = new SelectList(Resultsdb.Candidates, "CandidateID", "CandidateFirstNames", "CandidateLastName");
            ViewBag.NationalResultsID = new SelectList(Resultsdb.NationalResults, "NationalResultsID", "NationalResultsID");
            ViewBag.NationalResultsTotalVotes = new SelectList(Resultsdb.NationalResults, "NationalResultsTotalVotes", "NationalResultsTotalVotes");
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