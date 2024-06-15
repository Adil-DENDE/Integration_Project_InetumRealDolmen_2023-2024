using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace RealDolmenInetum.Components.Pages.Bencher
{
    public partial class ActiviteitStoppen
    {
        private int benchId;
        private string currentOccupationType;
        private string ErrorMessage;
        private bool userSuccesToegevoegd;

        protected override async Task OnInitializedAsync()
        {
            await getUserBenchId();
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
                    benchId = user.benchId;
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


        // Eindig de activiteit. In bench wordt occupation NULL en in history wordt end date ingesteld.
        private async Task EndActivity()
        {
            var clearOccupationUrl = $"https://localhost:7256/user/bench/clearOccupation/{benchId}";
            await Http.PutAsync(clearOccupationUrl, null);

            // Datum in URL anders werkte het niet
            var endDate = DateTime.UtcNow.ToString("o");
            var updateEndDateUrl = $"https://localhost:7256/occupationHistory/endAll/{benchId}?endDate={Uri.EscapeDataString(endDate)}";

            var response = await Http.PutAsync(updateEndDateUrl, null);
            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("De activiteit is succesvol beëindigd.", Severity.Success);
                NavigationManager.NavigateTo("/activiteiten");

            }
            else
            {
                Console.Error.WriteLine("Fout bij het beëindigen van alle actieve occupation histories.");
                Snackbar.Add("Probleem opgetreden!", Severity.Error);
            }
        }
    }
}