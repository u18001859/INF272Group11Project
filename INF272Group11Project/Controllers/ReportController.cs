using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;
using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;
using INF272Group11Project.Report;
using static INF272Group11Project.ViewModels.ReportViewModel;
using System.Data;

//Apparently I need help with this one according to Seb so Im going to leave it for now
namespace INF272Group11Project.Controllers
{
    public class ReportController : Controller
    {
        VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2();
        // GET: Report
        public ActionResult Index(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    ReportViewModel reportView = new ReportViewModel();
                    reportView.StaffView = staffGUID;
                    return View(reportView);
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
                return RedirectToAction("StaffLogin","Staff");
            }
        }
        [HttpPost]
        public ActionResult ProvincialReport(string StaffGUID, string id)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    ReportViewModel reportView = new ReportViewModel();
                    reportView.StaffView = staffGUID;
                    return View(reportView);
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
        
        public ActionResult MakeProvincialReport(string StaffGUID, string id, ReportViewModel reportView)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);

                    reportView.StaffView = staffGUID;

                    using (VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2())
                    {
                        db.Configuration.ProxyCreationEnabled = false;

                        var list = db.ProvincialResults.Include("Election").Where(x => x.Election.ElectionDate >= reportView.StartDate && x.Election.ElectionDate <= reportView.EndDate).ToList().Select(f => new ProvincialReports { ElectionDate = db.Elections.Where(d => d.ElectionID == f.ElectionID).Select(j => j.ElectionDate).FirstOrDefault(), PartyName = db.Parties.Where(c => c.PartyID == f.PartyID).Select(z => z.PartyName).FirstOrDefault(), CandidateName = db.Candidates.Where(h => h.PartyID == f.PartyID).Select(a => a.CandidateFirstNames + " " + a.CandidateLastName).FirstOrDefault(), TotalVotes = db.NationalResults.Count(l => l.ElectionID == f.Election.ElectionID && l.PartyID == f.PartyID), VotePercentage = Convert.ToDouble(db.Elections.Where(n => n.ElectionDate == f.Election.ElectionDate).Select(a => a.TotalVotes).FirstOrDefault()), ProvinceName = db.Provinces.Where(q => q.ProvinceID == f.ProvinceID).Select(w => w.ProvinceName).FirstOrDefault()});

                        reportView.provincialReports = list.GroupBy(s => s.ElectionDate.ToString()).ToList();

                        reportView.data = list.GroupBy(k => k.ElectionDate.ToString()).ToDictionary(k => k.Key, k => k.Sum(v => v.TotalVotes));
                        TempData["data"] = reportView.data;
                        TempData["record"] = list.ToList();
                        return View(reportView);
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
                TempData["message"] = "Your Session Has Expired Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }
        [HttpPost]
        public ActionResult NationalReport(string StaffGUID, string id)
        {
            ViewBag.message = TempData["message"];
            ViewBag.success = TempData["success"];
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    ReportViewModel reportView = new ReportViewModel();
                    reportView.StaffView = staffGUID;
                    reportView.StartDate = new DateTime(2016, 1,1);
                    reportView.EndDate = new DateTime(2019,12,31);
                    return View(reportView);
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

        public ActionResult MakeNationalReport(string StaffGUID, string id,ReportViewModel reportView)
        {
            if (StaffGUID != null)
            {
                StaffGUIDControl staffGUID = new StaffGUIDControl();
                if (staffGUID.IsLogedIn(db, StaffGUID))
                {
                    staffGUID.RefreshGUID(db);
                    
                    reportView.StaffView = staffGUID;
                    
                    using(VotingSystemProjectEntities2 db = new VotingSystemProjectEntities2())
                    {
                        db.Configuration.ProxyCreationEnabled = false;
                        
                        var list = db.NationalResults.Include("Election").Where(x => x.Election.ElectionDate >= reportView.StartDate && x.Election.ElectionDate <= reportView.EndDate).ToList().Select(f => new NationalReports {ElectionDate = db.Elections.Where(d => d.ElectionID == f.ElectionID).Select(j => j.ElectionDate).FirstOrDefault(), PartyName = db.Parties.Where(c => c.PartyID == f.PartyID).Select(z => z.PartyName).FirstOrDefault(), TotalVotes = db.NationalResults.Count(l => l.ElectionID == f.Election.ElectionID && l.PartyID == f.PartyID), VotePercentage = Convert.ToDouble( db.Elections.Where(n => n.ElectionDate == f.Election.ElectionDate).Select(a => a.TotalVotes).FirstOrDefault())});

                        reportView.nationalReports = list.GroupBy(s =>  s.ElectionDate.ToString()).ToList();
                        
                        reportView.data = list.GroupBy(k => k.ElectionDate.ToString()).ToDictionary(k => k.Key, k => k.Sum(v => v.TotalVotes));
                        TempData["data"] = reportView.data;
                        TempData["record"] = list.ToList();
                        return View(reportView);
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
                TempData["message"] = "Your Session Has Expired Please Login Again!";
                return RedirectToAction("StaffLogin", "Staff");
            }
        }

        public ActionResult ExportPDFNational()
        {
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Report/NationalReport.rpt")));
            report.SetDataSource(GetNational());
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "NationalReport.pdf");
        }
        public ActionResult ExportPDFProvincial()
        {
            ReportDocument report = new ReportDocument();
            report.Load(Path.Combine(Server.MapPath("~/Report/ProvincialReport.rpt")));
            report.SetDataSource(GetProvicial());
            Response.Buffer = false;
            Response.ClearContent();
            Response.ClearHeaders();
            Stream stream = report.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
            stream.Seek(0, SeekOrigin.Begin);

            return File(stream, "application/pdf", "ProvincialReport.pdf");
        }

        
        public ActionResult Chart()
        {
            var date = TempData["data"];
            return View(TempData["data"]);
        }

        private National GetNational()
        {
            National n = new National();

            

            n.NationalResults.Rows.Clear();

            foreach (var item in (IEnumerable<NationalReports>)TempData["record"])
            {
                DataRow data = n.NationalResults.NewRow();
                data["ElectionDate"] = item.ElectionDate;
                data["PartyName"] = item.PartyName;
                data["TotalVotes"] = item.TotalVotes;
                data["VotePercentage"] = item.VotePercentage;
                n.NationalResults.Rows.Add(data);
            }
            TempData["data"] = TempData["data"];
            TempData["record"] = TempData["record"];

            return n;
        }

        private Provicial GetProvicial()
        {
            Provicial p = new Provicial();

            p.ProvincialResults.Rows.Clear();

            foreach(var item in (IEnumerable<ProvincialReports>)TempData["record"])
            {
                DataRow data = p.ProvincialResults.NewRow();
                data["ElectionDate"] = item.ElectionDate;
                data["PartyName"] = item.ProvinceName;
                data["ProvinceName"] = item.ProvinceName;
                data["CandidateName"] = item.CandidateName;
                data["TotalVotes"] = item.TotalVotes;
            }
            TempData["data"] = TempData["data"];
            TempData["record"] = TempData["record"];
            return p;
        }

    }
    
}