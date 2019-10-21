using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INF272Group11Project.ViewModels;
using INF272Group11Project.Models;

namespace INF272Group11Project.ViewModels
{
    public class ReportViewModel
    {

        public StaffGUIDControl StaffView;

        public Party Party { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Candidate candidate { get; set; }
        public NationalResult NationalResult { get; set; }
        public ProvincialResult ProvincialResult { get; set; }

        public List<IGrouping<string, NationalReport>> nationalReports { get; set; }
        public List<IGrouping<string,ProvincialReport>> provincialReports { get; set; }

        public Dictionary<string, int> data { get; set; }

        public class NationalReport
        {
            public DateTime ElectionDate { get; set; }
            public string PartyName { get; set; }
            public int TotalVotes { get; set; }

            public double VotePercentage { get; set; }


        }
        public class ProvincialReport
        {
            public DateTime ElectionDate { get; set; }
            public string ProvinceName { get; set; }
            public string PartyName { get; set; }
            public string CandidateName { get; set; }
            public int TotalVotes { get; set; }
            public double VotePercentage { get; set; }
        }
    }
}