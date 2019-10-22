using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using INF272Group11Project.Models;

namespace INF272Group11Project.Views.Candidate
{
    public class CandidatePositionsController : Controller
    {
        private VotingSystemProjectEntities3 db = new VotingSystemProjectEntities3();

        // GET: CandidatePositions
        public ActionResult Index()
        {
            return View(db.CandidatePositions.ToList());
        }

        // GET: CandidatePositions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CandidatePosition candidatePosition = db.CandidatePositions.Find(id);
            if (candidatePosition == null)
            {
                return HttpNotFound();
            }
            return View(candidatePosition);
        }

        // GET: CandidatePositions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CandidatePositions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]


        // GET: CandidatePositions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CandidatePosition candidatePosition = db.CandidatePositions.Find(id);
            if (candidatePosition == null)
            {
                return HttpNotFound();
            }
            return View(candidatePosition);
        }

        // POST: CandidatePositions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CandidatePosition_ID,CandidatePosition_Description")] CandidatePosition candidatePosition)
        {
            if (ModelState.IsValid)
            {
                db.Entry(candidatePosition).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(candidatePosition);
        }

        // GET: CandidatePositions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CandidatePosition candidatePosition = db.CandidatePositions.Find(id);
            if (candidatePosition == null)
            {
                return HttpNotFound();
            }
            return View(candidatePosition);
        }

        // POST: CandidatePositions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CandidatePosition candidatePosition = db.CandidatePositions.Find(id);
            db.CandidatePositions.Remove(candidatePosition);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
