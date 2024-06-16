using Microsoft.AspNetCore.Components;
using ModelLibrary.Dto;
using MudBlazor;
using Newtonsoft.Json;

namespace RealDolmenInetum.Components.Pages.Manager
{
    public partial class ManagerUpdateDialog
    {
        private string searchTerm;
        private List<BencherDto> suggestedUsers = new List<BencherDto>();
        private bool searchPerformed = false;
        private int? selectedBenchId;
        [Parameter] public int? BenchId { get; set; }
        [Parameter] public EventCallback OnDialogClose { get; set; }

        [CascadingParameter] MudDialogInstance DialogService { get; set; }

        // Voer de zoekopdracht uit
        private async Task PerformSearch()
        {
            searchPerformed = true;
            suggestedUsers = await SearchUsersByEmail(searchTerm);
            StateHasChanged();
        }

        // Het opzoeke van een user met email
        private async Task<List<BencherDto>> SearchUsersByEmail(string email)
        {
            try
            {
                var response = await Http.GetAsync($"https://localhost:7256/user/bench/search?email={email}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var users = JsonConvert.DeserializeObject<List<BencherDto>>(content);
                    return users ?? new List<BencherDto>();
                }
                else
                {
                    Console.WriteLine($"Server error during search: {await response.Content.ReadAsStringAsync()}");
                    return new List<BencherDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in search: {ex.Message}");
                return new List<BencherDto>();
            }
        }

        
        private void SelectUser(BencherDto user)
        {
            selectedBenchId = user.BenchId;
            Console.WriteLine($"Selected User: {user.Username}");
            searchTerm = $"{user.Username} ({user.Mail})";
            suggestedUsers.Clear();
            StateHasChanged();
        }

        void Cancel() => DialogService.Close(DialogResult.Cancel());

        void Submit()
        {
            if (selectedBenchId.HasValue)
            {
                if (BenchId == 0)
                {
                    UpdateNewManagerStatus(selectedBenchId.Value, true).ContinueWith(async task =>
                    {
                        if (task.Result)
                        {
                            Snackbar.Add("New bench manager set successfully.", Severity.Success);
                            await OnDialogClose.InvokeAsync();
                            DialogService.Close(DialogResult.Ok(true));
                        }
                        else
                        {
                            Snackbar.Add("Error setting new bench manager.", Severity.Error);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                else
                {
                    UpdateBenchManagers(BenchId.Value, selectedBenchId.Value).ContinueWith(async task =>
                    {
                        if (task.Result)
                        {
                            Snackbar.Add("Bench managers updated successfully.", Severity.Success);
                            await OnDialogClose.InvokeAsync();
                            DialogService.Close(DialogResult.Ok(true));
                        }
                        else
                        {
                            Snackbar.Add("Error updating bench managers.", Severity.Error);
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            else
            {
                Snackbar.Add("No bencher selected.", Severity.Warning);
            }
        }

        private async Task<bool> UpdateNewManagerStatus(int benchId, bool newStatus)
        {
            var url = $"https://localhost:7256/user/bench/updateManager/{benchId}/{newStatus}";
            var response = await Http.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }

        private async Task<bool> UpdateBenchManagers(int oldManagerId, int newManagerId)
        {
            var url = $"https://localhost:7256/user/bench/updateManagers?oldManagerId={oldManagerId}&newManagerId={newManagerId}";
            var response = await Http.PutAsync(url, null);
            return response.IsSuccessStatusCode;
        }
    }
}