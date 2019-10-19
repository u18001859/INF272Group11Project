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

        [HttpGet]
        public ActionResult RegisterCandidate(string Party, int? errorID, string VoterGUID)
        {
            if (errorID == 1)
            {
                ViewBag.Error1 = "Please enter all first names";
            }
            else if (errorID == 2)
            {
                ViewBag.Error2 = "Please enter a last name";
            }
            else if (errorID == 3)
            {
                ViewBag.Error3 = "Please select a party";
            }
            else if (errorID == 4)
            {
                ViewBag.Error4 = "Please select a candidate position";
            }
            else if (errorID == 5)
            {
                ViewBag.Error5 = "Please select a province";
            }

            CandidateVM vm = new CandidateVM();

            vm.Parties = GetParties(0);
            vm.Provinces = GetProvinces(0);
            vm.Positions = GetPositions(0);

            VoterVM voterVM = new VoterVM();
            if (voterVM.IsLogedIn(db, VoterGUID))
            {
                voterVM.RefreshGUID(db);
                RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                registerVoterVM.voterView = voterVM;
                registerVoterVM.VoterList = db.Voters.ToList();
                ViewBag.message = TempData["message"];
                ViewBag.message = TempData["success"];
                return View(vm);
            }
            else
            {
                VoterVM = TempData["voterVM"] as voterVM;
                if (voterVM.IsLogedIn(db))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(vm);
                }

            }

            //return View(vm);


        }

        //Method for drop down list for Parties
        private SelectList GetParties(int selected)
        {
            using (VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var party = db.Parties.Select(p => new SelectListItem
                {
                    Value = p.PartyID.ToString(),
                    Text = p.PartyName
                }).ToList();

                if (selected == 0)
                    return new SelectList(party, "Value", "Text");
                else
                    return new SelectList(party, "Value", "Text", selected);
            }
        }

        //Method for drop down list for Positions
        private SelectList GetPositions(int selected)
        {
            using (VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var pos = db.CandidatePositions.Select(p => new SelectListItem
                {
                    Value = p.CandidatePosition_ID.ToString(),
                    Text = p.CandidatePosition_Description
                }).ToList();

                if (selected == 0)
                    return new SelectList(pos, "Value", "Text");
                else
                    return new SelectList(pos, "Value", "Text", selected);
            }
        }

        //Method for drop down list for Candidates
        private SelectList GetCandidates(int selected)
        {
            using (VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var can = db.Candidates.Select(c => new SelectListItem
                {
                    Value = c.Candidate_ID.ToString(),
                    Text = c.CandidateFirstNames + " " + c.CandidateLastName 
                    //+ " (" + (from pos in db.CandidatePositions
                    //                                                                   join cand in db.Candidates
                    //                                                                   on pos.CandidatePosition_ID equals cand.CandidatePosition_ID
                    //                                                                   where (pos.CandidatePosition_ID == c.CandidatePosition_ID)
                    //                                                                   select pos.CandidatePosition_ID).ToString() + ") (" + (from part in db.Parties
                    //                                                                                                               join cand in db.Candidates
                    //                                                                                                               on part.PartyID equals cand.PartyID
                    //                                                                                                               where (part.PartyID == c.PartyID)
                    //                                                                                                               select part.PartyName).ToString() + ")"
                }).ToList();

                if (selected == 0)
                    return new SelectList(can, "Value", "Text");
                else
                    return new SelectList(can, "Value", "Text", selected);
            }
        }

        //Method for drop down list for Provinces
        private SelectList GetProvinces(int selected)
        {
            using (VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2())
            {
                db.Configuration.ProxyCreationEnabled = false;

                var prov = db.Provinces.Select(p => new SelectListItem
                {
                    Value = p.ProvinceID.ToString(),
                    Text = p.ProvinceName
                }).ToList();

                if (selected == 0)
                    return new SelectList(prov, "Value", "Text");
                else
                    return new SelectList(prov, "Value", "Text", selected);
            }
        }


        [HttpPost]
        public ActionResult AddNewCandidate(string FirstNames, string LastName, string Position, string Party, string submitButton, string Province/*, CandidateVM model*/)
        {
            switch (submitButton)
            {
                case "Register":
                    //Validation: Prevents candidate from being registered if fields are left blank
                    if (String.IsNullOrEmpty(FirstNames))
                    {
                        return RedirectToAction("RegisterCandidate", "Candidate", new { @errorID = 1 });
                    }
                    if (String.IsNullOrEmpty(LastName))
                    {
                        return RedirectToAction("RegisterCandidate", "Candidate", new { @errorID = 2 });
                    }
                    if (String.IsNullOrEmpty(Party))
                    {
                        return RedirectToAction("RegisterCandidate", "Candidate", new { @errorID = 3 });
                    }
                    if (String.IsNullOrEmpty(Position))
                    {
                        return RedirectToAction("RegisterCandidate", "Candidate", new { @errorID = 4 });
                    }
                    if (String.IsNullOrEmpty(Province))
                    {
                        return RedirectToAction("RegisterCandidate", "Candidate", new { @errorID = 5 });
                    }
                    else
                    {
                        Candidate c = new Models.Candidate();

                        c.CandidateFirstNames = FirstNames /*model.CandidateFirstNames*/;
                        c.CandidateLastName = LastName /*model.CandidateLastName*/;
                        c.CandidatePosition_ID = Convert.ToInt32(Position /*model.SelectedCandidatePosition_ID*/);
                        c.PartyID = Convert.ToInt32(Party /*model.SelectedPartyID*/);
                        c.ProvinceID = Convert.ToInt32(Province /*model.SelectedProvinceID*/);

                        try
                        {

                            if (ModelState.IsValid)
                            {
                                db.Candidates.Add(c);
                                db.SaveChanges();
                            }

                            TempData["Message"] = "New candidate successfully added!";
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                        ve.PropertyName,
                                        eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                        ve.ErrorMessage);
                                }
                            }
                            throw;
                        }

                        return RedirectToAction("StaffHomePage", "Staff");
                    }
                case "Cancel":
                    return RedirectToAction("StaffHomePage", "Staff");
                default:
                    return RedirectToAction("StaffHomePage", "Staff");
            }

        }

        [HttpGet]
        public ActionResult UpdateOrDeleteCandidate(int? errorID, string VoterGUID)
        {
            //if (errorID == 1)
            //{
            //    ViewBag.Error1 = "Please select a party";
            //}
            if (errorID == 2)
            {
                ViewBag.Error2 = "Please select a candidate";
            }

            CandidateVM vm = new CandidateVM();

            //vm.Parties = GetParties(0);
            vm.Candidates = GetCandidates(0);


            VoterVM voterVM = new VoterVM();
            if (voterVM.IsLogedIn(db, VoterGUID))
            {
                voterVM.RefreshGUID(db);
                RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                registerVoterVM.voterView = voterVM;
                registerVoterVM.VoterList = db.Voters.ToList();
                ViewBag.message = TempData["message"];
                ViewBag.message = TempData["success"];
                return View(vm);
            }
            else
            {
                VoterVM = TempData["voterVM"] as voterVM;
                if (voterVM.IsLogedIn(db))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(vm);
                }

            }

            //return View(vm);
        }

        public ActionResult Choice(/*string PartyName,*/ string CandidateID, string submitButton)
        {
            switch (submitButton)
            {
                case "Update":
                    //Validation: Prevents candidate from being registered if fields are left blank

                    //if (String.IsNullOrEmpty(PartyName))
                    //{
                    //    return RedirectToAction("UpdateOrDeleteCandidate", "Candidate", new { @errorID = 1 });
                    //}
                    if (String.IsNullOrEmpty(CandidateID))
                    {
                        return RedirectToAction("UpdateOrDeleteCandidate", "Candidate", new { @errorID = 2 });
                    }
                    else
                    {
                        return RedirectToAction("UpdateCandidate", "Candidate", new { @CandidateID = CandidateID });
                    }
                case "Delete":
                    if (String.IsNullOrEmpty(CandidateID))
                    {
                        return RedirectToAction("UpdateOrDeleteCandidate", "Candidate", new { @errorID = 2 });
                    }
                    else
                    {
                        return RedirectToAction("DeleteCandidate", "Candidate", new { @CandidateID = CandidateID });
                    }
                case "Cancel":
                    return RedirectToAction("StaffHomePage", "Staff");
                default:
                    return RedirectToAction("StaffHomePage", "Staff");
            }
        }

        [HttpGet]
        public ActionResult UpdateCandidate(string CandidateID, int? errorID, string VoterGUID)
        {
            //viewbags to fill fields with already existing data
            if (errorID == 1)
            {
                ViewBag.Error1 = "Please enter all first names";
            }
            else if (errorID == 2)
            {
                ViewBag.Error2 = "Please enter a last name";
            }
            else if (errorID == 3)
            {
                ViewBag.Error3 = "Please select a party";
            }
            else if (errorID == 4)
            {
                ViewBag.Error4 = "Please select a candidate position";
            }
            else if (errorID == 5)
            {
                ViewBag.Error5 = "Please select a province";
            }

            TempData["CandidateEditID"] = CandidateID;

            CandidateVM vm = new CandidateVM();

            vm.Parties = GetParties(0);
            vm.Positions = GetPositions(0);
            vm.Provinces = GetProvinces(0);

            VoterVM voterVM = new VoterVM();
            if (voterVM.IsLogedIn(db, VoterGUID))
            {
                voterVM.RefreshGUID(db);
                RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                registerVoterVM.voterView = voterVM;
                registerVoterVM.VoterList = db.Voters.ToList();
                ViewBag.message = TempData["message"];
                ViewBag.message = TempData["success"];
                return View(vm);
            }
            else
            {
                VoterVM = TempData["voterVM"] as voterVM;
                if (voterVM.IsLogedIn(db))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(vm);
                }

            }

            //return View(vm);
        }

        [HttpPost]
        public ActionResult Update(string CandidateID, string FirstNames, string LastName, string Position, string Party, string Province, string submitButton)
        {
            switch (submitButton)
            {
                case "Update":
                    //Validation: Prevents candidate from being registered if fields are left blank
                    if (String.IsNullOrEmpty(FirstNames))
                    {
                        return RedirectToAction("UpdateCandidate", "Candidate", new { @errorID = 1 });
                    }
                    if (String.IsNullOrEmpty(LastName))
                    {
                        return RedirectToAction("UpdateCandidate", "Candidate", new { @errorID = 2 });
                    }
                    if (String.IsNullOrEmpty(Party))
                    {
                        return RedirectToAction("UpdateCandidate", "Candidate", new { @errorID = 3 });
                    }
                    if (String.IsNullOrEmpty(Position))
                    {
                        return RedirectToAction("UpdateCandidate", "Candidate", new { @errorID = 4 });
                    }
                    if (String.IsNullOrEmpty(Province))
                    {
                        return RedirectToAction("UpdateCandidate", "Candidate", new { @errorID = 5 });
                    }
                    else
                    {
                        int CanID = Convert.ToInt32(CandidateID);

                        Candidate c = db.Candidates.Where(i => i.Candidate_ID == CanID).FirstOrDefault();

                        c.CandidateFirstNames = FirstNames;
                        c.CandidateLastName = LastName;
                        c.CandidatePosition_ID = Convert.ToInt32(Position);
                        c.PartyID = Convert.ToInt32(Party);
                        c.ProvinceID = Convert.ToInt32(Province);

                        try
                        {

                            if (ModelState.IsValid)
                            {
                                db.Entry(c).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["Message"] = "Candidate successfully edited!";
                            }

                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                        ve.PropertyName,
                                        eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                        ve.ErrorMessage);
                                }
                            }
                            throw;
                        }
                        return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
                    }
                case "Back":
                    return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
                default:
                    return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
            }
        }

        [HttpGet]
        public ActionResult DeleteCandidate(string CandidateID, string VoterGUID)
        {
            if (CandidateID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Candidate candidate = db.Candidates.Find(Convert.ToInt32(CandidateID));
            if (candidate == null)
            {
                return HttpNotFound();
            }

            TempData["CandidateEditID"] = CandidateID;

            VoterVM voterVM = new VoterVM();
            if (voterVM.IsLogedIn(db, VoterGUID))
            {
                voterVM.RefreshGUID(db);
                RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                registerVoterVM.voterView = voterVM;
                registerVoterVM.VoterList = db.Voters.ToList();
                ViewBag.message = TempData["message"];
                ViewBag.message = TempData["success"];
                return View(candidate);
            }
            else
            {
                VoterVM = TempData["voterVM"] as voterVM;
                if (voterVM.IsLogedIn(db))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(candidate);
                }

            }

            //return View(candidate);
        }

        [HttpPost]
        public ActionResult Delete(string Candidate_ID)
        {
            Candidate candidate = db.Candidates.Find(Convert.ToInt32(Candidate_ID));
            if (candidate == null)
            {
                return HttpNotFound();
            }
            else
            {
                db.Candidates.Remove(candidate);
                db.SaveChanges();
                TempData["Message"] = "Candidate successfully deleted!";
                return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
            }
        }

        //CANDIDATE POSITION CRUD

        public ActionResult AddCandidatePosition(int? errorID, string VoterGUID)
        {
            if (errorID == 1)
            {
                ViewBag.Error1 = "Please enter a candidate position";
            }

            VoterVM voterVM = new VoterVM();
            if (voterVM.IsLogedIn(db, VoterGUID))
            {
                voterVM.RefreshGUID(db);
                RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                registerVoterVM.voterView = voterVM;
                registerVoterVM.VoterList = db.Voters.ToList();
                ViewBag.message = TempData["message"];
                ViewBag.message = TempData["success"];
                return View();
            }
            else
            {
                VoterVM = TempData["voterVM"] as voterVM;
                if (voterVM.IsLogedIn(db))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View();
                }

            }
            //return View();
        }

        [HttpPost]
        public ActionResult Create(string PositionName, string submitButton)
        {
            switch (submitButton)
            {
                case "Add":
                    //Validation: Prevents candidate from being registered if fields are left blank
                    if (String.IsNullOrEmpty(PositionName))
                    {
                        return RedirectToAction("AddCandidatePosition", "Candidate", new { @errorID = 1 });
                    }
                    else
                    {
                        CandidatePosition cp = new Models.CandidatePosition();

                        cp.CandidatePosition_Description = PositionName;


                        try
                        {
                            if (ModelState.IsValid)
                            {
                                db.CandidatePositions.Add(cp);
                                db.SaveChanges();
                                TempData["Message"] = "New candidate position successfully added!";
                            }

                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                        ve.PropertyName,
                                        eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                        ve.ErrorMessage);
                                }
                            }
                            throw;
                        }

                        return RedirectToAction("StaffHomePage", "Staff");
                    }
                case "Cancel":
                    return RedirectToAction("StaffHomePage", "Staff");
                default:
                    return RedirectToAction("StaffHomePage", "Staff");
            }
        }

        [HttpGet]
        public ActionResult UpdateOrDeleteCandidatePosition(int? errorID, string VoterGUID)
        {
            if (errorID == 1)
            {
                ViewBag.Error1 = "Please select a candidate position";
            }

            CandidatePositionVM vm2 = new CandidatePositionVM();

            vm2.Positions = GetPositions(0);

            VoterVM voterVM = new VoterVM();
            if (voterVM.IsLogedIn(db, VoterGUID))
            {
                voterVM.RefreshGUID(db);
                RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                registerVoterVM.voterView = voterVM;
                registerVoterVM.VoterList = db.Voters.ToList();
                ViewBag.message = TempData["message"];
                ViewBag.message = TempData["success"];
                return View(vm2);
            }
            else
            {
                VoterVM = TempData["voterVM"] as voterVM;
                if (voterVM.IsLogedIn(db))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(vm2);
                }

            }

            //return View(vm2);
        }

        public ActionResult Choice2(string CandidatePositionID, string submitButton)
        {
            switch (submitButton)
            {
                case "Update":
                    //Validation: Prevents candidate from being registered if fields are left blank

                    if (String.IsNullOrEmpty(CandidatePositionID))
                    {
                        return RedirectToAction("UpdateOrDeleteCandidatePosition", "Candidate", new { @errorID = 1 });
                    }
                    else
                    {
                        return RedirectToAction("UpdateCandidatePosition", "Candidate", new { @CandidatePositionID = CandidatePositionID });
                    }
                case "Delete":
                    if (String.IsNullOrEmpty(CandidatePositionID))
                    {
                        return RedirectToAction("UpdateOrDeleteCandidatePosition", "Candidate", new { @errorID = 1 });
                    }
                    else
                    {
                        return RedirectToAction("DeleteCandidatePosition", "Candidate", new { @CandidatePositionID = CandidatePositionID });
                    }
                case "Cancel":
                    return RedirectToAction("StaffHomePage", "Staff");
                default:
                    return RedirectToAction("StaffHomePage", "Staff");

            }
        }

        [HttpGet]
        public ActionResult UpdateCandidatePosition(string CandidatePositionID, int? errorID, string VoterGUID)
        {
            //viewbags to fill fields with already existing data
            if (errorID == 1)
            {
                ViewBag.Error1 = "Please select a candidate position";
            }

            TempData["CandidatePositionEditID"] = CandidatePositionID;

            CandidatePositionVM vm2 = new CandidatePositionVM();

            vm2.Positions = GetPositions(0);

            VoterVM voterVM = new VoterVM();
            if (voterVM.IsLogedIn(db, VoterGUID))
            {
                voterVM.RefreshGUID(db);
                RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                registerVoterVM.voterView = voterVM;
                registerVoterVM.VoterList = db.Voters.ToList();
                ViewBag.message = TempData["message"];
                ViewBag.message = TempData["success"];
                return View(vm2);
            }
            else
            {
                VoterVM = TempData["voterVM"] as voterVM;
                if (voterVM.IsLogedIn(db))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(vm2);
                }

            }

            //return View(vm2);
    }

        [HttpPost]
        public ActionResult Update2(string CandidatePositionID, string PositionName, string submitButton)
        {
            switch (submitButton)
            {
                case "Update":
                    //Validation: Prevents candidate from being registered if fields are left blank
                    if (String.IsNullOrEmpty(PositionName))
                    {
                        return RedirectToAction("UpdateCandidatePosition", "Candidate", new { @errorID = 1 });
                    }
                    else
                    {
                        int CanPosID = Convert.ToInt32(CandidatePositionID);

                        CandidatePosition cp = db.CandidatePositions.Where(i => i.CandidatePosition_ID == CanPosID).FirstOrDefault();

                        cp.CandidatePosition_Description = PositionName;

                        try
                        {

                            if (ModelState.IsValid)
                            {
                                db.Entry(cp).State = EntityState.Modified;
                                db.SaveChanges();
                                TempData["Message"] = "Candidate position successfully edited!";
                            }

                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Value: \"{1}\", Error: \"{2}\"",
                                        ve.PropertyName,
                                        eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName),
                                        ve.ErrorMessage);
                                }
                            }
                            throw;
                        }
                        return RedirectToAction("UpdateOrDeleteCandidatePosition", "Candidate");
                    }
                case "Back":
                    return RedirectToAction("UpdateOrDeleteCandidatePosition", "Candidate");
                default:
                    return RedirectToAction("UpdateOrDeleteCandidatePosition", "Candidate");
            }
        }

        [HttpGet]
        public ActionResult DeleteCandidatePosition(string CandidatePositionID, string VoterGUID)
        {
            if (CandidatePositionID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CandidatePosition position = db.CandidatePositions.Find(Convert.ToInt32(CandidatePositionID));
            if (position == null)
            {
                return HttpNotFound();
            }

            TempData["CandidatePositionEditID"] = CandidatePositionID;

            VoterVM voterVM = new VoterVM();
            if (voterVM.IsLogedIn(db, VoterGUID))
            {
                voterVM.RefreshGUID(db);
                RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                registerVoterVM.voterView = voterVM;
                registerVoterVM.VoterList = db.Voters.ToList();
                ViewBag.message = TempData["message"];
                ViewBag.message = TempData["success"];
                return View(position);
            }
            else
            {
                VoterVM = TempData["voterVM"] as voterVM;
                if (voterVM.IsLogedIn(db))
                {
                    voterVM.RefreshGUID(db);
                    RegisterVoterVM registerVoterVM = new RegisterVoterVM();
                    registerVoterVM.voterView = voterVM;
                    registerVoterVM.VoterList = db.Voters.ToList();
                    ViewBag.message = TempData["message"];
                    ViewBag.success = TempData["success"];
                    return View(position);
                }

            }

            //return View(position);
    }

        [HttpPost]
        public ActionResult Delete2(string CandidatePositionID)
        {
            CandidatePosition position = db.CandidatePositions.Find(Convert.ToInt32(CandidatePositionID));
            if (position == null)
            {
                return HttpNotFound();
            }
            else
            {
                //var candidates = db.Candidates.Where(y => y.Candidate_ID == position.CandidatePosition_ID).AsEnumerable();
                //foreach (var can in candidates)
                //{
                //    var c = can;
                //    candidates.
                //    candidates.Remove(c);
                //}
                //db.CandidatePositions.DeleteObject(author);

                //db.Candidates
                try
                {
                    db.CandidatePositions.Remove(position);
                    db.SaveChanges();
                    TempData["Message"] = "Candidate position successfully deleted!";
                }
                catch(Exception e)
                {
                    TempData["Message"] = "Error: " + e;
                }


               
                return RedirectToAction("UpdateOrDeleteCandidatePosition", "Candidate");
            }
        }
    }
}