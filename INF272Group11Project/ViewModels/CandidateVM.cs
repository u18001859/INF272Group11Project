using INF272Group11Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INF272Group11Project.ViewModels
{
    public class CandidateVM
    {
        //For drop down list for parties
        public IEnumerable<SelectListItem> Parties { get; set; }

        //For drop down list for candidate positions
        public IEnumerable<SelectListItem> Positions { get; set; }

        //Accessors and mutators
        public int Candidate_ID { set; get; }
        public string CandidateFirstNames { set; get; }
        public string CandidateLastName { set; get; }
        public int PartyID { get; set; }
        public int CandidatePosition_ID { get; set; }

    }
}