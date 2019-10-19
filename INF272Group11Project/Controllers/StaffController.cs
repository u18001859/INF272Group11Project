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
    public class StaffController : Controller
    {
        // GET: Staff
        VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2();
        StaffEncryption staffEncryptionVM = new StaffEncryption();
        
        public ActionResult StaffHomePage(string StaffGUID)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUIDControl = new StaffGUIDControl();
                 if (staffGUIDControl.IsLogedIn(db, StaffGUID))
                {
                    staffGUIDControl.RefreshGUID(db);
                    StaffViewModel staffVM = new StaffViewModel();
                    staffVM.StaffView = staffGUIDControl;
                    staffVM.ListStaff = db.Staffs.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(staffVM);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login Again";
                    return RedirectToAction("StaffLogin");
                }
            }
            else
            {
                StaffGUIDControl staffGUID = TempData["GUIDControl"] as StaffGUIDControl;
                if (staffGUID.IsLogedIn(db))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffVM = new StaffViewModel();
                    staffVM.StaffView = staffGUID;
                    staffVM.ListStaff = db.Staffs.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(staffVM);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login Again";
                    return RedirectToAction("StaffLogin");

                }
            }
            
          
        }

        public ActionResult StaffLogin()
        {
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["sucess"];
            return View();
        }
        [HttpPost]
        public ActionResult doStaffLogin(string UserName, string Password)
        {
            if(UserName == null && Password == null)
            {
                TempData["message"] = "Please fill in all your login details!";
                return RedirectToAction("StaffLogin");
            }
            else if (UserName == null || Password == null)
            {
                TempData["message"] = "Please fill in all your login details!";
                return RedirectToAction("StaffLogin");
            }
            else
            {
                
                var hassedPassword = staffEncryptionVM.HashedData(Password);
                Staff s = db.Staffs.Where(x => x.Staff_UserName == UserName && x.Staff_Password == hassedPassword).FirstOrDefault();
                if (s != null)
                {
                    StaffGUIDControl staffGUIDControl = new StaffGUIDControl();
                    staffGUIDControl.staff = s;
                    staffGUIDControl.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    
                    staffView.ListStaff = db.Staffs.ToList();
                    TempData["GUIDControl"] = staffGUIDControl;

                   return RedirectToAction("StaffHomePage");
                }
                else
                {
                    TempData["message"] = "Your Username or Password was incorrect!";
                    return RedirectToAction("StaffLogin");
                }
                
            }
            
        }
        [HttpPost]
        public ActionResult ForgotPassword(string UserName)
        {
            var check = db.Staffs.Where(x => x.Staff_UserName == UserName).FirstOrDefault();
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
                        ViewBag.Username = UserName;
                        ViewBag.message = TempData["message"];
                        return View(check);
                    }
                    else
                    {
                        TempData["message"] = "An Unknown Error Took Place, Please try again";
                        return RedirectToAction("StaffLogin");
                    }
                }
                else
                {
                    TempData["message"] = "An Unknown Error Took Place, Please try again";
                    return RedirectToAction("StaffLogin");
                }
            }
            else
            {
                TempData["message"] = "Your Username was not found";
                return RedirectToAction("StaffLogin");
            }
        }
        [HttpPost]
        public ActionResult doForgotPasswordUpdate(string Username, string Answer, string NewPassword, string ConfirmNewPassword)
        {
            
            Staff s = db.Staffs.Where(x => x.Staff_UserName == Username && x.StaffSecurityQuestionAnswer == Answer).FirstOrDefault();
            if (Username != null && Answer != null && NewPassword != null && ConfirmNewPassword != null)
            {
               
                if (s != null)
                {
                    if(NewPassword == ConfirmNewPassword)
                    {
                        var hassedPassword = staffEncryptionVM.HashedData(NewPassword);
                        s.Staff_Password = hassedPassword;
                        db.SaveChanges();
                        TempData["success"] = "Your Password Has Been Updated";
                        return RedirectToAction("StaffLogin");
                    }
                    else
                    {
                        TempData["message"] = "Your Passwords do not Match!";
                        return RedirectToAction("ForgotPassword", new { UserName = s.Staff_UserName});
                    }
                }
                else
                {
                    TempData["message"] = "An Error Has Occured! Please Try Again!";
                    return RedirectToAction("StaffLogin");
                }
            }
            else
            {
                TempData["message"] = "Please Fill in your details";
                return RedirectToAction("ForgotPassword", new { UserName = s.Staff_UserName});
            }

        }

        public ActionResult ChangePasswordStaff(string StaffGUID, string id)
        {
            return RedirectToAction("doUpdatePassword");
        }
        [HttpPost]
        public ActionResult doUpdatePassword(string StaffGUID, string id, string Answer, string NewPassword, string ConfirmNewPassword)
        {
            return RedirectToAction("StaffHomePage");
        }
        [HttpPost]
        public ActionResult RegisterStaff(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUIDControl = new StaffGUIDControl();
                if (staffGUIDControl.IsLogedIn(db, StaffGUID))
                {
                    staffGUIDControl.RefreshGUID(db);
                    StaffViewModel staffVM = new StaffViewModel();
                    staffVM.StaffView = staffGUIDControl;
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    ViewBag.SecurityQuestionID = new SelectList(db.SecurityQuestions, "SecurityQuestionID", "SecurityQuestion1");
                    ViewBag.StaffPositions = new SelectList(db.StaffPositions, "StaffPositionID", "StaffPosition_Description");
                    return View(staffVM);
                }
                else
                {
                    TempData["message"] = "You do not have permission to access this page";
                    return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID});
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login Again";
                return RedirectToAction("StaffLogin");
            }
           
        }

        [HttpPost]
        public ActionResult AddNewStaff(string StaffUserName, string FirstName, string LastName, string Email, string PhoneNumber, string Password, string ConfirmPassword, [Bind(Include = "StaffPostionID, SecurityQuestionID")] Staff staff, string SecurityQuestionAnswer, string StaffGUID, string id)
        {
            if(StaffUserName != null && FirstName != null && LastName != null && Email != null && PhoneNumber != null && Password != null && ConfirmPassword != null)
            {
                var searchusername = db.Staffs.Where(x => x.Staff_UserName == StaffUserName).FirstOrDefault();
                if(searchusername == null)
                {
                    var searchEmail = db.Staffs.Where(j => j.StaffEmail == Email).FirstOrDefault();
                    if(searchEmail == null)
                    {
                        var searchPhoneNumber = db.Staffs.Where(l => l.StaffPhoneNumber == PhoneNumber).FirstOrDefault();
                        if(searchPhoneNumber == null)
                        {
                            if(ConfirmPassword == Password)
                            {
                                var HassedPassword = staffEncryptionVM.HashedData(Password);
                                Staff CreateStaff = new Staff();
                                CreateStaff.Staff_UserName = StaffUserName;
                                CreateStaff.Staff_Password = HassedPassword;
                                CreateStaff.Staff_FirstNames = FirstName;
                                CreateStaff.Staff_LastName = LastName;
                                CreateStaff.SecurityQuestionID = staff.SecurityQuestionID;
                                CreateStaff.StaffPositionID = staff.StaffPositionID;
                                CreateStaff.StaffEmail = Email;
                                CreateStaff.StaffPhoneNumber = PhoneNumber;
                                CreateStaff.StaffSecurityQuestionAnswer = SecurityQuestionAnswer;
                                CreateStaff.GUID = Guid.NewGuid().ToString();
                                CreateStaff.GUIDTimeStamp = DateTime.Now;
                                db.Staffs.Add(CreateStaff);
                                db.SaveChanges();
                                TempData["success"] = "You have successfully Registered a StaffMember";
                                return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID});
                            }
                            else
                            {
                                TempData["message"] = "Your password do not match!";
                                return RedirectToAction("RegisterStaff", new { StaffGUID = StaffGUID, id = id });
                            }
                        }
                        else
                        {
                            TempData["message"] = "Your Phonenumber is already in use!";
                            return RedirectToAction("RegisterStaff", new { StaffGUID = StaffGUID, id = id });
                        }
                    }
                    else
                    {
                        TempData["message"] = "Your Email is already in use!";
                        return RedirectToAction("RegisterStaff", new { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "Your username is already in use!";
                    return RedirectToAction("RegisterStaff", new { StaffGUID = StaffGUID, id = id });
                }
            }
            else
            {
                TempData["message"] = "Please Fill In All of your Details";
                return RedirectToAction("RegisterStaff", new { StaffGUID = StaffGUID, id = id });
            }
         
        }

        public ActionResult UpdateStaffInfo(string StaffGUID, string id)
        {
            return RedirectToAction("StaffHomePage");

        }
        [HttpPost]
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

        public ActionResult GetUserName()
        {
            return View();
        }

        public ActionResult SetElectionDate(string StaffGUID, string id)
        {
            ViewBag.message = TempData["message"];
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if(staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    return View(staffView);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login Again!";
                    return RedirectToAction("StaffLogin");
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login Again!";
                return RedirectToAction("StaffLogin");
            }
            
        }
        [HttpPost]
        public ActionResult doSetElectionDate(string StaffGUID, string id, DateTime ElectionDate)
        {
            if(ElectionDate != null && ElectionDate.Date > DateTime.Today)
            {
                var search = db.Elections.Where(j => j.ElectionDate == ElectionDate).FirstOrDefault();
                if(search == null)
                {
                    Election election = new Election();
                    election.ElectionDate = ElectionDate;
                    db.Elections.Add(election);
                    RegisterVoterVM register = new RegisterVoterVM();
                    register.VoterList = db.Voters.Where(x => x.VotePartyStatus == true).ToList();
                    RegisterVoterVM register2 = new RegisterVoterVM();
                    register2.VoterList = db.Voters.Where(k => k.VoteProvinceStatus == true).ToList();
                    if (register.VoterList != null && register2.VoterList != null)
                    {


                        foreach (var item in register.VoterList)
                        {

                            Voter v = new Voter();
                            var searchVoter = db.Voters.Where(l => l.VoterID == item.VoterID).FirstOrDefault();
                            if (searchVoter != null)
                            {
                                item.VotePartyStatus = false;
                                v.VotePartyStatus = item.VotePartyStatus;

                                db.SaveChanges();
                            }
                            else
                            {
                                TempData["message"] = "Please Select A Valid Election Date!";
                                return RedirectToAction("SetElectionDate", new { StaffGUID = StaffGUID, id = id });
                            }

                        }





                        foreach (var item2 in register2.VoterList)
                        {
                            Voter v = new Voter();
                            var searchVoter = db.Voters.Where(l => l.VoterID == item2.VoterID).FirstOrDefault();
                            if (searchVoter != null)
                            {

                                item2.VoteProvinceStatus = false;
                                v.VoteProvinceStatus = item2.VoteProvinceStatus;

                                db.SaveChanges();
                            }
                            else
                            {
                                TempData["message"] = "Please Select A Valid Election Date!";
                                return RedirectToAction("SetElectionDate", new { StaffGUID = StaffGUID, id = id });
                            }
                        }
                        db.SaveChanges();
                        TempData["success"] = "The Eelection Date Has Been Set!";
                        return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
                    }
                    else
                    {
                        Election elections = new Election();
                        elections.ElectionDate = ElectionDate;
                        db.Elections.Add(election);
                        db.SaveChanges();
                        TempData["success"] = "The Eelection Date Has Been Set!";
                        return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });


                    }

                }
                else
                {
                    TempData["message"] = "This ElectionDate Already Exists Please Select A different One!";
                    return RedirectToAction("SetElectionDate", new { StaffGUID = StaffGUID, id = id });
                }
                

            }
            else
            {
                TempData["message"] = "Please Select A Valid Election Date!";
                return RedirectToAction("SetElectionDate", new { StaffGUID = StaffGUID, id = id });
            }
        }
    }
}