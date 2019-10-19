using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INF272Group11Project.Models;

namespace INF272Group11Project.ViewModels
{
    public class VoterVM
    {
        public Voter voter;

        public void RefreshGUID(VotingSystemProjectEntities2 db)
        {
            db.Configuration.ProxyCreationEnabled = false;

            voter.GUID = Guid.NewGuid().ToString();
            voter.GUIDTimeStamp = DateTime.Now.AddMinutes(30);
            var g = db.Voters.Where(x => x.GUID == voter.GUID).Count();
            if (g > 0)
            {
                RefreshGUID(db);
            }
            else
            {
                var u = db.Voters.Where(z => z.VoterID == voter.VoterID).FirstOrDefault();
                db.Entry(u).CurrentValues.SetValues(voter);
                db.SaveChanges();
            }
        }

        public bool IsLogedIn(VotingSystemProjectEntities2 db)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var guid = db.Voters.Where(x => x.GUID == voter.GUID && x.GUIDTimeStamp > DateTime.Now).Count();
            if (guid > 0)
                return true;
            return false;

        }
        public bool IsLogedIn(VotingSystemProjectEntities2 db, string userGUID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            voter = db.Voters.Where(x => x.GUID == userGUID && x.GUIDTimeStamp > DateTime.Now).FirstOrDefault();
            if (voter != null)
                return true;
            return false;

        }


    }
}