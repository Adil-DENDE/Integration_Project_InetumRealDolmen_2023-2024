using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMockLibrary
{
    public interface IDataService
    {
        TeamLeadDetail GetTeamLeadByEmail(string email);
    }
}
