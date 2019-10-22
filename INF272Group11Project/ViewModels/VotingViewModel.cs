using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INF272Group11Project.Models;

namespace INF272Group11Project.ViewModels
{
    public class VotingViewModel
    {
        VotingSystemProjectEntities3 db = new VotingSystemProjectEntities3();
       public Party party;
        
        public PartyImage PartyImage;

        public List<PartyImage> partiesImages;

        public List<Candidate> listcandidate;

        public VoterVM voterView;
       
        public Election GetElectionDate()
        {
            return db.Elections.Where(x => x.ElectionDate == DateTime.Today).FirstOrDefault();
        }



    }
}