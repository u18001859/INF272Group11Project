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
        RegisterVoterVM registerVoter = new RegisterVoterVM();
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
        public ActionResult AddNewVoter(string VoterIDNumber, string FirstName, string LastName, string Email, string PhoneNumber, [Bind(Include = "SuburbID, SecurityQuestionID")] Voter voter, string Address, string Password, string ConfrimPassword, string SecurityAnswer)
        {
            var EncrypVoterID = registerVoter.HashedData(VoterIDNumber);
            var searchID = db.Voters.Where(x => x.VoterIDNumber == EncrypVoterID).FirstOrDefault();

            if (searchID == null)
            {
                var SearchEmail = db.Voters.Where(j => j.VoterEmail == Email).FirstOrDefault();
                if(SearchEmail == null)
                {
                    var SearchPhone = db.Voters.Where(g => g.VoterPhoneNumber == PhoneNumber).FirstOrDefault();
                    if(SearchPhone == null)
                    {
                        var EncrypPassword1 = registerVoter.HashedData(Password);
                        var EncrypPassword2 = registerVoter.HashedData(ConfrimPassword);
                        var EncrypSecurityQuestionAnswer = registerVoter.HashedData(SecurityAnswer);
                        if (EncrypPassword1 == EncrypPassword2)
                        {
                            
                            Voter v = new Voter();
                            v.VoterIDNumber = EncrypVoterID.ToString();
                            v.VoterPassword = EncrypPassword1.ToString();
                            v.VoterFirstNames = FirstName.ToString();
                            v.VoterLastName = LastName.ToString();
                            v.VoterEmail = Email.ToString();
                            v.VoterPhoneNumber = PhoneNumber.ToString();
                            v.VoterStreetAddress = Address.ToString();
                            v.VotingStatus = false;
                            v.SecurityQuestionAnswer = EncrypSecurityQuestionAnswer.ToString();
                            v.SecurityQuestionID = voter.SecurityQuestionID;
                            v.SuburbID = voter.SuburbID;
                            
             
                            db.Voters.Add(v);
                            db.SaveChanges();
                            
                            
                            return RedirectToAction("VoterLogin");
                        }
                        else
                        {
                            ViewBag.message = "your passwords do not match";
                            RedirectToAction("RegisterVoter");
                        }
                    }
                    else
                    {
                        ViewBag.message = "The user already exist please login";
                        RedirectToAction("RegisterVoter");
                    }
                }
                else
                {
                    ViewBag.message = "The user already exist please login";
                    RedirectToAction("RegisterVoter");
                }
            }
            else
            {
                ViewBag.message = "The user already exist please login";
                RedirectToAction("RegisterVoter");
            }
            return RedirectToAction("VoterLogin");
        }

        

        
        public ActionResult VoterLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogVoterIn(string IDNumber, string Password)
        {
            var IDNum = registerVoter.HashedData(IDNumber);
            var Pass = registerVoter.HashedData(Password);

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

        [HttpPost]
        public ActionResult ForgotPassword(string IDnum)
        {
                var ids = IDnum;
                var hassed = registerVoter.HashedData(ids);
                var check = db.Voters.Where(x => x.VoterIDNumber == hassed).FirstOrDefault();
            if (check != null)
            {

                var question = check.SecurityQuestionID;
                if (question != null)
                {
                    var securityquestion = db.SecurityQuestions.Where(j => j.SecurityQuestionID == question).FirstOrDefault();
                    if (securityquestion != null)
                    {
                        var answer = securityquestion.SecurityQuestion1;
                        ViewBag.answer = answer;
                        ViewBag.ID = IDnum;
                    }
                }


            }
            else
            {
                ViewBag.message = "ID Number or Security Question Answer is incorrect";
                return RedirectToAction("VoterLogin");
            }
                return View();
        }
        [HttpPost]
        public ActionResult doForgotPasswordUpdate(string Answer, string NewPassword, string ConfirmNewPassword, string IDNumber)
        {
            using (VotingSystemProjectEntities db = new VotingSystemProjectEntities())
            {
                var ans = registerVoter.HashedData(Answer);
                var idnum = registerVoter.HashedData(IDNumber);
                var search = db.Voters.Where(x => x.VoterIDNumber == idnum && x.SecurityQuestionAnswer == ans).FirstOrDefault();
                if (search != null)
                {
                    if (NewPassword == ConfirmNewPassword)
                    {
                        string hassednew = registerVoter.HashedData(NewPassword);
                        string hassedcnew = registerVoter.HashedData(ConfirmNewPassword);
                        search.VoterPassword = hassednew;
                        db.SaveChanges();

                    }
                }
            }
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

        public ActionResult ForgotPasswordGetID()
        {
            return View();
        }
    }
}