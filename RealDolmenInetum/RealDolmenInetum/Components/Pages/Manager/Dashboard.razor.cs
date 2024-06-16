using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using ModelLibrary.Dto;
using ModelLibrary.ViewModels;
using MudBlazor;
using Newtonsoft.Json;

namespace RealDolmenInetum.Components.Pages.Manager
{
    public partial class Dashboard
    {
        public List<UserViewModel> usersOpBench;
        public HttpClient httpClient = new HttpClient();
        public string ErrorMessage = "";
        private int daysOnBench;
        private string managerName = "Laden...";
        private int managerBenchId;
        private string currentUserId;
        private int currentManagerUserId;
        private string rol;

        protected override async Task OnInitializedAsync()
        {
            await FetchData();
            await FetchCurrentBenchManager();
            rol = auth.GetRolFromLoggedUser();
            currentUserId = auth.GetIdFromLoggedUser();
        }

        // Haal data op van alle benchers
        public async Task FetchData()
        {
            try
            {
                var token = auth.GetToken();

                if (!string.IsNullOrEmpty(token))
                {
                    token = token.Trim('"');
                    Http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                var response = await Http.GetAsync("https://localhost:7256/user/bench");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    usersOpBench = JsonConvert.DeserializeObject<List<UserViewModel>>(content);
                }
                else
                {
                    ErrorMessage = "Alleen Admin kan dit zien!";
                    usersOpBench = new List<UserViewModel>();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Er is een onverwachte fout opgetreden: {ex.Message}";
                usersOpBench = new List<UserViewModel>();
            }

        }
        // Ga naar de detail pagina

        public void NavigateUserPage(int id)
        {
            NavigationManager.NavigateTo("/user/" + id);
        }
        // Haal huidige benchmanager op
        private async Task FetchCurrentBenchManager()
        {
            try
            {
                var response = await Http.GetAsync("https://localhost:7256/user/bench/CurrentBenchManager");
                if (response.IsSuccessStatusCode)
                {
                    var manager = await response.Content.ReadFromJsonAsync<BenchManagerDto>();
                    managerName = $"{manager.FirstName} {manager.LastName}";
                    managerBenchId = manager.BenchId;
                    currentManagerUserId = manager.userId;
                }
                else
                {
                    managerName = "Er is momenteel geen Manager";
                }
            }
            catch (Exception ex)
            {
                managerName = "Fout bij het laden van gegevens";
                Console.WriteLine($"Error bij het ophalen van de huidige bench manager: {ex.Message}");
            }
        }

        private void OpenDialog()
        {
            var parameters = new DialogParameters
            {
                ["BenchId"] = managerBenchId,
                ["OnDialogClose"] = EventCallback.Factory.Create(this, FetchCurrentBenchManager)
            };
            DialogService.Show<ManagerUpdateDialog>("Bench Manager wijzigen", parameters);
        }
    }
}