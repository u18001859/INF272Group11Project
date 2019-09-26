using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INF272Group11Project.Models;

namespace INF272Group11Project.ViewModels
{
    public class RegisterVoterVM
    {
        public List<Province> Provinces;
        public List<CityOrTown> CityOrTowns;
        public List<Suburb> suburbs;
        public List<SecurityQuestion> SecurityQuestions;
    }
}