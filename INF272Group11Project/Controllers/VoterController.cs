using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INF272Group11Project.Controllers
{
    public class VoterController : Controller
    {
        // GET: Voter
        //The Index ActionResult is the main homepage for the project.
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FindVotingStation()
        {
            return View();
        }

        public ActionResult RegisterVoter()
        {
            return View();
        }

        public ActionResult VoterLogin()
        {
            return View();
        }

        public ActionResult VoterHomePage()
        {
            return View();
        }

        public ActionResult UpdateVoterInfo()
        {
            return View();
        }

        public ActionResult ChangePasswordVoter()
        {
            return View();
        }
    }
}