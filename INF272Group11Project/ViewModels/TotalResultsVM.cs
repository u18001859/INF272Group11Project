using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using INF272Group11Project.Models;

namespace INF272Group11Project.ViewModels
{
    public class TotalResultsVM
    {
        public List<Party> Parties;
        public List<Candidate> Candidates;
        public List<NationalResult> Results;
        public List<Election> ListElection;
        public NationalResultsVM resultsView;
        public Election election;

        public StaffGUIDControl StaffView;

    }
}