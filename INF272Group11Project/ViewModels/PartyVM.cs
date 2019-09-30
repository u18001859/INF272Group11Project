using INF272Group11Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace INF272Group11Project.ViewModels
{
    public class PartyVM
    {
        public IEnumerable<SelectListItem> Parties { get; set; }
        public int PartyID { get; set; }
    }
}