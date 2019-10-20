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
        // GET: Staff
        VotingSystemProjectEntities3 db = new VotingSystemProjectEntities3();
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
                    staffVM.Liststaff = db.Staffs.ToList();
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
                    staffVM.Liststaff = db.Staffs.ToList();
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
            if (UserName == null && Password == null)
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

                    staffView.Liststaff = db.Staffs.ToList();
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
                                return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
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
                        return RedirectToAction("RegisterStaff", new
                        {
                            StaffGUID = StaffGUID,
                            id = id
                        });
                    }
                }
                else
                {
                    TempData["message"] = "Your username is already in use!";
                    return RedirectToAction("RegisterStaff", new
                    {
                        StaffGUID = StaffGUID,
                        id = id
                    });
                }
            }
            else
            {
                TempData["message"] = "Please Fill In All of your Details";
                return RedirectToAction("RegisterStaff", new
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
                        return RedirectToAction("StaffHomePage", new
                        {
                            StaffGUID = StaffGUID
                        });
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
                TempData["message"] = "An Error Occured Please Login Again";
                return RedirectToAction("StaffLogin");
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
                                    return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
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
                        TempData["message"] = "The Email Entered Is Already In Use! Please Use A Different Email!";
                        return RedirectToAction("UpdateStaffInfo", new
                        { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "The Phone Number Entered Is Already In Use! Please Use A Different Number!";
                    return RedirectToAction("UpdateStaffInfo", new
                    {
                        StaffGUID = StaffGUID,
                        id = id
                    });
                }
            }
            else
            {
                TempData["message"] = "Please Fill In All The Required Details!";
                return RedirectToAction("UpdateStaffInfo", new
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
                    ViewBag.CityOrTowns = new SelectList(db.CityOrTowns, "CityOrTownID", "CityOrTownName");
                    ViewBag.Provinces = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                    ViewBag.Suburbs = new SelectList(db.Suburbs, "SuburbID", "SuburbName");
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
        [HttpPost]
        public ActionResult AddNewParty(string StaffGUID, string id, string PartyName, string PartyAcronym, string PhoneNumber, string Email, [Bind(Include = "ProvinceID, CityOrTownsID, SuburbID")] Party party, string StreetAddress, string Website)
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
                                        db.Parties.Add(CreateParty);
                                        db.SaveChanges();
                                        return RedirectToAction("PartyImages", new { StaffGUID = StaffGUID, id = id });
                                    }
                                    else
                                    {
                                        TempData["message"] = "Website is already in use, please use a different one.";
                                        return RedirectToAction("RegisterParty", new { StaffGUID = StaffGUID, id = id });
                                    }
                                }
                                else
                                {
                                    TempData["message"] = "StreetAddress is already in use, please use a different one.";
                                    return RedirectToAction("RegisterParty", new { StaffGUID = StaffGUID, id = id });
                                }
                            }
                            else
                            {
                                TempData["message"] = "Email is already in use, please use a different one.";
                                return RedirectToAction("RegisterParty", new { StaffGUID = StaffGUID, id = id });
                            }
                        }
                        else
                        {
                            TempData["message"] = "The Phone Number is already in use please use a different one.";
                            return RedirectToAction("RegisterParty", new { StaffGUID = StaffGUID, id = id });
                        }
                    }
                    else
                    {
                        TempData["message"] = "Please enter a valid phone number";
                        return RedirectToAction("RegisterParty", new { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "The party already exists, please create a new one!";
                    return RedirectToAction("RegisterParty", new { StaffGUID = StaffGUID, id = id });
                }
            }
            else
            {
                TempData["message"] = "Please fill in all the data!";
                return RedirectToAction("RegisterParty", new { StaffGUID = StaffGUID, id = id });
            }

        }

        public ActionResult PartyImages(string StaffGUID, string id, string Logo)
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
        [HttpPost]
        public ActionResult AddImages(string StaffGUID, string id, PartyImage model, HttpPostedFileBase Logo, HttpPostedFileBase LeadPic)
        {
            var db = new VotingSystemProjectEntities3(); 
            if (Logo != null && LeadPic != null)
            { 
                model.partyID = 1;
                model.PartyPicture = new byte[Logo.ContentLength];
                Logo.InputStream.Read(model.PartyPicture, 0, Logo.ContentLength);
                model.PartyLeaderPicture = new byte[LeadPic.ContentLength];
                LeadPic.InputStream.Read(model.PartyLeaderPicture, 0, LeadPic.ContentLength);
                db.PartyImages.Add(model);
                db.SaveChanges();
                return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
            }
            else
            {
                TempData["message"] = "Please select images";
                return RedirectToAction("PartyImages", new { StaffGUID = StaffGUID, id = id });
            }
            
        }
        public ActionResult UpdateOrDeleteParty(string StaffGUID, string id)
        {
            ViewBag.Parties = new SelectList(db.Parties, "PartyID", "PartyName");

            return View();
        }
        public ActionResult UpateParty(string StaffGUID, string id, [Bind(Include = "PartyID")] Party party)
        {
            if (party != null)
            {
                var p = db.Parties.Where(x => x.PartyID == party.PartyID).FirstOrDefault();
                if (p != null)
                {
                    return View(p);
                }
                else
                {
                    return RedirectToAction("UpdateOrDeleteParty");
                }
            }
            else
            {
                return RedirectToAction("UpdateOrDeleteParty");
            }

            if (party.PartyName != null && party.PartyAccronym != null && party.PartyTelephone != null && party.PartyStreetAddress != null && party.PartyEmail != null && party.PartyWebsite != null)
            {
                var ids = Convert.ToInt32(party.PartyID);
                var searchName = db.Parties.Where(x => x.PartyID != ids && x.PartyName == party.PartyName).FirstOrDefault();
                if (searchName == null)
                {
                    var searchAcronym = db.Parties.Where(j => j.PartyID != ids && j.PartyAccronym == party.PartyAccronym).FirstOrDefault();
                    if (searchAcronym == null)
                    {
                        var searchTelephone = db.Parties.Where(g => g.PartyID == ids && g.PartyTelephone == party.PartyTelephone).FirstOrDefault();
                        if (searchTelephone != null)
                        {
                            var searchStreetAddress = db.Parties.Where(g => g.PartyID == ids && g.PartyStreetAddress == party.PartyStreetAddress).FirstOrDefault();
                            if (searchStreetAddress != null)
                            {
                                var searchEmail = db.Parties.Where(g => g.PartyID == ids && g.PartyEmail == party.PartyEmail).FirstOrDefault();
                                if (searchEmail != null)
                                {
                                    var searchWebsite = db.Parties.Where(g => g.PartyID == ids && g.PartyWebsite == party.PartyWebsite).FirstOrDefault();
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
                                                party.PartyName = party.PartyName;
                                                party.PartyAccronym = party.PartyAccronym;
                                                party.PartyTelephone = party.PartyTelephone;
                                                party.PartyStreetAddress = party.PartyStreetAddress;
                                                party.PartyEmail = party.PartyEmail;
                                                party.PartyWebsite = party.PartyWebsite;
                                                db.SaveChanges();
                                                TempData["success"] = "Your Information Has Been Updated!";
                                                return RedirectToAction("StaffHomePage", new { StaffGUID = StaffGUID });
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
                        return RedirectToAction("UpdateStaffInfo", new
                        { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "The Phone Number Entered Is Already In Use! Please Use A Different Number!";
                    return RedirectToAction("UpdateStaffInfo", new
                    {
                        StaffGUID = StaffGUID,
                        id = id
                    });
                }
            }
            else
            {
                TempData["message"] = "Please Fill In All The Required Details!";
                return RedirectToAction("UpdateStaffInfo", new
                {
                    StaffGUID = StaffGUID,
                    id = id
                });
            }

    }
        public ActionResult UpdatePartyImages(string StaffGUID, string id)
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

        public ActionResult DoUpdateImages(string StaffGUID)
        {
            return RedirectToAction("StaffHomePage");
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
    }
}