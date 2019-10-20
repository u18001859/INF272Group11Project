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
        //For drop down list for candidate positions
        public IEnumerable<SelectListItem> Positions { get; set; }
        public int SelectedCandidatePosition_ID { get; set; }

        //For drop down list for parties
        public IEnumerable<SelectListItem> Parties { get; set; }
        public int SelectedPartyID { get; set; }

        //For drop down list for provinces
        public IEnumerable<SelectListItem> Provinces { get; set; }
        public int SelectedProvinceID { get; set; }

        //For drop down list for candidates themselves
        public IEnumerable<SelectListItem> Candidates { get; set; }
        public int SelectedCandidateID { get; set; }

        //Accessors and mutators
        public int Candidate_ID { set; get; }
        public string CandidateFirstNames { set; get; }
        public string CandidateLastName { set; get; }
        public int CandidatePosition_ID { set; get; }
        public int PartyID { set; get; }
        public int ProvinceID { set; get; }

        public StaffGUIDControl StaffView;

        public Candidate candidate;

    }
}
