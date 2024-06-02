using ModelLibrary.Models;
using ModelLibrary.ViewModels;
using Newtonsoft.Json;
using System.Net.Http;

namespace RealDolmenAPI.Services
{
    public class MockTeamService
    {

        private readonly HttpClient _httpClient;

        public MockTeamService(HttpClient httpClient)
        {
            _httpClient = httpClient;
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

        public async Task<TeamleadDetail> GetTeamleadByEmail(string email)
        {

            var response = await _httpClient.GetAsync($"groupsservice/user/email/{email}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var teamleadDetail = JsonConvert.DeserializeObject<TeamleadDetail>(content);
                return teamleadDetail;
            }
            return null;
        }
    }
}
