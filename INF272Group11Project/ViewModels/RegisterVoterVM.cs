using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INF272Group11Project.Models;
using System.Security.Cryptography;
using System.Text;

namespace INF272Group11Project.ViewModels
{
    public class RegisterVoterVM
    {
        public List<Province> Provinces;
        public List<CityOrTown> CityOrTowns;
        public List<Suburb> suburbs;
        public List<SecurityQuestion> SecurityQuestions;
        public int? id;
        public List<Voter> VoterList;
        public VoterVM voterView;

        public string HashedData(string placeholder)
        {
            using (SHA256 h = SHA256.Create())
            {
                byte[] b = h.ComputeHash(Encoding.UTF8.GetBytes(placeholder));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < b.Length; i++)
                {
                    builder.Append(b[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
