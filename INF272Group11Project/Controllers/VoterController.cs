using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;
using System.Security.Cryptography;
using System.Text;

namespace INF272Group11Project.Controllers
{
    
    public class VoterController : Controller
    {
        VotingSystemProjectEntities db = new VotingSystemProjectEntities();
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

            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
            ViewBag.CityOrTownID = new SelectList(db.CityOrTowns, "CityOrTownID", "CityOrTownName");
            ViewBag.SuburbID = new SelectList(db.Suburbs, "SuburbID", "SuburbName");
            ViewBag.SecurityQuestionID = new SelectList(db.SecurityQuestions, "SecurityQuestionID", "SecurityQuestion1");
            
            return View();
        }

        [HttpPost]
        public ActionResult AddNewVoter(string VoterIDNumber, string FirstName, string LastName, string Email, string PhoneNumber, [Bind(Include = "SuburbID, SecurityQuestionID")] Voter voter, string Address, string Password, string ConfirmPassword, string SecurityAnswer)
        {
            string EncrypVoterID = HashedData(VoterIDNumber);
            var searchID = db.Voters.Where(x => x.VoterIDNumber == EncrypVoterID).FirstOrDefault();

            if (searchID == null)
            {
                var SearchEmail = db.Voters.Where(j => j.VoterEmail == Email).FirstOrDefault();
                if(SearchEmail == null)
                {
                    var SearchPhone = db.Voters.Where(g => g.VoterPhoneNumber == PhoneNumber).FirstOrDefault();
                    if(SearchPhone == null)
                    {
                        string EncrypPassword1 = HashedData(Password);
                        string EncrypPassword2 = HashedData(ConfirmPassword);
                        if (EncrypPassword1 == EncrypPassword2)
                        {
                            string EncrypSecurityQuestionAnswer = HashedData(SecurityAnswer);
                            Voter v = new Voter();
                            v.VoterIDNumber = EncrypVoterID;
                            v.VoterPassword = EncrypPassword1;
                            v.VoterFirstNames = FirstName;
                            v.VoterLastName = LastName;
                            v.VoterEmail = Email;
                            v.VoterPhoneNumber = PhoneNumber;
                            v.VoterStreetAddress = Address;
                            v.VotingStatus = false;
                            v.SecurityQuestionID = voter.SecurityQuestionID;
                            v.SuburbID = voter.SuburbID;
                            v.SecurityQuestionAnswer = EncrypSecurityQuestionAnswer;
                            db.Voters.Add(v);
                            db.SaveChanges();
                            return RedirectToAction("VoterLogin");
                        }
                        else
                        {
                            RedirectToAction("RegisterVoter");
                        }
                    }
                    else
                    {
                        RedirectToAction("RegisterVoter");
                    }
                }
                else
                {
                    RedirectToAction("RegisterVoter");
                }
            }
            else
            {
                RedirectToAction("RegisterVoter");
            }
            return RedirectToAction("VoterLogin");
        }

        string HashedData(string placeholder)
        {
            using (SHA256 h = SHA256.Create())
            {
                byte[] b = h.ComputeHash(Encoding.UTF8.GetBytes(placeholder));

                StringBuilder builder = new StringBuilder();
                for(int i=0; i< b.Length; i++)
                {
                    builder.Append(b[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        
        public ActionResult VoterLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogVoterIn(string IDNumber, string Password)
        {
            string IDNum = HashedData(IDNumber);
            string Pass = HashedData(Password);

            Voter u = db.Voters.Where(x => x.VoterIDNumber == IDNum && x.VoterPassword == Pass).FirstOrDefault();
            if(u != null)
            {
                return RedirectToAction("VoterHomePage");
            }
            else
            {
                return RedirectToAction("VoterLogin");
            }
            
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