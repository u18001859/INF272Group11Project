using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INF272Group11Project.Controllers
{
    public class StaffController : Controller
    {
        // GET: Staff
        public ActionResult StaffHomePage()
        {
            return View();
        }

        public ActionResult StaffLogin()
        {
            return View();
        }
        public ActionResult LogStaffIn()
        {
            return RedirectToAction("StaffHomePage");
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult doForgotPasswordUpdate()
        {
            return RedirectToAction("StaffLogin");
        }

        public ActionResult ChangePasswordStaff()
        {
            return View();
        }

        public ActionResult doUpdatePassword()
        {
            return RedirectToAction("StaffHomePage");
        }

        public ActionResult RegisterStaff()
        {
            return View();
        }

        public ActionResult AddNewStaff()
        {
            return RedirectToAction("StaffHomePage");
        }

        public ActionResult UpdateStaffInfo()
        {
            return View();
        }

        public ActionResult doStaffUpdate()
        {
            return RedirectToAction("StaffHomePage");
        }

        public ActionResult RegisterParty()
        {
            return View();
        }

        public ActionResult AddNewParty()
        {
            return RedirectToAction("PartyImages");
        }

        public ActionResult PartyImages()
        {
            return View();
        }

        public ActionResult AddImages()
        {
            return RedirectToAction("StaffHomePage");
        }
        public ActionResult UpdateOrDeleteParty()
        {
            return View();
        }
        public ActionResult UpateParty()
        {
            return View();
        }
        public ActionResult UpdatePartyImages()
        {
            return View();
        }

        public ActionResult DoUpdateImages()
        {
            return RedirectToAction("StaffHomePage");
        }
    }
}