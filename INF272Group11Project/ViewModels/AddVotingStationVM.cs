using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INF272Group11Project.Models;

namespace INF272Group11Project.ViewModels
{
    public class AddVotingStationVM
    {
        public int votingStationID { get; set; }
        public string votingStationName { get; set; }
        public string votingStationLattitude { get; set; }
        public string votingStationLongitude { get; set; }
        public string votingStationOpenTime { get; set; }
        public string votingStationCloseTime { get; set; }
        public string votingStationStreetAddress { get; set; }
        public int votingStationSuburbID { get; set; }

    }
}