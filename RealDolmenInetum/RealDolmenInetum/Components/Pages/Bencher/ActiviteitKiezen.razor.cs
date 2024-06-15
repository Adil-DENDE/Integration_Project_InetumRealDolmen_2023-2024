using Microsoft.AspNetCore.Components;
using ModelLibrary.Dto;
using ModelLibrary.Models;
using Newtonsoft.Json;

namespace RealDolmenInetum.Components.Pages.Bencher
{
    public partial class ActiviteitKiezen
    {
        public List<Occupation> occupations = new List<Occupation>();
        public string ErrorMessage = "";
        private string selectedOccupationType;
        private int userBenchId;

        protected override async Task OnInitializedAsync()
        {
            await getUserBenchId();
            await FetchOccupations();
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
                    var user = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(userJson);
                    userBenchId = user.benchId;
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


        // Data ophalen uit de api en daar een lijst mee vullen maken
        public async Task FetchOccupations()
        {
            try
            {
                var response = await Http.GetAsync("https://localhost:7256/occupation");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    occupations = JsonConvert.DeserializeObject<List<Occupation>>(content);
                }
                else
                {
                    ErrorMessage = "Kon de activiteiten niet ophalen uit de database.";
                    occupations = new List<Occupation>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Een fout is opgetreden: {ex.Message}");
                ErrorMessage = "Er is een probleem opgetreden bij  het ophalen van de activiteiten. Probeer het later opnieuw.";
                occupations = new List<Occupation>();
            }
        }

        // Methode om de occupation te updaten
        private async Task UpdateOccupation()
        {
            var selectedOccupation = GetSelectedOccupation();
            if (selectedOccupation != null)
            {
                if (await UpdateOccupationInBench(selectedOccupation))
                {
                    await AddOccupationHistory(selectedOccupation);
                }
            }
            else
            {
                ErrorMessage = "Geselecteerde occupation niet gevonden.";
            }
        }

        // Methode om de geselecteerde occupation te vinden op basis van het type.
        private Occupation GetSelectedOccupation()
        {
            return occupations.FirstOrDefault(o => o.Type == selectedOccupationType);
        }


        // Methode om de occupation_id van een Bench record te updaten.
        private async Task<bool> UpdateOccupationInBench(Occupation selectedOccupation)
        {
            var updateResponse = await Http.PutAsJsonAsync($"https://localhost:7256/user/bench/occupation/{userBenchId}", new { type = selectedOccupationType });
            if (!updateResponse.IsSuccessStatusCode)
            {
                ErrorMessage = "Fout bij het updaten van de occupation.";
                return false;
            }
            return true;
        }

        // Methode om een nieuwe occupation history record toe te voegen.
        private async Task AddOccupationHistory(Occupation selectedOccupation)
        {
            var historyDto = new OccupationHistoryDto
            {
                BenchId = userBenchId,
                OccupationId = selectedOccupation.Id,
                StartDate = DateTime.Now,
                EndDate = null
            };

            var addHistoryResponse = await Http.PostAsJsonAsync("https://localhost:7256/occupationHistory/add", historyDto);
            if (addHistoryResponse.IsSuccessStatusCode)
            {
                NavigationManager.NavigateTo($"/activiteiten");
            }
            else
            {
                ErrorMessage = "Fout bij het toevoegen van occupation history.";
            }
        }

    }
}