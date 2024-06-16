using Microsoft.AspNetCore.Components;
using ModelLibrary.Dto;
using ModelLibrary.Models;
using MudBlazor;
using Newtonsoft.Json;

namespace RealDolmenInetum.Components.Pages.Bencher
{
    public partial class ActiviteitenGeschiedenis
    {
        [Parameter]
        public int? BenchId { get; set; }
        public int? BenchIdCurrent { get; set;}
        [Parameter]
        public string searchedUserMail { get; set; }
        private List<OccupationHistoryDto> occupationHistories = new List<OccupationHistoryDto>();
        private List<Occupation> occupations = new List<Occupation>();
        public bool loadSuccessful { get; set; }
        public string ErrorMessage;
        private int currentManagerUserId;
        private string rol;
        private string currentUserId;

        protected override async Task OnInitializedAsync()
        {
            await getUserBenchId();
            rol = auth.GetRolFromLoggedUser();
            currentUserId = auth.GetIdFromLoggedUser();
            await FetchCurrentBenchManager();
            await LoadData();
            occupations = await Http.GetFromJsonAsync<List<Occupation>>("https://localhost:7256/occupation");
        }

        private async Task<string> getUserBenchId()
        {
            try
            {
                var userId = auth.GetIdFromLoggedUser();
                var response = await Http.GetAsync("https://localhost:7256/user/" + userId);
                if (response.IsSuccessStatusCode)
                {
                    var userJson = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<dynamic>(userJson);
                    BenchId = user.benchId;
                    BenchIdCurrent = user.benchId;
                    return user.benchId;
                }
                else
                {
                    return $"Error: Status code {response.StatusCode} received.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een onverwachte fout opgetreden: {ex.Message}";
                return $"Error: '{ex.Message}'";
            }
        }

        private async Task LoadData()
        {
            loadSuccessful = true;
            occupationHistories.Clear();

            if (BenchId > 0)
            {
                try
                {
                    var fetchedData = await Http.GetFromJsonAsync<List<OccupationHistoryDto>>($"https://localhost:7256/occupationHistory/{BenchId}");

                    if (fetchedData != null && fetchedData.Any())
                    {
                        occupationHistories = fetchedData
                            .OrderBy(o => o.EndDate.HasValue)
                            .ThenByDescending(o => o.EndDate)
                            .ToList();

                        foreach (var history in occupationHistories)
                        {
                            var occupationResponse = await Http.GetFromJsonAsync<Occupation>($"https://localhost:7256/occupation/{history.OccupationId}");
                            if (occupationResponse != null)
                            {
                                history.OccupationType = occupationResponse.Type;
                            }
                        }
                    }
                    else
                    {
                        loadSuccessful = false;
                    }
                }
                catch (Exception)
                {
                    loadSuccessful = false;
                }
            }
            else
            {
                loadSuccessful = false;
            }
        }

        // Open een dialoog om de geselecteerde record te wijzigen
        private async Task EditRecord(OccupationHistoryDto record)
        {
            var parameters = new DialogParameters
            {
                ["OccupationHistory"] = record,
                ["Occupations"] = occupations,
                ["BenchId"] = BenchId
            };

            var dialog = DialogService.Show<EditOccupationHistoryDialog>("Bewerk Record", parameters);
            var result = await dialog.Result;

            if (!result.Cancelled)
            {
                await LoadData();
            }
        }

        // Zoek huidige benchmanager
        private async Task FetchCurrentBenchManager()
        {
            try
            {
                var response = await Http.GetAsync("https://localhost:7256/user/bench/CurrentBenchManager");
                if (response.IsSuccessStatusCode)
                {
                    var manager = await response.Content.ReadFromJsonAsync<BenchManagerDto>();
                    currentManagerUserId = manager.userId;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error bij het ophalen van de huidige bench manager: {ex.Message}");
            }
        }

        private async Task getHistoryEmail()
        {
            // Controleer of het e-mailadres is ingevuld
            if (string.IsNullOrWhiteSpace(searchedUserMail))
            {
                ErrorMessage = "Vul een e-mailadres in om te zoeken.";
                return;
            }

           
            ErrorMessage = string.Empty;

            try
            {
                var response = await Http.GetAsync($"https://localhost:7256/user/bench/search?email={searchedUserMail}");
                if (response.IsSuccessStatusCode)
                {
                    var resultsJson = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<List<BencherDto>>(resultsJson);

                    if (results != null && results.Any())
                    {
                        var user = results.First();
                        BenchId = user.BenchId;
                        await LoadData();
                    }
                    else
                    {
                        ErrorMessage = "Geen resultaten gevonden voor het opgegeven e-mailadres.";
                    }
                }
                else
                {
                    ErrorMessage = $"Error: Status code {response.StatusCode} received.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een onverwachte fout opgetreden: {ex.Message}";
            }
        }


        private async Task CurrentUserBenchHistory()
        {
            loadSuccessful = true;
            occupationHistories.Clear();

            if (BenchId > 0)
            {
                try
                {
                    var fetchedData = await Http.GetFromJsonAsync<List<OccupationHistoryDto>>($"https://localhost:7256/occupationHistory/{BenchIdCurrent}");

                    if (fetchedData != null && fetchedData.Any())
                    {
                        occupationHistories = fetchedData
                            .OrderBy(o => o.EndDate.HasValue)
                            .ThenByDescending(o => o.EndDate)
                            .ToList();

                        foreach (var history in occupationHistories)
                        {
                            var occupationResponse = await Http.GetFromJsonAsync<Occupation>($"https://localhost:7256/occupation/{history.OccupationId}");
                            if (occupationResponse != null)
                            {
                                history.OccupationType = occupationResponse.Type;
                            }
                        }
                    }
                    else
                    {
                        loadSuccessful = false;
                    }
                }
                catch (Exception)
                {
                    loadSuccessful = false;
                }
            }
            else
            {
                loadSuccessful = false;
            }
        }

    }
}