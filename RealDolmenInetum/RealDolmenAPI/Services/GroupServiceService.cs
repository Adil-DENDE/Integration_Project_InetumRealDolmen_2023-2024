using Moq;
using Newtonsoft.Json;
using ModelLibrary.Models;
using System.Text;
using ModelLibrary.ViewModels;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RealDolmenAPI.Services
{
    public interface IGroupServiceService
    {
        public Task<int> TeamleadDetail();
        public Task<TeamleadDetail> GetTeamleadByEmail(string email);
        public Task<bool> UpdateTeamleadByEmail(string email, TeamleadDetail teamlead);
        public Task<List<UserViewModel>> GetTeamMembersAsync(string managerEmail);
        Task<List<EmployeeItem>> GetTeamMembersByEmail(TeamMemberQueryParameters parameters);
        public IGroupServiceService UseMoq();
    }



    public class GroupServiceService : IGroupServiceService
    {
        private readonly HttpClient _httpClient;

        public GroupServiceService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7256/");
        }

        public async Task<int> TeamleadDetail()
        {
            // Maak http call
            var teamlead = await _httpClient.GetAsync("groupsservice.com/user/1");
            // Deze moet een int returnen maar later het object van Yaml content
            return 1;
        }

        public async Task<TeamleadDetail> GetTeamleadByEmail(string email)
        {
            var response = await _httpClient.GetAsync($"groupsservice.com/user/email/{email}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var teamleadDetail = JsonConvert.DeserializeObject<TeamleadDetail>(content);
                return teamleadDetail;
            }
            return null;
        }

        public async Task<bool> UpdateTeamleadByEmail(string email, TeamleadDetail teamlead)
        {
            var content = new StringContent(JsonConvert.SerializeObject(teamlead), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/user/email/{email}", content);
            return response.IsSuccessStatusCode;
        }

        public IGroupServiceService UseMoq()
        {
            var mock = new Mock<IGroupServiceService>();
            mock.Setup(m => m.TeamleadDetail()).ReturnsAsync(2);
            mock.Setup(m => m.GetTeamleadByEmail(It.Is<string>(s => s.Equals("test@example.com")))).ReturnsAsync(new TeamleadDetail
            {
                Email = "test@example.com",
                UserName = "testuser",
                FirstName = "Test",
                LastName = "User",
                DirectoryGroupNames = new List<string> { "Group1", "Group2" },
                IsManager = true
            });
            mock.Setup(m => m.UpdateTeamleadByEmail(It.IsAny<string>(), It.IsAny<TeamleadDetail>())).ReturnsAsync(true);

            // Mock voor GetTeamMembersByEmail
            // Ofwel allebij met equeals fo wel allebij zonder. Probeer met.
            mock.Setup(m => m.GetTeamMembersByEmail(It.IsAny<TeamMemberQueryParameters>()))
            .ReturnsAsync(new List<EmployeeItem>
            {
                new EmployeeItem
                {
                    Email = "member1@example.com",
                    Username = "member1",
                    FirstName = "Member",
                    LastName = "One",
                    DirectoryGroupNames = new List<string> { "Group1" },
                    MemberCount = 5,
                    LeadType = LeadType.TeamLead
                },
                new EmployeeItem
                {
                    Email = "marwan@student.be",
                    Username = "member2",
                    FirstName = "Member",
                    LastName = "Two",
                    DirectoryGroupNames= new List<string> { "Group2" },
                    MemberCount = 3,
                    LeadType = LeadType.ProjectManager
                },
                new EmployeeItem
                {
                    Email = "mdsfqn@student.be",
                    Username = "qsfdqdfqdfsdqf",
                    FirstName = "Member",
                    LastName = "Two",
                    DirectoryGroupNames= new List<string> { "Group4" },
                    MemberCount = 3,
                    LeadType = LeadType.ProjectManager
                }
            });
                return mock.Object;
        }

        public async Task<List<EmployeeItem>> GetTeamMembersByEmail(TeamMemberQueryParameters parameters)
        {
            var url = $"/user/teamleads/{parameters.Email}/members";

            var response = await _httpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<EmployeeItem>>(content);
            }
            return new List<EmployeeItem>();
        }


        public async Task<List<UserViewModel>> GetTeamMembersAsync(string managerEmail)
        {
            return new List<UserViewModel>
            {
                new UserViewModel
                {
                    UserId = 1,
                    NiveauId = 2,
                    Mail = "john.doe@example.com",
                    Username = "JohnDoe",
                    StartBench = DateTime.Now.AddDays(-30),
                    EndBench = null
                },
                new UserViewModel
                {
                    UserId = 2,
                    NiveauId = 3,
                    Mail = "jane.smith@example.com",
                    Username = "JaneSmith",
                    StartBench = DateTime.Now.AddDays(-15),
                    EndBench = null
                }
            };
        }
    }
}
