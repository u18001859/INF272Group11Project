using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.Data.Entity;

namespace INF272Group11Project.Controllers
{
    
    public class VoterController : Controller
    {
        VotingSystemProjectEntities3 db = new VotingSystemProjectEntities3();
        RegisterVoterVM registerVoter = new RegisterVoterVM();
        // GET: Voter
        //The Index ActionResult is the main homepage for the project.
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FindVotingStation()
        {
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];
            FindVotingStationViewModel find = new FindVotingStationViewModel();
            find.FindVotingStation = TempData["VotingStation"] as VotingStation;

            return View(find);
        }

        public ActionResult RegisterVoter()
        {
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];
            ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
            ViewBag.CityOrTownID = new SelectList(db.CityOrTowns, "CityOrTownID", "CityOrTownName");
            ViewBag.SuburbID = new SelectList(db.Suburbs, "SuburbID", "SuburbName");
            ViewBag.SecurityQuestionID = new SelectList(db.SecurityQuestions, "SecurityQuestionID", "SecurityQuestion1");
            
            return View();
        }

        [HttpPost]
        public ActionResult AddNewVoter(string VoterIDNumber, string FirstName, string LastName, string Email, string PhoneNumber, [Bind(Include = "SuburbID, ProvinceID, CityOrTownID, SecurityQuestionID")] Voter voter, string Address, string Password, string ConfrimPassword, string SecurityAnswer)
        {
            if (VoterIDNumber != null)
            {

                var EncrypVoterID = registerVoter.HashedData(VoterIDNumber);
                var searchID = db.Voters.Where(x => x.VoterIDNumber == EncrypVoterID).FirstOrDefault();

                if (searchID == null)
                {
                    var SearchEmail = db.Voters.Where(j => j.VoterEmail == Email).FirstOrDefault();
                    if (SearchEmail == null)
                    {
                        var SearchPhone = db.Voters.Where(g => g.VoterPhoneNumber == PhoneNumber).FirstOrDefault();
                        if (SearchPhone == null)
                        {
                            if (PhoneNumber.Length == 10)
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
                                    v.VotePartyStatus = false;
                                    v.VotePartyStatus = false;
                                    v.SecurityQuestionAnswer = EncrypSecurityQuestionAnswer.ToString();
                                    v.SecurityQuestionID = voter.SecurityQuestionID;
                                    v.SuburbID = voter.SuburbID;
                                    v.ProvinceID = voter.ProvinceID;
                                    v.CityorTownID = voter.CityorTownID;
                                    v.GUID = Guid.NewGuid().ToString();
                                    v.GUIDTimeStamp = DateTime.Now;

                                    db.Voters.Add(v);
                                    db.SaveChanges();

                                    TempData["success"] = "Registration was successfull!";
                                    return RedirectToAction("VoterLogin");
                                }
                                else
                                {
                                    TempData["message"] = "your passwords do not match Please try again";
                                    RedirectToAction("RegisterVoter");
                                }
                            }
                            else
                            {
                                TempData["message"] = "The Phone Number is not the right length!";
                                RedirectToAction("RegisterVoter");
                            }
                        }
                        else
                        {
                            TempData["message"] = "The Phone Number is already in use please use a different one.";
                            RedirectToAction("RegisterVoter");
                        }
                    }
                    else
                    {
                        TempData["message"] = "Email is already in use, please use a different one.";
                        RedirectToAction("RegisterVoter");
                    }
                }
                else
                {
                    TempData["message"] = "The user already exists please login!";
                    RedirectToAction("VoterLogin");
                }
            }
            else
            {
                TempData["message"] = "Please enter a valid ID number!";
                return RedirectToAction("RegisterVoter");
            }
            TempData["message"] = "An error has occured! please try again";
            return RedirectToAction("RegisterVoter");
        }

        

        
        public ActionResult VoterLogin()
        {
            ViewBag.message = TempData["Message"];
            ViewBag.success = TempData["success"];
            return View();
        }

        [HttpPost]
        public ActionResult LogVoterIn(string IDNumber, string Password)
        {
            
            if (IDNumber.Length != 13)
            {
                TempData["Message"] = "Your ID number is not the right length";
                return RedirectToAction("VoterLogin");
            }
            else
            {
                var IDNum = registerVoter.HashedData(IDNumber);
                var Pass = registerVoter.HashedData(Password);
                Voter u = db.Voters.Where(x => x.VoterIDNumber == IDNum && x.VoterPassword == Pass).FirstOrDefault();
                if (u != null)
                {
                    VoterVM voterVM = new VoterVM();
                    voterVM.voter = u;
                    voterVM.RefreshGUID(db);
                    TempData["voterVM"] = voterVM;

                    return RedirectToAction("VoterHomePage");
                }
                else
                {
                    TempData["Message"] = "Your ID number or password was incorect! Please try again!";
                    return RedirectToAction("VoterLogin", "Voter");
                }
            }
            
        }
        
        
        public ActionResult VoterHomePage(string VoterGUID)
        {
            if(VoterGUID != null)
            {
                VoterVM voterVM = new VoterVM();
                if (voterVM.IsLogedIn(db, VoterGUID))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(registerVoterVM);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login Again";
                    return RedirectToAction("VoterLogin");
                }
            }
            else
            {
                VoterVM voterVM = TempData["voterVM"] as VoterVM;
                if (voterVM.IsLogedIn(db))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(registerVoterVM);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login Again";
                    return RedirectToAction("VoterLogin");
                }
            }
            TempData["message"] = "An Error Occured Please Login Again";
            return RedirectToAction("VoterLogin");
        }

        public ActionResult UpdateVoterInfo(string VoterGUID, int? id)
        {
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];
            if (VoterGUID != null)
            {
                VoterVM voterVM = new VoterVM();
                if (voterVM.IsLogedIn(db, VoterGUID))
                {
                    voterVM.RefreshGUID(db);
                    int test = Convert.ToInt32(id);
                   Voter v = db.Voters.Where(x => x.VoterID == test).FirstOrDefault();

                    registerVoter.voterView = voterVM;
                    registerVoter.VoterList = db.Voters.Where(x => x.VoterID == test).ToList();
                    TempData["voterVM"] = voterVM;
                    ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                    ViewBag.CityOrTownID = new SelectList(db.CityOrTowns, "CityOrTownID", "CityOrTownName");
                    ViewBag.SuburbID = new SelectList(db.Suburbs, "SuburbID", "SuburbName");
                    return View(v);
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please try again!";
                return RedirectToAction("VoterHomePage", new { VoterGUID = registerVoter.voterView.voter.GUID });
            }
            return View();
        }

        public ActionResult ChangePasswordVoter(string VoterGUID, string id)
        {
            ViewBag.message = TempData["message"];
            if (VoterGUID != null)
            {
                VoterVM voterVM = new VoterVM();
                if (voterVM.IsLogedIn(db, VoterGUID))
                {
                    voterVM.RefreshGUID(db);
                    int test = Convert.ToInt32(id);
                    Voter v = db.Voters.Where(x => x.VoterID == test).FirstOrDefault();
                    if (v != null)
                    {
                        var searchforAnswer = v.SecurityQuestionID;
                        if (searchforAnswer != null)
                        {
                            var FoundAnswer = db.SecurityQuestions.Where(y => y.SecurityQuestionID == searchforAnswer).FirstOrDefault();
                            if(FoundAnswer != null)
                            {
                                registerVoter.voterView = voterVM;
                                registerVoter.VoterList = db.Voters.ToList();
                                TempData["voterVM"] = voterVM;
                                ViewBag.Answer = FoundAnswer.SecurityQuestion1;
                                return View(v);
                            }
                            else
                            {
                                TempData["message"] = "The Security Question was not found";
                                return RedirectToAction("VoterHomePage", new { VoterGUID = registerVoter.voterView.voter.GUID });
                            }
                        }
                        else
                        {
                            TempData["message"] = "The Security Question was not found";
                            return RedirectToAction("VoterHomePage", new { VoterGUID = registerVoter.voterView.voter.GUID });
                        }
                        
                    }
                    else
                    {
                        TempData["message"] = "Your Information was not found!";
                        return RedirectToAction("VoterHomePage", new { VoterGUID = registerVoter.voterView.voter.GUID });
                    }
                    
                }
            }
            else
            {
                TempData["message"] = "An Error Has Occured!";
                return RedirectToAction("VoterHomePage", new { VoterGUID = registerVoter.voterView.voter.GUID });
            }
            return View();
        }

        public ActionResult Logout()
        {
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult doVoterUpdate([Bind(Include = "VoterIDNumber,VoterFirstNames,VoterLastName,VoterStreetAddress,SuburbID,ProvinceID,CityOrTownID,VoterPhoneNumber,VoterEmail")] Voter voter)
        {
            VoterVM voterVM = TempData["voterVM"] as VoterVM;
          
            
                registerVoter.voterView = voterVM;
                Voter u = db.Voters.Where(x => x.VoterIDNumber == voter.VoterIDNumber).FirstOrDefault();

            if (u != null && voter.VoterFirstNames != null && voter.VoterLastName != null && voter.VoterStreetAddress != null && voter.SuburbID != null && voter.VoterPhoneNumber != null && voter.VoterEmail != null)
            {
                u.VoterFirstNames = voter.VoterFirstNames;
                u.VoterLastName = voter.VoterLastName;
                u.VoterStreetAddress = voter.VoterStreetAddress;
                u.SuburbID = voter.SuburbID;
                u.ProvinceID = voter.ProvinceID;
                u.CityorTownID = voter.CityorTownID;
                u.VoterEmail = voter.VoterEmail;
                u.VoterPhoneNumber = voter.VoterPhoneNumber;
                db.SaveChanges();
                TempData["success"] = "Your Information was updated successfully.";
                return RedirectToAction("VoterHomePage", new { VoterGUID = registerVoter.voterView.voter.GUID });
            }
            else
            {
                TempData["message"] = "Your Information was not updated try again!";
                return RedirectToAction("UpdateVoterInfo", new { VoterGUID = registerVoter.voterView.voter.GUID, id = registerVoter.voterView.voter.VoterID });
            }
          
        }

        [HttpPost]
        public ActionResult doUpdatePassword(string Answer, string NewPassword, string ConfirmNewPassword, string VoterGUID, string id)
        {
            VoterVM voterVM = TempData["voterVM"] as VoterVM;

            registerVoter.voterView = voterVM;

            using (VotingSystemProjectEntities3 db = new VotingSystemProjectEntities3())
            {
                var ans = registerVoter.HashedData(Answer);
                var vid = Convert.ToInt32(id);
                var search = db.Voters.Where(x => x.VoterID == vid && x.SecurityQuestionAnswer == ans).FirstOrDefault();
                if(search != null)
                {
                    if (NewPassword == ConfirmNewPassword)
                    {
                        string hassednew = registerVoter.HashedData(NewPassword);
                        string hassedcnew = registerVoter.HashedData(ConfirmNewPassword);
                        search.VoterPassword = hassednew;
                        db.SaveChanges();
                        TempData["success"] = "Your Password has been updated";
                        return RedirectToAction("VoterHomePage", new { VoterGUID = registerVoter.voterView.voter.GUID});
                    }
                }
                else
                {
                    TempData["message"] = "Your Security Question Answer was wrong! Please try again!";
                    return RedirectToAction("ChangePasswordVoter", new { VoterGUID = registerVoter.voterView.voter.GUID, id = registerVoter.voterView.voter.VoterID });
                }
            }
            TempData["message"] = "An Error Occured!"; 
            return RedirectToAction("VoterHomePage", new { VoterGUID = registerVoter.voterView.voter.GUID });
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
                    else
                    {
                        TempData["message"] = "An Unknown Error Took Place, Please try again";
                        return RedirectToAction("VoterLogin");
                    }
                }
                else
                {
                    TempData["message"] = "An Unknown Error Took Place, Please try again";
                    return RedirectToAction("VoterLogin");
                }


            }
            else
            {
                TempData["message"] = "ID Number or Security Question Answer is incorrect";
                return RedirectToAction("VoterLogin");
            }
                return View();
        }
        [HttpPost]
        public ActionResult doForgotPasswordUpdate(string Answer, string NewPassword, string ConfirmNewPassword, string IDNumber)
        {
            using (VotingSystemProjectEntities3 db = new VotingSystemProjectEntities3())
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
                        TempData["success"] = "Your Password has been updated";
                        return RedirectToAction("Voterlogin");
                    }
                }
                else
                {
                    TempData["message"] = "Your your Security Question Answer was incorrect! Please try again!";
                    return RedirectToAction("ForgotPassword", new { IDnum = IDNumber});
                }
            }
            TempData["message"] = "Your Password was not updated";
            return RedirectToAction("VoterLogin");
        }

        public ActionResult VoteParty(string VoterGUID, string id)
        {
            VotingViewModel votingViewModel = new VotingViewModel();
            if (votingViewModel.GetElectionDate() != null)
            {

                if (VoterGUID != null)
                {
                    VoterVM voterVM = new VoterVM();
                    if (voterVM.IsLogedIn(db, VoterGUID) && voterVM.voter.VotePartyStatus == false)
                    {
                        voterVM.RefreshGUID(db);
                        
                        votingViewModel.voterView = voterVM;

                        votingViewModel.listcandidate = db.Candidates.Include(y => y.Party).Include(j => j.Party.PartyImage).Where(x => x.CandidatePosition_ID == 1).ToList();
                        votingViewModel.partiesImages = db.PartyImages.ToList();

                        ViewBag.message = TempData["message"];
                        ViewBag.success = TempData["success"];
                        return View(votingViewModel);
                    }
                    else
                    {
                        TempData["message"] = "You have already voted!";
                        return RedirectToAction("VoterHomePage","Voter", new { VoterGUID = VoterGUID });
                    }
                }
                else
                {
                    TempData["message"] = "An Error Occured Please try again";
                    return RedirectToAction("VoterHomePage", "Voter", new { VoterGUID = VoterGUID });
                }
            }
            else
            {
                TempData["message"] = "You can only vote on the set election date";
                return RedirectToAction("VoterHomePage", new { VoterGUID = VoterGUID});
            }
        }

        public ActionResult doVoteParty(string PartyID, string VoterGUID, string VoterID)
        {
            if (VoterGUID != null && PartyID != null && VoterID != null)
            {
                VoterVM voterVM = new VoterVM();
                if (voterVM.IsLogedIn(db, VoterGUID) && voterVM.voter.VotePartyStatus == false)
                {
                    voterVM.RefreshGUID(db);
                    VotingViewModel votingViewModel = new VotingViewModel();
                    votingViewModel.voterView = voterVM;
                    if(votingViewModel.GetElectionDate() != null)
                    {
                        int ids = Convert.ToInt32(VoterID);
                        var v = db.Voters.Where(x => x.VoterID == ids).FirstOrDefault();
                        if (v != null)
                        {
                            //Gets the Election and increases the total votes by 1
                            Election getdateElection = votingViewModel.GetElectionDate();
                            getdateElection.TotalVotes = getdateElection.TotalVotes + 1;
                            NationalResult nr = new NationalResult();
                            nr.ElectionID = getdateElection.ElectionID;
                            nr.PartyID = Convert.ToInt32(PartyID);
                            nr.NationalResultsTotalVotes = 1;
                            v.VotePartyStatus = true;
                            db.NationalResults.Add(nr);
                            db.SaveChanges();
                            TempData["success"] = "You have successfully voted for National Government";
                            return RedirectToAction("VoterHomePage", new { VoterGUID = votingViewModel.voterView.voter.GUID, id = votingViewModel.voterView.voter.VoterID });
                        }
                        else
                        {
                            TempData["message"] = "An Error Occured Please Try Again!";
                            return RedirectToAction("VoteParty", new { VoterGUID = votingViewModel.voterView.voter.GUID, id = registerVoter.voterView.voter.VoterID });
                        
                        }
                    }
                    else
                    {
                        TempData["message"] = "An Error Occured Please Try Again!";
                        return RedirectToAction("VoteParty", new { VoterGUID = votingViewModel, id = registerVoter.voterView.voter.VoterID });
                    }
                    
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Try Again";
                    return RedirectToAction("VoteParty", new { VoterGUID = VoterGUID, id = VoterID });
                }
                    
            }
            else
            {
                TempData["message"] = "An Error Occured Please try again!";
                return RedirectToAction("VoteParty", new { VoterGUID = registerVoter.voterView.voter.GUID, id = registerVoter.voterView.voter.VoterID });
            }
        }

        public ActionResult VoteProvincial(string VoterGUID, string id)
        {
            VotingViewModel votingViewModel = new VotingViewModel();
            if (votingViewModel.GetElectionDate() != null)
            {

                if (VoterGUID != null)
                {
                    VoterVM voterVM = new VoterVM();
                    if (voterVM.IsLogedIn(db, VoterGUID) && voterVM.voter.VoteProvinceStatus == false)
                    {
                        voterVM.RefreshGUID(db);
                        int ids = Convert.ToInt32(id);
                        votingViewModel.voterView = voterVM;
                        var v = db.Voters.Where(l => l.VoterID == ids).FirstOrDefault();
                        if (v != null)
                        {
                            votingViewModel.listcandidate = db.Candidates.Include(y => y.Party).Include(k => k.Party.PartyImage).Where(x => x.ProvinceID == v.ProvinceID && x.CandidatePosition_ID != 1).ToList();
                            return View(votingViewModel);
                        }
                        else
                        {
                            TempData["message"] = "You have already voted!";
                            return RedirectToAction("VoterHomePage", new { VoterGUID = votingViewModel.voterView.voter.GUID });
                        }
                    }
                    else
                    {
                        TempData["message"] = "You have already voted!";
                        return RedirectToAction("VoterHomePage", new { VoterGUID = VoterGUID });

                    }
                }
                else
                {
                    TempData["message"] = "An Error Occured Please try again";
                    return RedirectToAction("VoterHomePage", new { VoterGUID = VoterGUID});
                }
            }
            else
            {
                TempData["message"] = "You can only vote on the set election date";
                return RedirectToAction("VoterHomePage", new { VoterGUID = VoterGUID });
            
            }
        }

        public ActionResult doVoteProvincial(string VoterGUID, string id, string PartyID, string provinceID)
        {
            if (VoterGUID != null && PartyID != null && id != null)
            {
                VoterVM voterVM = new VoterVM();
                if (voterVM.IsLogedIn(db, VoterGUID) && voterVM.voter.VoteProvinceStatus == false)
                {
                    voterVM.RefreshGUID(db);
                    VotingViewModel votingViewModel = new VotingViewModel();
                    votingViewModel.voterView = voterVM;
                    if (votingViewModel.GetElectionDate() != null)
                    {
                        int ids = Convert.ToInt32(id);
                        var v = db.Voters.Where(x => x.VoterID == ids).FirstOrDefault();
                        if (v != null)
                        {
                            //Gets the Election and increases the total votes by 1
                            Election getdateElection = votingViewModel.GetElectionDate();
                            ProvincialResult pr = new ProvincialResult();
                            pr.PartyID = Convert.ToInt32(PartyID);
                            pr.ElectionID = getdateElection.ElectionID;
                            pr.ProvincialResultsTotalVotes = 1;
                            pr.ProvinceID = Convert.ToInt32(provinceID);
                            v.VoteProvinceStatus = true;
                            db.ProvincialResults.Add(pr);
                            db.SaveChanges();
                            TempData["success"] = "You have successfully voted for a Provincial Government!";
                            return RedirectToAction("VoterHomePage", new { VoterGUID = votingViewModel.voterView.voter.GUID, id = votingViewModel.voterView.voter.VoterID });
                        }
                        else
                        {
                            TempData["message"] = "An Error Occured Please Try Again!";
                            return RedirectToAction("VoteProvincial", new { VoterGUID = votingViewModel.voterView.voter.GUID, id = votingViewModel.voterView.voter.VoterID });

                        }
                    }
                    else
                    {
                        TempData["message"] = "An Error Occured Please Try Again!";
                        return RedirectToAction("VoteProvincial", new { VoterGUID = votingViewModel.voterView.voter.GUID, id = votingViewModel.voterView.voter.VoterID });
                    }

                }
                else
                {
                    TempData["message"] = "An Error Occured Please Try Again";
                    return RedirectToAction("VoteProvincial", new { VoterGUID = VoterGUID, id = id });
                }

            }
            else
            {
                TempData["message"] = "An Error Occured Please try again!";
                return RedirectToAction("VoteProvincial", new { VoterGUID = VoterGUID, id = id });
            }
        }


        public ActionResult ForgotPasswordGetID()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SearchForStation(string IDNumber)
        {
            var idnumEncrypt = registerVoter.HashedData(IDNumber);
            var v = db.Voters.Where(x => x.VoterIDNumber == idnumEncrypt).FirstOrDefault();
            if(v != null)
            {
                var station = db.VotingStations.Where(j => j.ProvinceID == v.ProvinceID && j.CityOrTownID == v.CityorTownID && j.SuburbID == v.SuburbID).FirstOrDefault();
                if(station != null)
                {

                    TempData["VotingStation"] = station;
                    TempData["success"] = "We Found Your Voting Station!";
                    return RedirectToAction("FindVotingStation","Voter");
                }
                else
                {
                    TempData["message"] = "We could not Locate a Voting station in your Area. Please Vote Online Instead";
                    return RedirectToAction("FindVotingStation", "Voter");
                }
            }
            else
            {
                TempData["message"] = "We could not Locate a Voting station in your area. Please Vote Online Instead!";
                return RedirectToAction("FindVotingStation", "Voter");
            }
        }
    }
}