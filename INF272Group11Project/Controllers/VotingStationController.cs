using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using INF272Group11Project.Models;
using INF272Group11Project.ViewModels;

namespace INF272Group11Project.Controllers
{
    public class VotingStationController : Controller
    {
        VotingSystemProjectEntities3 Resultsdb = new VotingSystemProjectEntities3();
        AddVotingStationVM addStation = new AddVotingStationVM();
        // GET: VotingStation
        [HttpPost]
        public ActionResult AddVotingStation()
        {
            
            
            return View();
        }

        public ActionResult UpdateDeleteVotingStation()
        {
            //same with this one
            return View();
        }

        public ActionResult UpdateVotingStation()
        {
            //same with this one
            return View();
        }

        
    }
}