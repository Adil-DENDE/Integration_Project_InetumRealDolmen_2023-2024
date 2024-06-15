using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace RealDolmenInetum.Components.Pages.Bencher
{
    public partial class ActiviteitEindigen
    {
        private int benchId;
        private string currentOccupationType;

        protected override async Task OnInitializedAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("benchId", out var benchIdValue)) // Parse a query string into its component key and value parts. https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.webutilities.queryhelpers?view=aspnetcore-8.0
            {
                benchId = Convert.ToInt32(benchIdValue);
            }

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("occupationType", out var occupationType))
            {
                currentOccupationType = Uri.UnescapeDataString(occupationType);
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
                NavigationManager.NavigateTo("/activiteit", true);
            }
            else
            {
                Console.Error.WriteLine("Fout bij het beëindigen van alle actieve occupation histories.");
            }
        }
    }
}