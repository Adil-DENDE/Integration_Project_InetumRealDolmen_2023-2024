using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.Models
{
    public class TeamMemberQueryParameters
    {
        public string Email { get; set; }
        public string FirstNamePattern { get; set; }
        public string LastNamePattern { get; set; }
        public string EmailPattern { get; set; }
        public bool MatchAllCriteria { get; set; }
        public int PageIndex { get; set; }
    }

}
