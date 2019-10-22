using INF272Group11Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INF272Group11Project.ViewModels
{
    public class CandidatePositionVM
    {
        //For drop down list for candidate positions
        public IEnumerable<SelectListItem> Positions { get; set; }
        public int SelectedCandidatePosition_ID { get; set; }

        public int CandidatePosition_ID { set; get; }
        public string CandidatePosition_Description { set; get; }
    }
}