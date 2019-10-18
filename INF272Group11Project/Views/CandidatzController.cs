//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;
//using INF272Group11Project.Models;

//namespace INF272Group11Project.Views
//{
//    public class CandidatzController : Controller
//    {
//        private VotingSystemProjectEntities1 db = new VotingSystemProjectEntities1();

//        GET: Candidatz
//        public ActionResult Index()
//        {
//            var candidates = db.Candidates.Include(c => c.CandidatePosition).Include(c => c.Party);
//            return View(candidates.ToList());
//        }

//        GET: Candidatz/Details/5
//        public ActionResult Details(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Candidate candidate = db.Candidates.Find(id);
//            if (candidate == null)
//            {
//                return HttpNotFound();
//            }
//            return View(candidate);
//        }

//        GET: Candidatz/Create
//        public ActionResult Create()
//        {
//            ViewBag.CandidatePosition_ID = new SelectList(db.CandidatePositions, "CandidatePosition_ID", "CandidatePosition_Description");
//            ViewBag.PartyID = new SelectList(db.Parties, "PartyID", "PartyName");
//            return View();
//        }

//        POST: Candidatz/Create
//        To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//         more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Create([Bind(Include = "Candidate_ID,CandidateFirstNames,CandidateLastName,CandidatePosition_ID,PartyID")] Candidate candidate)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Candidates.Add(candidate);
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }

//            ViewBag.CandidatePosition_ID = new SelectList(db.CandidatePositions, "CandidatePosition_ID", "CandidatePosition_Description", candidate.CandidatePosition_ID);
//            ViewBag.PartyID = new SelectList(db.Parties, "PartyID", "PartyName", candidate.PartyID);
//            return View(candidate);
//        }

//        GET: Candidatz/Edit/5
//        public ActionResult Edit(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Candidate candidate = db.Candidates.Find(id);
//            if (candidate == null)
//            {
//                return HttpNotFound();
//            }
//            ViewBag.CandidatePosition_ID = new SelectList(db.CandidatePositions, "CandidatePosition_ID", "CandidatePosition_Description", candidate.CandidatePosition_ID);
//            ViewBag.PartyID = new SelectList(db.Parties, "PartyID", "PartyName", candidate.PartyID);
//            return View(candidate);
//        }

//        POST: Candidatz/Edit/5
//         To protect from overposting attacks, please enable the specific properties you want to bind to, for 
//         more details see https://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public ActionResult Edit([Bind(Include = "Candidate_ID,CandidateFirstNames,CandidateLastName,CandidatePosition_ID,PartyID")] Candidate candidate)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Entry(candidate).State = EntityState.Modified;
//                db.SaveChanges();
//                return RedirectToAction("Index");
//            }
//            ViewBag.CandidatePosition_ID = new SelectList(db.CandidatePositions, "CandidatePosition_ID", "CandidatePosition_Description", candidate.CandidatePosition_ID);
//            ViewBag.PartyID = new SelectList(db.Parties, "PartyID", "PartyName", candidate.PartyID);
//            return View(candidate);
//        }

//        GET: Candidatz/Delete/5
//        public ActionResult Delete(int? id)
//        {
//            if (id == null)
//            {
//                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
//            }
//            Candidate candidate = db.Candidates.Find(id);
//            if (candidate == null)
//            {
//                return HttpNotFound();
//            }
//            return View(candidate);
//        }

//        POST: Candidatz/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public ActionResult DeleteConfirmed(int id)
//        {
//            Candidate candidate = db.Candidates.Find(id);
//            db.Candidates.Remove(candidate);
//            db.SaveChanges();
//            return RedirectToAction("Index");
//        }

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing)
//            {
//                db.Dispose();
//            }
//            base.Dispose(disposing);
//        }
//    }
//}
