using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;
using System.Net;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace INF272Group11Project.Controllers
{
    public class CandidateController : Controller
    {
        //CANDIDATE CRUD

        VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2();

        // GET: Candidate


        public ActionResult RegisterCandidate(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    CandidateVM candidateVM = new CandidateVM();
                    candidateVM.StaffView = staffGUID;
                    ViewBag.PartyID = new SelectList(db.Parties, "PartyID", "PartyName");
                    ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                    ViewBag.CandidatePosition_ID = new SelectList(db.CandidatePositions, "CandidatePostion_ID", "CandidatePosition_Description");
                    return View(candidateVM);
                }
                else
                {
                    TempData["message"] = "Your Session Has Expired! Please Login Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired! Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }



        [HttpPost]
        public ActionResult AddNewCandidate(string StaffGUID, string id, string FirstNames, string LastName, [Bind(Include = "ProvinceID, CandidatePosition_ID, PartyID")]Candidate candidate)
        {
            if (StaffGUID != null)
            {
                if (FirstNames != null && LastName != null && candidate.PartyID != null && candidate.ProvinceID != null && candidate.CandidatePosition_ID != null)
                {
                    var search = db.Candidates.Where(x => x.CandidatePosition_ID == candidate.CandidatePosition_ID && x.PartyID == candidate.PartyID).FirstOrDefault();
                    if (search != null)
                    {
                        Candidate candidate1 = new Candidate();
                        candidate1.CandidateFirstNames = FirstNames;
                        candidate1.CandidateLastName = LastName;
                        candidate1.CandidatePosition_ID = candidate.CandidatePosition_ID;
                        candidate1.ProvinceID = candidate.ProvinceID;
                        candidate1.PartyID = candidate.PartyID;
                        db.Candidates.Add(candidate1);
                        db.SaveChanges();
                        TempData["success"] = "The Candidate Has Been Registered";
                        return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID });
                    }
                    else
                    {
                        TempData["message"] = "This Candidate Already Exists for this party Please Update the Candidate Instead";
                        return RedirectToAction("RegisterCandidate", new { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "Please Ensure That All Information Has Been Enetred";
                    return RedirectToAction("RegisterCandidate", new { StaffGUID = StaffGUID, id = id });
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired! Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }

        }

        [HttpGet]
        public ActionResult UpdateOrDeleteCandidate(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    CandidateVM candidateVM = new CandidateVM();
                    candidateVM.StaffView = staffGUID;
                    candidateVM.candidate = TempData["CanSearch"] as Candidate;
                    ViewBag.PartyID = new SelectList(db.Parties, "PartyID", "PartyName");
                    ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                    ViewBag.CandidatePosition_ID = new SelectList(db.CandidatePositions, "CandidatePostion_ID", "CandidatePosition_Description");
                    return View(candidateVM);
                }
                else
                {
                    TempData["message"] = "Your Session Has Expired! Please Login Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired! Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }

        public ActionResult SearchCandidate(string StaffGUID, string id, [Bind(Include = "ProvinceID, CandidatePosition_ID, PartyID")]Candidate candidate)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    CandidateVM candidateVM = new CandidateVM();
                    candidateVM.StaffView = staffGUID;

                    if (candidate.PartyID != null && candidate.ProvinceID != null && candidate.CandidatePosition_ID != null)
                    {
                        var search = db.Candidates.Where(x => x.CandidatePosition_ID == candidate.CandidatePosition_ID && x.ProvinceID == candidate.ProvinceID && x.PartyID == candidate.PartyID).FirstOrDefault();
                        if (search != null)
                        {
                            TempData["CanSearch"] = search;
                            return RedirectToAction("UpdateOrDeleteCandidate", new { StaffGUID = StaffGUID, id = id });
                        }
                        else
                        {
                            TempData["message"] = "The Candidate was not found, Please Try Again!";
                            return RedirectToAction("UpdateOrDeleteCandidate", new { StaffGUID = StaffGUID, id = id });
                        }

                    }
                    else
                    {
                        TempData["message"] = "Please Ensure That You have Entered All Your Data";
                        return RedirectToAction("UpdateOrDeleteCandidate", new { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "Your Session Has Expired! Please Login Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired! Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }

        [HttpPost]
        public ActionResult UpdateCandidate(string CandidateID, string id, string StaffGUID)
        {
            //viewbags to fill fields with already existing data

            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    CandidateVM candidateVM = new CandidateVM();
                    candidateVM.StaffView = staffGUID;
                    var ids = Convert.ToInt32(CandidateID);
                    var search = db.Candidates.Where(x => x.Candidate_ID == ids).FirstOrDefault();
                    if (search != null)
                    {
                        candidateVM.candidate = search;
                        ViewBag.PartyID = new SelectList(db.Parties, "PartyID", "PartyName");
                        ViewBag.ProvinceID = new SelectList(db.Provinces, "ProvinceID", "ProvinceName");
                        ViewBag.CandidatePosition_ID = new SelectList(db.CandidatePositions, "CandidatePostion_ID", "CandidatePosition_Description");
                        return View(candidateVM);
                    }
                    else
                    {
                        TempData["message"] = "The Candidate Was Not Found Please Try Again!";
                        return RedirectToAction("UpdateOrDeleteCandidate", new { StaffGUID = candidateVM.StaffView.staff.GUID, id = candidateVM.StaffView.staff.StaffID });
                    }
                }
                else
                {
                    TempData["message"] = "Your Session Has Expired! Please Login Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired! Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }

        [HttpPost]
        public ActionResult Update(string CandidateID, string FirstNames, string LastName, string StaffGUID, string id, [Bind(Include = "ProvinceID, CandidatePosition_ID, PartyID")]Candidate candidate)
        {
            if (CandidateID != null && FirstNames != null && LastName != null && candidate.CandidatePosition_ID != null && candidate.PartyID != null && candidate.ProvinceID != null)
            {

                if (StaffGUID != null)
                {
                    StaffGUIDControl staffGUID = new StaffGUIDControl();
                    if (staffGUID.IsLogedIn(db, StaffGUID))
                    {
                        staffGUID.RefreshGUID(db);
                        CandidateVM candidateVM = new CandidateVM();
                        candidateVM.StaffView = staffGUID;
                        var ids = Convert.ToInt32(CandidateID);
                        Candidate s = db.Candidates.Where(x => x.Candidate_ID == ids).FirstOrDefault();
                        if (s != null)
                        {
                            var search = db.Candidates.Where(j => j.Candidate_ID != ids && j.PartyID == candidate.PartyID && j.CandidatePosition_ID == candidate.CandidatePosition_ID && j.ProvinceID == candidate.ProvinceID).FirstOrDefault();
                            if (search == null)
                            {
                                s.CandidateFirstNames = FirstNames;
                                s.CandidateLastName = LastName;
                                s.CandidatePosition_ID = candidate.CandidatePosition_ID;
                                s.ProvinceID = candidate.ProvinceID;
                                s.PartyID = candidate.PartyID;
                                db.SaveChanges();
                                TempData["success"] = "The Candidate Was Successfully Updated";
                                return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID = StaffGUID, id = id });
                            }
                            else
                            {
                                TempData["message"] = "The Information that has been entered already exists!";
                                return RedirectToAction("UpdateCandidate", new { staffGUID = StaffGUID, id = id, CandidateID = CandidateID });
                            }
                        }
                        else
                        {
                            TempData["message"] = "The Candidate Was Not Found, Please Try Again";
                            return RedirectToAction("UpdateOrDeleteCandidate", new { staffGUID = StaffGUID, id = id });
                        }

                    }
                    else
                    {
                        TempData["message"] = "Your Session Has Expired! Please Login Again!";
                        return RedirectToAction("StaffLogin", "Staff");
                    }
                }
                else
                {
                    TempData["message"] = "Your Session Has Expired! Please Login Again!";
                    return RedirectToAction("StaffLogin", "Staff");
                }
            }
            else
            {
                TempData["message"] = "Please Ensure All Fields Contain Information!";
                return RedirectToAction("UpdateCandidate", new { StaffGUID = StaffGUID, CandidateID = CandidateID, id = id });
            }

        }

        [HttpPost]
        public ActionResult DeleteCandidate(string CandidateID, string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                if (CandidateID != null)
                {
                    var ids = Convert.ToInt32(CandidateID);
                    Candidate search = db.Candidates.Where(x => x.Candidate_ID == ids).FirstOrDefault();
                    if (search != null)
                    {
                        db.Candidates.Remove(search);
                        db.SaveChanges();
                        TempData["success"] = "The Candidate Has Been Removed!";
                        return RedirectToAction("StaffHomePage", "Staff", new { StaffGUID });
                    }
                    else
                    {
                        TempData["message"] = "The Candidate Was Not Found, Please Try Again";
                        return RedirectToAction("UpdateOrDeleteCandidate", new { StaffGUID = StaffGUID, id = id });
                    }
                }
                else
                {
                    TempData["message"] = "The Candidate Was Not Found, Please Try Again";
                    return RedirectToAction("UpdateOrDeleteCandidate", new { StaffGUID = StaffGUID, id = id });
                }
            }
            else
            {
                TempData["message"] = "Your Session Has Expired! Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }


    }
}
