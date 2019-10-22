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

        public ActionResult NationalResults(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    NationalResultsVM NationalResults = new NationalResultsVM();
                    NationalResults.StaffView = staffGUID;
                    NationalResults.Results = db.NationalResults.ToList();
                    return View(NationalResults);
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

        public ActionResult ProvincialResults(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    ProvincialResultsVM NationalResults = new ProvincialResultsVM();
                    NationalResults.StaffView = staffGUID;
                    NationalResults.Results = db.ProvincialResults.ToList();
                    return View(NationalResults);
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
    }
}