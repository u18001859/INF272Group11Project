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
        VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2();

        // GET: Candidate

        [HttpGet]
        public ActionResult RegisterCandidate(string Party, int? errorID)
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
            vm.Positions = GetPositions(0);
            vm.Provinces = GetProvinces(0);

            return View(vm);
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
                    Text = c.CandidateFirstNames + " " + c.CandidateLastName /*+ " " + db.Candidates.Where(a => a.CandidatePosition_ID = db.Candidates.Select(p => p.CandidatePosition_ID))*/
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

                        //catch (Exception e)
                        //{
                        //    TempData["Message"] = "Add candidate error:" + e;
                        //}

                        return RedirectToAction("StaffHomePage", "Staff");
                    }
                case "Cancel":
                    return RedirectToAction("StaffHomePage", "Staff");
                default:
                    return RedirectToAction("StaffHomePage", "Staff");
            }

        }

        [HttpGet]
        public ActionResult UpdateOrDeleteCandidate(int? errorID)
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

            return View(vm);
        }

        public ActionResult Choice(/*string PartyName,*/ string Candidate, string submitButton)
        {
            switch (submitButton)
            {
                case "Update":
                    //Validation: Prevents candidate from being registered if fields are left blank
                    //if (String.IsNullOrEmpty(PartyName))
                    //{
                    //    return RedirectToAction("UpdateOrDeleteCandidate", "Candidate", new { @errorID = 1 });
                    //}
                    if (String.IsNullOrEmpty(Candidate))
                    {
                        return RedirectToAction("UpdateOrDeleteCandidate", "Candidate", new { @errorID = 2 });
                    }
                    else
                    {
                        return RedirectToAction("UpdateCandidate", "Candidate", new {@CandidateID = Candidate});
                    }
                case "Delete":
                    if (String.IsNullOrEmpty(Candidate))
                    {
                        return RedirectToAction("UpdateOrDeleteCandidate", "Candidate", new { @errorID = 2 });
                    }
                    else
                    {
                        return RedirectToAction("UpdateCandidate", "Candidate", new {@CandidateID = Candidate});
                    }
                case "Cancel":
                    return RedirectToAction("StaffHomePage", "Staff");
                default:
                    return RedirectToAction("StaffHomePage", "Staff");
            }
        }

        [HttpGet]
        public ActionResult UpdateCandidate(string CandidateID, string firstNames, string lastName, string party, string position, int? errorID)
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

            ViewBag.ID = CandidateID;

            TempData["CandidateEditID"] = CandidateID;

            CandidateVM vm = new CandidateVM();

            vm.Parties = GetParties(0);
            vm.Positions = GetPositions(0);
            vm.Provinces = GetProvinces(0);

            return View(vm);
        }

        [HttpPost]
        public ActionResult Update(Candidate candidate, string CandidateID, string FirstNames, string LastName, string Party, string Position, string Province, string submitButton)
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
                        int CanID = 0;

                        if (int.TryParse(CandidateID, out CanID))
                        {
                             ViewBag.Num = CandidateID;
                        }

                        Candidate c = db.Candidates.Where(i => i.CandidatePosition_ID == CanID).FirstOrDefault();



                        try
                        {

                            if (ModelState.IsValid)
                            {
                                c.CandidateFirstNames = FirstNames;
                                c.CandidateLastName = LastName;
                                c.CandidatePosition_ID = Convert.ToInt32(Position);
                                c.PartyID = Convert.ToInt32(Party);
                                c.ProvinceID = Convert.ToInt32(Province);

                                db.SaveChanges();
                            }

                            TempData["Message"] = "Candidate successfully edited!";
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

        public ActionResult Delete()
        {
            return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
        }

        public ActionResult AddCandidatePosition()
        {
            return View();
        }

        public ActionResult Create([Bind(Include = "CandidatePosition_ID,CandidatePosition_Description")] CandidatePosition candidatePosition)
        {
            if (ModelState.IsValid)
            {
                db.CandidatePositions.Add(candidatePosition);
                db.SaveChanges();
                return RedirectToAction("StaffHomePage", "Staff");
            }

            return View(candidatePosition);
        }
    }
}