using Microsoft.AspNetCore.Components;
using ModelLibrary.Dto;
using ModelLibrary.Models;
using MudBlazor;

namespace RealDolmenInetum.Components.Pages.Bencher
{
    public partial class EditOccupationHistoryDialog
    {
        [CascadingParameter] public MudDialogInstance MudDialog { get; set; }
        [Parameter] public OccupationHistoryDto OccupationHistory { get; set; }
        [Parameter] public List<Occupation> Occupations { get; set; }
        [Parameter] public int BenchId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Occupations = await FetchOccupationsAsync();
        }

        // Geeft een lijst van occupations (type)
        private async Task<List<Occupation>> FetchOccupationsAsync()
        {
            var response = await Http.GetFromJsonAsync<List<Occupation>>("https://localhost:7256/occupation");
            return response ?? new List<Occupation>();
        }

        // Update een occupationHistory record.
        private async Task Save()
        {
            // Stuur eerst de update naar de OccupationHistory
            var response = await Http.PutAsJsonAsync($"https://localhost:7256/occupationHistory/update/{OccupationHistory.Id}", OccupationHistory);
            if (response.IsSuccessStatusCode && OccupationHistory.EndDate.HasValue)
            {
                // Als de EndDate is ingesteld, stuur dan een request om de occupation_id van de bench op NULL te zetten
                await Http.PutAsync($"https://localhost:7256/user/bench/clearOccupation/{BenchId}", null);
                MudDialog.Close(DialogResult.Ok(true));
            }
            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("Record succesvol bijgewerkt.", Severity.Success);
            }
            else
            {
                Snackbar.Add("Er is een fout opgetreden bij het bijwerken van de record.", Severity.Error);
            }
        }

    }
}