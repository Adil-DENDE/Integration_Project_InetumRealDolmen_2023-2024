using Microsoft.AspNetCore.Components;
using ModelLibrary.Dto;
using ModelLibrary.Models;
using ModelLibrary.ViewModels;
using MudBlazor;
using Newtonsoft.Json;

namespace RealDolmenInetum.Components.Pages.Manager
{
    public partial class UserBenchDetails
    {
        [Parameter]
        public string Id { get; set; }
        [Parameter]
        public int BenchId { get; set; }

        public UserDetailsViewModel usersDetails;
        public BenchViewModel benchView;
        public HttpClient httpClient = new HttpClient();
        public int benchId;
        public string ErrorMessage = "";
        private int daysOnBench;
        bool isManager;
        private string rol;
        private string currentUserId;
        private int currentManagerUserId;



        protected override async Task OnInitializedAsync()
        {
            rol = auth.GetRolFromLoggedUser();
            currentUserId = auth.GetIdFromLoggedUser();
            await FetchUserData();
            await FetchCurrentBenchManager();
            await FetchBenchManagerDetails();
            await FetchUserBenchId();
        }
        // Informatie van een bepaalde user opzoeken
        public async Task FetchUserData()
        {
            try
            {
                var response = await Http.GetAsync("https://localhost:7256/user/" + @Id);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    usersDetails = JsonConvert.DeserializeObject<UserDetailsViewModel>(content);

                    // gebruik date helper om de aantal dagen since de startdate te berekenen
                    if (usersDetails != null)
                    {
                        daysOnBench = RealDolmenInetum.Helper.DateHelper.CalculateDaysSince(usersDetails.StartBench);
                    }

                    if (usersDetails.OccupationId.HasValue)
                    {
                        var occupationResponse = await Http.GetAsync($"https://localhost:7256/occupation/{usersDetails.OccupationId.Value}");

                        //ophalen van occupation met een andere endpoint
                        if (occupationResponse.IsSuccessStatusCode)
                        {
                            var occupationContent = await occupationResponse.Content.ReadAsStringAsync();
                            var occupation = JsonConvert.DeserializeObject<Occupation>(occupationContent);
                            usersDetails.OccupationDetails = occupation.Type;
                        }
                    }
                }
                else
                {
                    ErrorMessage = "Fout bij het ophalen van gegevens uit de database.";
                    usersDetails = new UserDetailsViewModel();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een onverwachte fout opgetreden: {ex.Message}";
                usersDetails = new UserDetailsViewModel();
            }
        }


        // DIALOG GEBRUIKEN OM DELETE TE BEVESTIGEN
        private async Task Einde()
        {
            var url = $"https://localhost:7256/user/bench/end/{benchId}";

            var requestBody = new UpdateEndBenchDto
            {
                EndBench = DateTime.UtcNow
            };

            var content = JsonContent.Create(requestBody);

            var response = await Http.PutAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Snackbar.Add("Gebruiker zit niet meer op de bench!", Severity.Success);
                NavigationManager.NavigateTo("/home");

            }
            else
            {
            }
        }

        // Zoek benchId  van een user
        private async Task FetchUserBenchId()
        {
            try
            {
                var response = await Http.GetAsync($"https://localhost:7256/user/bench/user-bench/{Id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var benchInfo = JsonConvert.DeserializeObject<BenchViewModel>(content);
                    benchId = benchInfo.BenchId;
                }
                else
                {
                    ErrorMessage = "Fout bij het ophalen van bench ID uit de database.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een onverwachte fout opgetreden bij het ophalen van bench ID: {ex.Message}";
            }
        }

        // Haal de details op van de benchmanager
        public async Task FetchBenchManagerDetails()
        {
            if (usersDetails.IsCurrentBenchManager.HasValue)
            {
                var managerResponse = await Http.GetAsync($"https://localhost:7256/user/userInfo/{usersDetails.IsCurrentBenchManager.Value}");

                if (managerResponse.IsSuccessStatusCode)
                {
                    var managerContent = await managerResponse.Content.ReadAsStringAsync();
                    var managerInfo = JsonConvert.DeserializeObject<BenchManagerViewModel>(managerContent);
                    usersDetails.BenchManagerFirstName = managerInfo.First_Name;
                    usersDetails.BenchManagerLastName = managerInfo.Last_Name;

                    //StateHasChanged(); // Forceer de UI om te updaten
                }
                else
                {
                    Snackbar.Add("Fout bij het ophalen van Bench Manager gegevens.", Severity.Error);
                }
            }
        }

        // Verander de BenchManager status
        private async Task UpdateBenchManagerStatus()
        {
            if (usersDetails.IsCurrentBenchManager.HasValue)
            {
                var response = await Http.PutAsync($"https://localhost:7256/user/bench/updateManager/{benchId}/{usersDetails.IsCurrentBenchManager}", null);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Manager status succesvol bijgewerkt!");
                }
                else
                {
                    Console.WriteLine("Fout bij het bijwerken van de manager status.");
                }
            }
            else
            {
                Console.WriteLine("Manager status kan niet worden bijgewerkt omdat de waarde onbepaald is.");
            }
        }

        private async Task ToggleManagerStatus()
        {
            isManager = !isManager;

            if (usersDetails.IsCurrentBenchManager.HasValue)
            {
                usersDetails.IsCurrentBenchManager = !usersDetails.IsCurrentBenchManager.Value;
            }
            else
            {
                usersDetails.IsCurrentBenchManager = true;
            }
            await UpdateBenchManagerStatus();
        }


        // Deze mag weg, heet te maken met currentbench manager update
        private void OpenDialog()
        {
            DialogOptions options = new DialogOptions() { CloseOnEscapeKey = true };

            // Maak een nieuwe instance van DialogParameters
            var parameters = new DialogParameters();

            // Voeg de volledige naam en benchId toe als parameters
            string fullName = $"{usersDetails.BenchManagerFirstName} {usersDetails.BenchManagerLastName}";
            parameters.Add("Name", fullName);
            parameters.Add("BenchId", benchId);
            if (usersDetails.IsCurrentBenchManager.HasValue)
            {
                parameters.Add("CurrentBenchManagerId", usersDetails.IsCurrentBenchManager.Value);
            }
            else
            {
                parameters.Add("CurrentBenchManagerId", null);
            }
            parameters.Add("Id", Id);

            DialogService.Show<ManagerUpdateDialog>("Manager bijwerken", parameters, options);
        }

        // Zoek de huidige benchmanager op
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

    }
}