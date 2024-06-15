using ModelLibrary.Models;
using Newtonsoft.Json;

namespace RealDolmenInetum.Components.Pages.Manager
{
    public partial class DashboardGroupService
    {
        private TeamleadDetail teamleadDetail;
        private List<EmployeeItemWithBenchInfo> teamMembers;
        private List<string> benchInfoResults;
        private bool benchCheckCompleted;
        private const string BaseUrl = "https://localhost:7256/user/teamleads/";
        private const string BenchUrl = "https://localhost:7256/user/bench/search?email=";

        protected override async Task OnInitializedAsync()
        {
            await FetchTeamleadDetail("test@example.com");
            if (teamleadDetail != null)
            {
                await FetchTeamMembers(teamleadDetail.Email);
            }
        }

        private async Task FetchTeamleadDetail(string email)
        {
            try
            {
                var response = await HttpClient.GetAsync($"{BaseUrl}{email}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    teamleadDetail = JsonConvert.DeserializeObject<TeamleadDetail>(content);
                }
                else
                {
                    Console.WriteLine($"Failed to fetch team lead details: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching team lead details: {ex.Message}");
            }
        }

        private async Task FetchTeamMembers(string email)
        {
            try
            {
                var response = await HttpClient.GetAsync($"{BaseUrl}{email}/members");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<FindMembersOut>(content);
                    teamMembers = new List<EmployeeItemWithBenchInfo>();

                    foreach (var member in result.Members)
                    {
                        teamMembers.Add(new EmployeeItemWithBenchInfo
                        {
                            Email = member.Email,
                            Username = member.Username,
                            FirstName = member.FirstName,
                            LastName = member.LastName,
                            DirectoryGroupNames = member.DirectoryGroupNames ?? new List<string>(),
                            MemberCount = member.MemberCount,
                            LeadType = member.LeadType,
                            BenchInfo = null // Placeholder, will be updated later
                        });
                    }
                }
                else
                {
                    Console.WriteLine($"Failed to fetch team members: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching team members: {ex.Message}");
            }
        }

        private async Task CheckBenchStatus()
        {
            benchInfoResults = new List<string>();

            foreach (var member in teamMembers)
            {
                member.BenchInfo = await FetchBenchInfo(member.Email);

                if (member.BenchInfo != null)
                {
                    benchInfoResults.Add($"{member.Username} - Bench ID: {member.BenchInfo.BenchId}, Start Bench: {member.BenchInfo.StartBench}");
                }
                else
                {
                    benchInfoResults.Add($"{member.Username} - Niet op de bench");
                }
            }

            benchCheckCompleted = true;

            // Update UI
            StateHasChanged();
        }

        private async Task<BenchInfo> FetchBenchInfo(string email)
        {
            try
            {
                var response = await HttpClient.GetAsync($"{BenchUrl}{email}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<BenchInfo>>(content);
                    return results.FirstOrDefault();
                }
                else
                {
                    Console.WriteLine($"Failed to fetch bench info for {email}: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching bench info for {email}: {ex.Message}");
                return null;
            }
        }

        private class FindMembersOut
        {
            public List<EmployeeItem> Members { get; set; }
            public int TotalCount { get; set; }
        }

        private class EmployeeItemWithBenchInfo : EmployeeItem
        {
            public BenchInfo BenchInfo { get; set; }
        }

        private class BenchInfo
        {
            public int BenchId { get; set; }
            public DateTime StartBench { get; set; }
        }
    }
}