using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Text;

namespace INF272Group11Project.ViewModels
{
    public class StaffEncryption
    {

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