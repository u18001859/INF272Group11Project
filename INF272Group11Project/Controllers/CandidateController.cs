using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Security.Cryptography;

namespace INF272Group11Project.Controllers
{
    public class CandidateController : Controller
    {
        VotingSystemProjectEntities db = new VotingSystemProjectEntities();

        // GET: Candidate

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
            return View();
        }

        [HttpPost]
        public ActionResult AddNewCandidate(string FirstNames, string LastName, string Party, string Position, string submitButton, CandidateVM model)
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
                    else
                    {
                        Candidate c = new Models.Candidate();

                        c.Candidate_ID = model.Candidate_ID;
                        c.CandidateFirstNames = model.CandidateFirstNames;
                        c.CandidateLastName = model.CandidateLastName;
                        c.CandidatePosition_ID = model.CandidatePosition_ID;
                        c.PartyID = model.PartyID;

                        try
                        {

                            if (ModelState.IsValid)
                            {
                                db.Candidates.Add(c);
                                db.SaveChanges();
                            }

                            TempData["Message"] = "New candidate successfully added!";
                        }
                        catch (Exception e)
                        {
                            TempData["Message"] = "Add candidate error:" + e;
                        }

                        return RedirectToAction("StaffHomePage", "Staff");
                    }
                case "Cancel":
                    return RedirectToAction("StaffHomePage", "Staff");
                default:
                    return RedirectToAction("StaffHomePage", "Staff");
            }

        }


        public ActionResult UpdateOrDeleteCandidate(int? errorID)
        {
            if (errorID == 1)
            {
                ViewBag.Error1 = "Please select a party";
            }
            else if (errorID == 2)
            {
                ViewBag.Error2 = "Please select a candidate";
            }
            return View();
        }

        public ActionResult Choice(string PartyName, string Candidate, string submitButton)
        {
            switch (submitButton)
            {
                case "Update":
                    //Validation: Prevents candidate from being registered if fields are left blank
                    if (String.IsNullOrEmpty(PartyName))
                    {
                        return RedirectToAction("UpdateOrDeleteCandidate", "Candidate", new { @errorID = 1 });
                    }
                    if (String.IsNullOrEmpty(Candidate))
                    {
                        return RedirectToAction("UpdateOrDeleteCandidate", "Candidate", new { @errorID = 2 });
                    }
                    else
                    {
                        //search here 
                        return RedirectToAction("UpdateCandidate", "Candidate"/*, new {@ID = (id), Id@firstNames = (FirstNames), @lastNames = (LastName), @position = (Position), @party = (Party)*/);
                    }
                case "Delete":
                    return RedirectToAction("Delete", "Candidate");
                case "Cancel":
                    return RedirectToAction("StaffHomePage", "Staff");
                default:
                    return RedirectToAction("StaffHomePage", "Staff");
            }
        }

        public ActionResult UpdateCandidate(string ID, string firstNames, string lastName, string party, string position, int? errorID)
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
            return View();
        }

        public ActionResult Update(string FirstNames, string LastName, string Party, string Position, string submitButton)
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
                    else
                    {
                        return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
                    }
                case "Cancel":
                    return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
                default:
                    return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
            }
        }

        public ActionResult Delete()
        {
            return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
        }
    }
}