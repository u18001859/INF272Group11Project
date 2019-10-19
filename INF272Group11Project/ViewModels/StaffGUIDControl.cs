using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INF272Group11Project.Models;

namespace INF272Group11Project.ViewModels
{
    public class StaffGUIDControl
    {
        public Staff staff;


        public void RefreshGUID(VotingSystemProjectEntities1 db)

        {
            db.Configuration.ProxyCreationEnabled = false;

            staff.GUID = Guid.NewGuid().ToString();
            staff.GUIDTimeStamp = DateTime.Now.AddMinutes(30);
            var g = db.Staffs.Where(x => x.GUID == staff.GUID).Count();
            if (g > 0)
            {
                RefreshGUID(db);
            }
            else
            {

                var u = db.Staffs.Where(z => z.StaffID == staff.StaffID).FirstOrDefault();

                db.Entry(u).CurrentValues.SetValues(staff);
                db.SaveChanges();
            }
        }


        public bool IsLogedIn(VotingSystemProjectEntities2 db)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var guid = db.Staffs.Where(x => x.GUID == staff.GUID && x.GUIDTimeStamp > DateTime.Now).Count();

       
            if (guid > 0)
                return true;
            return false;

        }

        public bool IsLogedIn(VotingSystemProjectEntities2 db, string userGUID)
        {
            db.Configuration.ProxyCreationEnabled = false;
            staff = db.Staffs.Where(x => x.GUID == userGUID && x.GUIDTimeStamp > DateTime.Now).FirstOrDefault();

            if (staff != null)
                return true;
            return false;

        }
    }

}


