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

        public ActionResult AddNewVoter()
        {
            return RedirectToAction("VoterLogin");
        }

        public ActionResult VoterLogin()
        {
            return View();
        }

        public ActionResult LogVoterIn()
        {
            return RedirectToAction("VoterHomePage");
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

        public ActionResult Logout()
        {
            return RedirectToAction("Index");
        }

        public ActionResult doVoterUpdate()
        {
            return RedirectToAction("VoterHomePage");
        }

        public ActionResult doUpdatePassword()
        {
            return RedirectToAction("VoterHomePage");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        public ActionResult doForgotPasswordUpdate()
        {
            return RedirectToAction("VoterLogin");
        }

        public ActionResult VoteParty()
        {
            return View();
        }

        public ActionResult doVoteParty()
        {
            return RedirectToAction("VoteProvincial");
        }

        public ActionResult VoteProvincial()
        {
            return View();
        }

        public ActionResult doVoteProvincial()
        {
            return RedirectToAction("ThankYou");
        }

        public ActionResult ThankYou()
        {
            return View();
        }
    }
}