using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INF272Group11Project.Controllers
{
    public class CandidateController : Controller
    {
        // GET: Candidate

       public ActionResult RegisterCandidate()
        {
            
            return View();
        }

        public ActionResult AddNewCandidate()
        {
                    return RedirectToAction("StaffHomePage", "Staff");  
        }


        public ActionResult UpdateOrDeleteCandidate()
        {
            return View();
        }
 
 
        public ActionResult Choice(string submitbutton)
        {
            if (submitbutton == "Update")
            {
                return RedirectToAction("UpdateCandidate");
            }
            else if (submitbutton == "Delete")
            {
                return RedirectToAction("Delete");
            }
            else
            {
                return RedirectToAction("StaffHomePage","Staff");
            }
            
        }

        public ActionResult UpdateCandidate()
        {
            //viewbags to fill fields with already existing data
            
            return View();
        }

        public ActionResult Update()
        {
             return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
        }

        public ActionResult Delete()
        {
            return RedirectToAction("UpdateOrDeleteCandidate", "Candidate");
        }
        
        public ActionResult TotalVotes()
        {
            return View();
        }
        
    }
}
