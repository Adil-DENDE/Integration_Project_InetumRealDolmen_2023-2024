using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMockLibrary
{
    public class MockDataService : IDataService
    {
        private readonly Dictionary<string, TeamLeadDetail> _teamLeads;

        public MockDataService()
        {
            _teamLeads = new Dictionary<string, TeamLeadDetail>
            {
                { "john.doe@example.com", new TeamLeadDetail { Email = "john.doe@example.com", Name = "John Doe" } },
                { "sarah.connor@example.com", new TeamLeadDetail { Email = "sarah.connor@example.com", Name = "Sarah Connor" } },
                { "james.bond@example.com", new TeamLeadDetail { Email = "james.bond@example.com", Name = "James Bond" } },
                { "tony.stark@example.com", new TeamLeadDetail { Email = "tony.stark@example.com", Name = "Tony Stark" } }
            };
        }

        public TeamLeadDetail GetTeamLeadByEmail(string email)
        {
            _teamLeads.TryGetValue(email, out var teamLead);
            return teamLead;
        }
    }
}
