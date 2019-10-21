using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;

namespace INF272Group11Project.Controllers
{
    public class ResultsController : Controller
    {
        VotingSystemProjectEntities3 Resultsdb = new VotingSystemProjectEntities3();
        NationalResultsVM results = new NationalResultsVM();
        // GET: Results
        VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2();
        public ActionResult TotalVotes(string StaffGUID, string id)
        {

            if(StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    TotalResultsVM totalResults = new TotalResultsVM();
                    totalResults.StaffView = staffGUID;
                    totalResults.ListElection = db.Elections.ToList();
                    return View(totalResults);
                }
                else
                {
                    TempData["message"] = "Your Session Has Expired Please Login Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }

           
        }

        public ActionResult NationalResults()
        {
            ViewBag.PartyID = new SelectList(Resultsdb.Parties, "PartyID", "PartyName");
            ViewBag.CandidateID = new SelectList(Resultsdb.Candidates, "CandidateID", "CandidateLastName");
            ViewBag.NationalResultsID = new SelectList(Resultsdb.NationalResults, "NationalResultsID", "NationalResultsID");
            ViewBag.NationalResultsTotalVotes = new SelectList(Resultsdb.NationalResults, "NationalResultsTotalVotes", "NationalResultsTotalVotes");
            return View();
        }

        public ActionResult ProvincialResults()
        {

            ViewBag.ElectionID = new SelectList(Resultsdb.Elections, "ElectionID", "ElectionID");
            ViewBag.PartyID = new SelectList(Resultsdb.Parties, "PartyID", "PartyName");
            ViewBag.ProvinceID = new SelectList(Resultsdb.Provinces, "ProvinceID", "ProvinceName");
            ViewBag.ProvincialResultsID = new SelectList(Resultsdb.ProvincialResults, "ProvincialResultsID", "ProvincialResultsID");
            ViewBag.ProvincialResultsTotalVotes = new SelectList(Resultsdb.ProvincialResults, "ProvincialResultsTotalVotes", "ProvincialResultsTotalVotes");

            return View();
        }
    }
}