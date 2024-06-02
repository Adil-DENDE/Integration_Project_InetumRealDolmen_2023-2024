using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLibrary.Models
{
    public class EmployeeItem
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> DirectoryGroupNames { get; set; }
        public int MemberCount { get; set; }
        public LeadType LeadType { get; set; }
    }

    public class FindMembersOut
    {
        public List<EmployeeItem> Members { get; set; }
        public int TotalCount { get; set; }
    }

    public enum LeadType
    {
        TeamLead,
        ProjectManager
    }
}
