using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace INF272Group11Project.Controllers
{
    public class StaffController : Controller
    {

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
                    return RedirectToAction("StaffLogin","Staff");
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
                    return RedirectToAction("StaffLogin", "Staff");

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
            if (UserName == null && Password == null)
            {
                TempData["message"] = "Please fill in all your login details!";
                return RedirectToAction("StaffLogin", "Staff");
            }
            else if (UserName == null || Password == null)
            {
                TempData["message"] = "Please fill in all your login details!";
                return RedirectToAction("StaffLogin", "Staff");
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

                    return RedirectToAction("StaffHomePage", "Staff");
                    
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
                    if (NewPassword == ConfirmNewPassword)
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
                        return RedirectToAction("ForgotPassword", new
                        { UserName = s.Staff_UserName });
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
                return RedirectToAction("ForgotPassword", new
                {
                    UserName = s.Staff_UserName
                });
            }

        }

        public ActionResult ChangePasswordStaff(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    var ids = Convert.ToInt32(id);
                    Staff s = db.Staffs.Where(x => x.StaffID == ids).FirstOrDefault();
                    if (s != null)
                    {
                        var searchAnswer = s.SecurityQuestionID;
                        if (searchAnswer != null)
                        {

                            var searchForDec = db.SecurityQuestions.Where(k => k.SecurityQuestionID == searchAnswer).FirstOrDefault();
                            if (searchForDec != null)
                            {
                                ViewBag.Answer = searchForDec.SecurityQuestion1;
                                return View(staffView);
                            }
                            else
                            {
                                TempData["message"] = "An Error Occured Please Try Again";
                                return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
                            }
                        }
                        else
                        {
                            TempData["message"] = "An Error Occured Please Try Again";
                            return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
                        }

                    }
                    else
                    {
                        TempData["message"] = "An Error Occured Please Try Again";
                        return RedirectToAction("StaffHomePage", new
                        {
                            StaffGUID = StaffGUID
                        });
                    }

                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login Again";
                    return RedirectToAction("StaffLogin");
                }

            }
            else
            {
                TempData["message"] = "An Error Occured Please Try Again";
                return RedirectToAction("StaffHomePage", new
                {
                    StaffGUID = StaffGUID
                });
            }
        }
        [HttpPost]
        public ActionResult doUpdatePassword(string StaffGUID, string id, string Answer, string NewPassword, string ConfirmNewPassword)
        {  
          
                  if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    var ids = Convert.ToInt32(id);
                    Staff s = db.Staffs.Where(j => j.StaffID == ids && j.StaffSecurityQuestionAnswer == Answer).FirstOrDefault();
                    if (s != null)
                    {
                        if (NewPassword == ConfirmNewPassword)
                        {
                            var hassedPassword = staffEncryptionVM.HashedData(NewPassword);
                            s.Staff_Password = hassedPassword;
                            db.SaveChanges();
                            TempData["success"] = "Your Password Has Been Updated!";
                            return RedirectToAction("StaffHomePage", new { StaffGUID = staffView.StaffView.staff.GUID });
                        }
                        else
                        {
                            TempData["message"] = "Your Passwords Do Not March. Try Again!";
                            return RedirectToAction("ChangePasswordStaff", new { StaffGUID = StaffGUID, id = id });
                        }

                    }
                    else
                    {
                        TempData["message"] = "Your Security Question Answer was Wrong.Try Again!";
                        return RedirectToAction("ChangePasswordStaff", new { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Try Again!";
                    return RedirectToAction("StaffHomePage", new
                    {
                        StaffGUID = StaffGUID
                    });
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login Again!";
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
                        return RedirectToAction("StaffLogin", "Staff");
                    }
                }
                else
                {
                    TempData["message"] = "An Unknown Error Took Place, Please try again";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "Your Username was not found";
                return RedirectToAction("StaffLogin", "Staff");
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
                    if (NewPassword == ConfirmNewPassword)
                    {
                        var hassedPassword = staffEncryptionVM.HashedData(NewPassword);
                        s.Staff_Password = hassedPassword;
                        db.SaveChanges();
                        TempData["success"] = "Your Password Has Been Updated";
                        return RedirectToAction("StaffLogin", "Staff");
                    }
                    else
                    {
                        TempData["message"] = "Your Passwords do not Match!";
                        return RedirectToAction("ForgotPassword","Staff", new
                        { UserName = s.Staff_UserName });
                    }
                }
                else
                {
                    TempData["message"] = "An Error Has Occured! Please Try Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "Please Fill in your details";
                return RedirectToAction("ForgotPassword", "Staff", new
                {
                    UserName = s.Staff_UserName
                });
            }

        }

        public ActionResult ChangePasswordStaff(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    var ids = Convert.ToInt32(id);
                    Staff s = db.Staffs.Where(x => x.StaffID == ids).FirstOrDefault();
                    if (s != null)
                    {
                        var searchAnswer = s.SecurityQuestionID;
                        if (searchAnswer != null)
                        {

                            var searchForDec = db.SecurityQuestions.Where(k => k.SecurityQuestionID == searchAnswer).FirstOrDefault();
                            if (searchForDec != null)
                            {
                                ViewBag.Answer = searchForDec.SecurityQuestion1;
                                return View(staffView);
                            }
                            else
                            {
                                TempData["message"] = "An Error Occured Please Try Again";
                                return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                            }
                        }
                        else
                        {
                            TempData["message"] = "An Error Occured Please Try Again";
                            return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                        }

                    }
                    else
                    {
                        TempData["message"] = "An Error Occured Please Try Again";
                        return RedirectToAction("StaffHomePage", "Staff", new
                        {
                            StaffGUID = StaffGUID
                        });
                    }

                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login Again";
                    return RedirectToAction("StaffLogin", "Staff");
                }

            }
            else
            {
                TempData["message"] = "An Error Occured Please Try Again";
                return RedirectToAction("StaffHomePage", "Staff", new
                {
                    StaffGUID = StaffGUID
                });
            }
        }
        [HttpPost]
        public ActionResult doUpdatePassword(string StaffGUID, string id, string Answer, string NewPassword, string ConfirmNewPassword)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    var ids = Convert.ToInt32(id);
                    Staff s = db.Staffs.Where(j => j.StaffID == ids && j.StaffSecurityQuestionAnswer == Answer).FirstOrDefault();
                    if (s != null)
                    {
                        if (NewPassword == ConfirmNewPassword)
                        {
                            var hassedPassword = staffEncryptionVM.HashedData(NewPassword);
                            s.Staff_Password = hassedPassword;
                            db.SaveChanges();
                            TempData["success"] = "Your Password Has Been Updated!";
                            return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = staffView.StaffView.staff.GUID });
                        }
                        else
                        {
                            TempData["message"] = "Your Passwords Do Not March. Try Again!";
                            return RedirectToAction("ChangePasswordStaff", "Staff", new { StaffGUID = StaffGUID, id = id });
                        }

                    }
                    else
                    {
                        TempData["message"] = "Your Security Question Answer was Wrong.Try Again!";
                        return RedirectToAction("ChangePasswordStaff", "Staff", new { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Try Again!";
                    return RedirectToAction("StaffHomePage", "Staff", new
                    {
                        StaffGUID = StaffGUID
                    });
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
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
                    return RedirectToAction("StaffHomePage", "Staff", new
                    {
                        StaffGUID = StaffGUID
                    });
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login Again";
                return RedirectToAction("StaffLogin", "Staff");
            }

        }

        [HttpPost]
        public ActionResult AddNewStaff(string StaffUserName, string FirstName, string LastName, string Email, string PhoneNumber, string Password, string ConfirmPassword, [Bind(Include = "StaffPostionID, SecurityQuestionID")] Staff staff, string SecurityQuestionAnswer, string StaffGUID, string id)
        {
            if (StaffUserName != null && FirstName != null && LastName != null && Email != null && PhoneNumber != null && Password != null && ConfirmPassword != null)
            {
                var searchusername = db.Staffs.Where(x => x.Staff_UserName == StaffUserName).FirstOrDefault();
                if (searchusername == null)
                {
                    var searchEmail = db.Staffs.Where(j => j.StaffEmail == Email).FirstOrDefault();
                    if (searchEmail == null)
                    {
                        var searchPhoneNumber = db.Staffs.Where(l => l.StaffPhoneNumber == PhoneNumber).FirstOrDefault();
                        if (searchPhoneNumber == null)
                        {
                            if (ConfirmPassword == Password)
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
                                return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                            }
                            else
                            {
                                TempData["message"] = "Your password do not match!";
                                return RedirectToAction("RegisterStaff", "Staff", new { StaffGUID = StaffGUID, id = id });
                            }
                        }
                        else
                        {
                            TempData["message"] = "Your Phonenumber is already in use!";
                            return RedirectToAction("RegisterStaff", "Staff", new { StaffGUID = StaffGUID, id = id });
                        }
                    }
                    else
                    {
                        TempData["message"] = "Your Email is already in use!";
                        return RedirectToAction("RegisterStaff", "Staff", new
                        {
                            StaffGUID = StaffGUID,
                            id = id
                        });
                    }
                }
                else
                {
                    TempData["message"] = "Your username is already in use!";
<
                    return RedirectToAction("RegisterStaff", "Staff", new

                    {
                        StaffGUID = StaffGUID,
                        id = id
                    });
                }
            }
            else
            {
                TempData["message"] = "Please Fill In All of your Details";

                return RedirectToAction("RegisterStaff", "Staff", new
                {
                    StaffGUID = StaffGUID,
                    id = id
                });
            }

        }

        public ActionResult UpdateStaffInfo(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUIDControl = new StaffGUIDControl();
                if (staffGUIDControl.IsLogedIn(db, StaffGUID))
                {
                    var ids = Convert.ToInt32(id);
                    staffGUIDControl.RefreshGUID(db);
                    Staff s = db.Staffs.Where(x => x.StaffID == ids).FirstOrDefault();
                    if (s != null)
                    {

                        StaffViewModel staffVM = new StaffViewModel();
                        staffVM.StaffView = staffGUIDControl;

                        ViewBag.message = TempData["message"];
                        ViewBag.success = TempData["success"];
                        return View(s);
                    }
                    else
                    {
                        TempData["message"] = "An Error Occured Please Try Again!";
                        return RedirectToAction("StaffHomePage", "Staff", new
                        {
                            StaffGUID = StaffGUID
                        });
                    }
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Try Again!";
                    return RedirectToAction("StaffHomePage", "Staff", new
                    {
                        StaffGUID = StaffGUID
                    });
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login Again";
                return RedirectToAction("StaffLogin", "Staff");
            }

        }
        [HttpPost]
        public ActionResult doStaffUpdate(string StaffGUID, string id, [Bind(Include = "Staff_FirstNames, Staff_LastName, StaffPhoneNumber, StaffEmail")] Staff staff)
        {
            var ids = Convert.ToInt32(id);
            if (staff.Staff_FirstNames != null && staff.Staff_LastName != null && staff.StaffPhoneNumber != null && staff.StaffEmail != null)
            {
                var searchPhone = db.Staffs.Where(x => x.StaffID != ids && x.StaffPhoneNumber == staff.StaffPhoneNumber).FirstOrDefault();
                if (searchPhone == null)
                {
                    var searchEmail = db.Staffs.Where(j => j.StaffID != ids && j.StaffEmail == staff.StaffEmail).FirstOrDefault();
                    if (searchEmail == null)
                    {
                        Staff searchStaff = db.Staffs.Where(g => g.StaffID == ids).FirstOrDefault();
                        if (searchStaff != null)
                        {
                            if (StaffGUID != null)
                            {
                                StaffGUIDControl staffGUIDControl = new StaffGUIDControl();
                                if (staffGUIDControl.IsLogedIn(db, StaffGUID))
                                {
                                    staffGUIDControl.RefreshGUID(db);
                                    StaffViewModel staffVM = new StaffViewModel();
                                    staffVM.StaffView = staffGUIDControl;
                                    searchStaff.Staff_FirstNames = staff.Staff_FirstNames;
                                    searchStaff.Staff_LastName = staff.Staff_LastName;
                                    searchStaff.StaffPhoneNumber = staff.StaffPhoneNumber;
                                    searchStaff.StaffEmail = staff.StaffEmail;
                                    db.SaveChanges();
                                    TempData["success"] = "Your Information Has Been Updated!";
                                    return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
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
                        else
                        {
                            TempData["message"] = "An Error Occured, Please Try Again!";
                            return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                        }
                    }
                    else
                    {
                        TempData["message"] = "The Email Entered Is Already In Use! Please Use A Different Email!";
                        return RedirectToAction("UpdateStaffInfo", "Staff", new
                        { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "The Phone Number Entered Is Already In Use! Please Use A Different Number!";
                    return RedirectToAction("UpdateStaffInfo", "Staff", new
                    {
                        StaffGUID = StaffGUID,
                        id = id
                    });
                }
            }
            else
            {
                TempData["message"] = "Please Fill In All The Required Details!";
                return RedirectToAction("UpdateStaffInfo", "Staff", new
                {
                    StaffGUID = StaffGUID,
                    id = id
                });
            }
        }

        public ActionResult RegisterParty(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    ViewBag.CityOrTownID = new SelectList(db.CityOrTowns, "CityOrTownID", "CityOrTownName");
                    ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                    ViewBag.SuburbID = new SelectList(db.Suburbs, "SuburbID", "SuburbName");
                    return View(staffView);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login In Again!";

                    return RedirectToAction("StaffLogin", "Staff");

                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login In Again!";

                return RedirectToAction("StaffLogin", "Staff");

            }

        }
        [HttpPost]
        public ActionResult AddNewParty(string StaffGUID, string id, string PartyName, string PartyAcronym, string PhoneNumber, string Email, [Bind(Include = "ProvinceID, CityOrTownID, SuburbID")] Party party, string StreetAddress, string Website)

        {

            if (PartyName != "" && PartyAcronym != "" && PhoneNumber != "" && Email != "" && StreetAddress != "" && Website != "")
            {
                var searchParty = db.Parties.Where(x => x.PartyName == PartyName).FirstOrDefault();
                if (searchParty == null)
                {
                    if (PhoneNumber.Length == 10)
                    {
                        var searchPhoneNumber = db.Parties.Where(x => x.PartyTelephone == PhoneNumber).FirstOrDefault();
                        if (searchPhoneNumber == null)
                        {
                            var searchEmail = db.Parties.Where(x => x.PartyEmail == Email).FirstOrDefault();
                            if (searchEmail == null)
                            {
                                var searchAddress = db.Parties.Where(x => x.PartyStreetAddress == StreetAddress).FirstOrDefault();
                                if (searchAddress == null)
                                {
                                    var searchWebsite = db.Parties.Where(x => x.PartyWebsite == Website).FirstOrDefault();
                                    if (searchWebsite == null)
                                    {
                                        Party CreateParty = new Party();
                                        CreateParty.PartyName = PartyName;
                                        CreateParty.PartyAccronym = PartyAcronym;
                                        CreateParty.PartyTelephone = PhoneNumber;
                                        CreateParty.PartyEmail = Email;
                                        CreateParty.PartyStreetAddress = StreetAddress;
                                        CreateParty.PartyWebsite = Website;
                                        CreateParty.SuburbID = party.SuburbID;
                                        CreateParty.ProvinceID = party.ProvinceID;
                                        CreateParty.CityOrTownID = party.CityOrTownID;
                                        db.Parties.Add(CreateParty);
                                        db.SaveChanges();
                                        var s = db.Parties.Where(l => l.PartyName == PartyName).FirstOrDefault();
                                        string t = s.PartyID.ToString();
                                        
                                        return RedirectToAction("PartyImages", "Staff", new { StaffGUID = StaffGUID, id = id, party = t });

                                    }
                                    else
                                    {
                                        TempData["message"] = "Website is already in use, please use a different one.";

                                        return RedirectToAction("RegisterParty", "Staff", new { StaffGUID = StaffGUID, id = id });

                                    }
                                }
                                else
                                {
                                    TempData["message"] = "StreetAddress is already in use, please use a different one.";

                                    return RedirectToAction("RegisterParty", "Staff", new { StaffGUID = StaffGUID, id = id });

                                }
                            }
                            else
                            {
                                TempData["message"] = "Email is already in use, please use a different one.";

                                return RedirectToAction("RegisterParty", "Staff", new { StaffGUID = StaffGUID, id = id });

                            }
                        }
                        else
                        {
                            TempData["message"] = "The Phone Number is already in use please use a different one.";

                            return RedirectToAction("RegisterParty", "Staff", new { StaffGUID = StaffGUID, id = id });

                        }
                    }
                    else
                    {
                        TempData["message"] = "Please enter a valid phone number";

                        return RedirectToAction("RegisterParty", "Staff", new { StaffGUID = StaffGUID, id = id });

                    }
                }
                else
                {
                    TempData["message"] = "The party already exists, please create a new one!";
                    return RedirectToAction("RegisterParty", "Staff", new { StaffGUID = StaffGUID, id = id });

                }
            }
            else
            {
                TempData["message"] = "Please fill in all the data!";
                return RedirectToAction("RegisterParty", "Staff", new { StaffGUID = StaffGUID, id = id });

            }

        }

        
        public ActionResult PartyImages(string StaffGUID, string id, string party)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    ViewBag.party = party;
                    return View(staffView);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login In Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login In Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }
        
        public ActionResult AddImages(string StaffGUID, string id, PartyImage model, HttpPostedFileBase Logo, HttpPostedFileBase LeadPic, string party)
        {

            var db = new VotingSystemProjectEntities3();
            if (Logo != null && LeadPic != null)
            {
                model.partyID = Convert.ToInt32(party);
                model.PartyPicture = new byte[Logo.ContentLength];
                Logo.InputStream.Read(model.PartyPicture, 0, Logo.ContentLength);
                model.PartyLeaderPicture = new byte[LeadPic.ContentLength];
                LeadPic.InputStream.Read(model.PartyLeaderPicture, 0, LeadPic.ContentLength);
                db.PartyImages.Add(model);
                db.SaveChanges();
                TempData["success"] = "The Party was Successfully Created!";
                return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
            }
            else
            {
                TempData["message"] = "Please select images";
                return RedirectToAction("PartyImages", new { StaffGUID = StaffGUID, id = id , party = party});
            }

        }



        public ActionResult DeleteParty(string StaffGUID, string id, string PartyID, string PartyName)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;

                    int ids = Convert.ToInt32(PartyID);
                    Party p = db.Parties.Where(x => x.PartyID == ids).FirstOrDefault();
                    PartyImage pi = db.PartyImages.Where(j => j.partyID == ids).FirstOrDefault();
                    if (p != null)
                    {
                        db.Parties.Remove(p);
                        db.PartyImages.Remove(pi);
                        db.SaveChanges();
                        
                        TempData["success"] = "The Party Was Successfully Deleted!";
                        return RedirectToAction("StaffHomePage",  new { StaffGUID = staffView.StaffView.staff.GUID });

                    }
                    else
                    {
                        TempData["message"] = "An Error Occured Please Login In Again!";
                        return RedirectToAction("StaffLogin");
                    }
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login In Again!";
                    return RedirectToAction("StaffLogin");
                }
            }

            else
            {
                TempData["message"] = "Your Session Has Expired Please Login Again!";
                return RedirectToAction("StaffLogin");
            }
        }
        public ActionResult UpateParty(string StaffGUID, string id, string PartyID)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    ViewBag.CityOrTownID = new SelectList(db.CityOrTowns, "CityOrTownID", "CityOrTownName");
                    ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                    ViewBag.SuburbID = new SelectList(db.Suburbs, "SuburbID", "SuburbName");
                    var ids = Convert.ToInt32(PartyID);
                    Party search = db.Parties.Where(x => x.PartyID == ids).FirstOrDefault();
                    staffView.Party = search;
                    return View(staffView);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login In Again!";
                    return RedirectToAction("StaffLogin");
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login In Again!";
                return RedirectToAction("StaffLogin");
            }
        }

        public ActionResult doPartyUpdate(string StaffGUID, string id, string PartyID, string PartyName, string PartyAccronym, string PartyTelephone, string StreetAddress, string PartyEmail, string Website, [Bind(Include = "ProvinceID, CityOrTownID, SuburbID")] Party party)
        {
            if (PartyName != null && PartyAccronym != null && PartyTelephone != null && StreetAddress != null && PartyEmail != null && Website != null)
            {
                var ids = Convert.ToInt32(PartyID);
                var searchName = db.Parties.Where(x => x.PartyID != ids && x.PartyName == PartyName).FirstOrDefault();
                if (searchName == null)
                {
                    var searchAcronym = db.Parties.Where(j => j.PartyID != ids && j.PartyAccronym == PartyAccronym).FirstOrDefault();
                    if (searchAcronym == null)
                    {
                        var searchTelephone = db.Parties.Where(g => g.PartyID == ids && g.PartyTelephone == PartyTelephone).FirstOrDefault();
                        if (searchTelephone != null)
                        {
                            var searchStreetAddress = db.Parties.Where(g => g.PartyID == ids && g.PartyStreetAddress == StreetAddress).FirstOrDefault();
                            if (searchStreetAddress != null)
                            {
                                var searchEmail = db.Parties.Where(g => g.PartyID == ids && g.PartyEmail == PartyEmail).FirstOrDefault();
                                if (searchEmail != null)
                                {
                                    var searchWebsite = db.Parties.Where(g => g.PartyID == ids && g.PartyWebsite == Website).FirstOrDefault();
                                    if (searchWebsite != null)
                                    {
                                        if (StaffGUID != null)
                                        {
                                            StaffGUIDControl staffGUIDControl = new StaffGUIDControl();
                                            if (staffGUIDControl.IsLogedIn(db, StaffGUID))
                                            {
                                                staffGUIDControl.RefreshGUID(db);
                                                StaffViewModel staffVM = new StaffViewModel();
                                                staffVM.StaffView = staffGUIDControl;
                                                Party p = db.Parties.Where(k => k.PartyID == ids).FirstOrDefault();
                                                p.PartyName = PartyName;
                                                p.PartyAccronym = PartyAccronym;
                                                p.PartyTelephone = PartyTelephone;
                                                p.PartyStreetAddress = StreetAddress;
                                                p.PartyEmail = PartyEmail;
                                                p.PartyWebsite = Website;
                                                p.SuburbID = party.SuburbID;
                                                p.ProvinceID = party.ProvinceID;
                                                p.CityOrTownID = party.CityOrTownID;
                                                db.SaveChanges();
                                                TempData["success"] = "Your Information Has Been Updated!";
                                                return RedirectToAction("UpdatePartyImages","Staff", new { StaffGUID = staffVM.StaffView.staff.GUID, id = id,  PartyID = PartyID });
                                            }
                                            else
                                            {
                                                TempData["message"] = "Your Session Has Expired Please Login Again!";
                                                return RedirectToAction("StaffLogin");
                                            }
                                        }
                                        else
                                        {
                                            TempData["message"] = "Your Session Has Expired Please Login Again!";
                                            return RedirectToAction("StaffLogin");
                                        }
                                    }
                                    else
                                    {
                                        TempData["message"] = "An Error Occured, Please Try Again!";
                                        return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
                                    }
                                }
                                else
                                {
                                    TempData["message"] = "An Error Occured, Please Try Again!";
                                    return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
                                }
                            }
                            else
                            {
                                TempData["message"] = "An Error Occured, Please Try Again!";
                                return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
                            }
                        }
                        else
                        {
                            TempData["message"] = "An Error Occured, Please Try Again!";
                            return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
                        }
                    }
                    else
                    {
                        TempData["message"] = "The Email Entered Is Already In Use! Please Use A Different Email!";
                        return RedirectToAction("UpdateParty", new
                        { StaffGUID = StaffGUID, id = id, PartyID = PartyID });
                    }
                }
                else
                {
                    TempData["message"] = "The Phone Number Entered Is Already In Use! Please Use A Different Number!";
                    return RedirectToAction("UpdateParty", new
                    {
                        StaffGUID = StaffGUID,
                        id = id,
                        PartyID = PartyID
                    });
                }
            }
            else
            {
                TempData["message"] = "Please Fill In All The Required Details!";
                return RedirectToAction("UpdateParty", new
                {
                    StaffGUID = StaffGUID,
                    id = id,
                    PartyID = PartyID
                });
            }
        }
        public ActionResult SearchParty(string StaffGUID, string id, [Bind(Include = "PartyID")] Party party)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
            if (staffGUID.IsLogedIn(db, StaffGUID))
            {
                staffGUID.RefreshGUID(db);
                StaffViewModel staffView = new StaffViewModel();
                staffView.StaffView = staffGUID;
                ViewBag.message = TempData["message"];
                ViewBag.success = TempData["success"];
                    var s = party.PartyID;
                    TempData["success"] = "The Party Was Successfully Found!";
                return RedirectToAction("UpdateOrDeleteParty", "Staff", new { StaffGUID = staffView.StaffView.staff.GUID, id = id, s = s });
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login In Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }
            else
            {
                TempData["message"] = "An Error Occured Please Login In Again!";
                return RedirectToAction("StaffLogin", "Staff");
    }
}
        public ActionResult UpdatePartyImages(string StaffGUID, string id, string PartyID)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    ViewBag.PartyID = PartyID;
                    return View(staffView);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login In Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login In Again!";
                return RedirectToAction("StaffLogin");
            }
        
        }

        public ActionResult DoUpdateImages(string StaffGUID, string id, PartyImage model, HttpPostedFileBase Logo, HttpPostedFileBase LeadPic, string PartyID)
        {
            if (Logo != null && LeadPic != null)
            {
                int p = Convert.ToInt32(PartyID);
                PartyImage pi = db.PartyImages.Where(x => x.partyID == p).FirstOrDefault();
                Party ps = db.Parties.Where(j => j.PartyID == p).FirstOrDefault();
                if (pi != null)
                {


                    pi.partyID = Convert.ToInt32(PartyID);
                    pi.PartyPicture = new byte[Logo.ContentLength];
                    Logo.InputStream.Read(pi.PartyPicture, 0, Logo.ContentLength);
                    pi.PartyLeaderPicture = new byte[LeadPic.ContentLength];
                    LeadPic.InputStream.Read(pi.PartyLeaderPicture, 0, LeadPic.ContentLength);
                    db.SaveChanges();
                    TempData["success"] = "The Party was Successfully Updated!";
                    return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
                }
                else if(ps != null)
                {
                    model.partyID = Convert.ToInt32(PartyID);
                    model.PartyPicture = new byte[Logo.ContentLength];
                    Logo.InputStream.Read(model.PartyPicture, 0, Logo.ContentLength);
                    model.PartyLeaderPicture = new byte[LeadPic.ContentLength];
                    LeadPic.InputStream.Read(model.PartyLeaderPicture, 0, LeadPic.ContentLength);
                    db.PartyImages.Add(model);
                    db.SaveChanges();
                    TempData["success"] = "The Party was Successfully Created!";
                    return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
                }
                else
                {
                    TempData["message"] = "An Error Occured and The Party could not be Updated!";
                    return RedirectToAction("UpdatePartyImages", new { StaffGUID = StaffGUID, id = id, PartyID = PartyID });
                }
            }
            else
            {
                TempData["message"] = "Please select images";
                return RedirectToAction("UpdatePartyImages", new { StaffGUID = StaffGUID, id = id, PartyID = PartyID });
            }
        }

        public  ActionResult UpdateOrDeleteParty(string StaffGUID, string id, string s)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    StaffViewModel staffView = new StaffViewModel();
                    staffView.StaffView = staffGUID;
                    ViewBag.PartyID = new SelectList(db.Parties, "PartyID", "PartyName");

                    int ids = Convert.ToInt32(s);
                    Party p = db.Parties.Where(x => x.PartyID == ids).FirstOrDefault();
                    staffView.Party = p;
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(staffView);
                }
                else
                {
                    TempData["message"] = "An Error Occured Please Login In Again!";
                    return RedirectToAction("StaffLogin");
                }
            }
            else
            {
                TempData["message"] = "An Error Occured Please Login In Again!";
                return RedirectToAction("StaffLogin");
            }
        }

        public ActionResult SetElectionDate(string StaffGUID, string id)
        {
            ViewBag.message = TempData["message"];
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
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
            if (ElectionDate != null && ElectionDate.Date > DateTime.Today)
            {
                var search = db.Elections.Where(j => j.ElectionDate == ElectionDate).FirstOrDefault();
                if (search == null)
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

                        return RedirectToAction("StaffHomePage", new
                        {
                            StaffGUID = StaffGUID
                        });

                    }
                    else
                    {
                        Election elections = new Election();
                        elections.ElectionDate = ElectionDate;
                        db.Elections.Add(election);
                        db.SaveChanges();
                        TempData["success"] = "The Eelection Date Has Been Set!";

                        return RedirectToAction("StaffHomePage", new
                        {
                            StaffGUID = StaffGUID
                        });


                        return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });

                    }

                }
                else
                {
                    TempData["message"] = "This ElectionDate Already Exists Please Select A different One!";

                    return RedirectToAction("SetElectionDate", new
                    {
                        StaffGUID = StaffGUID,
                        id = id
                    });

                }


            }
            else
            {
                TempData["message"] = "Please Select A Valid Election Date!";
                return RedirectToAction("SetElectionDate", new
                {
                    StaffGUID = StaffGUID,
                    id = id
                });
            }
        }
        public ActionResult GetUserName()
        {
            ViewBag.message = TempData["message"];
            return View();
        }
    }
}