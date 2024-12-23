using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ModelLibrary.Dto;
using MudBlazor;
using Newtonsoft.Json;
using System.Text;

namespace RealDolmenInetum.Components.Pages.Manager
{
    public partial class Toevoegen
    {
        [SupplyParameterFromForm]
        public UserBenchDto formModel { get; set; }
        private string searchTerm = string.Empty;
        private List<UserDto> suggestedUsers = new List<UserDto>();
        private UserDto? selectedUser = null;
        private bool searchPerformed = false;
        public string ErrorMessage = "";
        public bool userSuccesToegevoegd = false;
        private string currentUserId;
        private int currentManagerUserId;
        private string rol;

        // NOUVELLE INSTANCE DU FORM
        protected override void OnInitialized() => formModel ??= new UserBenchDto() { StartBench = DateTime.Today };

        protected override async Task OnInitializedAsync()
        {
            //await FetchCurrentBenchManager();
            rol = auth.GetRolFromLoggedUser();
            currentUserId = auth.GetIdFromLoggedUser();
        }

        private async Task Submit(EditContext editContext)
        {
            if (!IsValidEmail(formModel.Email)) // e-mail validatie
            {
                ErrorMessage = "Ongeldig e-mailadres.";
                return;
            }

            var validUser = await SearchUsersByEmail(formModel.Email);
            if (!validUser.Any())
            {
                ErrorMessage = "Er bestaat geen werknemer met dit e-mail adres.";
                return;
            }

            try
            {
                Console.WriteLine("Email=" + formModel.Email + ", DateTime=" + formModel.StartBench.ToString());
                var stringData = JsonConvert.SerializeObject(formModel);
                var stringContent = new StringContent(stringData, Encoding.UTF8, "application/json");
                Console.WriteLine(stringData);

                var httpResponseMessage = await Http.PostAsync("https://localhost:7256/user/bench/add", stringContent);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    NavigationManager.NavigateTo("/home");
                    userSuccesToegevoegd = true;
                    snackBarShow();
                }
                else
                {
                    var errorMessage = await httpResponseMessage.Content.ReadAsStringAsync();
                    if (errorMessage.Contains("zit al op de bank en heeft een lopende sessie"))
                    {
                        Snackbar.Add("De gebruiker zit al op de bench.", Severity.Error);
                    }
                    else
                    {
                        errorMessage = "Er bestaat geen werknemer met dit e-mail adres.";
                        Console.WriteLine($"Serverfout: {errorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Er is een error opgetreden. Probeer het later opnieuw.";
                Console.WriteLine($"Uitzondering: {ex.Message}");
            }
        }



        // Methode die wordt aangeroepen bij elke wijziging van het zoekveld
        private async Task OnSearchTermChanged(ChangeEventArgs e)
        {
            formModel.Email = e.Value?.ToString() ?? string.Empty;
            // Voer de zoekopdracht uit als de input minstens 3 tekens lang is (werkt nog niet)
            if (!string.IsNullOrWhiteSpace(formModel.Email) && formModel.Email.Length >= 3)
            {
                suggestedUsers = await SearchUsersByEmail(formModel.Email);
                StateHasChanged();
            }
            else
            {
                suggestedUsers.Clear();
                StateHasChanged();
            }
        }

        // Methode om gebruikers op te zoeken met API met de e-mail
        private async Task<List<UserDto>> SearchUsersByEmail(string email)
        {
            try
            {
                var response = await Http.GetAsync($"https://localhost:7256/user/search?email={email}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UserDto>>(content) ?? new List<UserDto>(); // Als "sonConvert.DeserializeObject<List<UserDto>>(content)" NULL is dan wordt er een nieuwe lijst gemaakt
                }
                else
                {
                    Console.WriteLine($"Serverfout bij zoeken: {await response.Content.ReadAsStringAsync()}");
                    return new List<UserDto>(); // Retourneer lege lijst bij serverfouten
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error bij zoeken: {ex.Message}");
                return new List<UserDto>();
            }
        }

        // Wanneer een gebruiker wordt geselecteerd uit de suggesties
        private void SelectUser(UserDto user)
        {
            selectedUser = user;
            formModel.Email = user.Email;
            suggestedUsers.Clear();
        }

        // Methode om geforceerd een zoekopdracht uit te voeren wanneer op de button wordt geklikt.
        private async Task PerformSearch()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(formModel.Email))
                {
                    suggestedUsers = await SearchUsersByEmail(formModel.Email);
                    searchPerformed = true;
                    ErrorMessage = ""; // Reset foutmelding na succesvolle actie
                }
                else
                {
                    suggestedUsers.Clear();
                    searchPerformed = false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Zoeken mislukt. Controleer de verbinding en probeer het opnieuw."; // Log de uitzondering en toon een foutbericht
                Console.WriteLine($"Zoekfout: {ex.Message}");
                suggestedUsers.Clear();
                searchPerformed = false;
            }
        }

        // Controleert ofdat de email valid is
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void snackBarShow()
        {
            if (userSuccesToegevoegd)
            {
                Snackbar.Add("De gebruiker werd succesvol toegevoegd!", Severity.Success);
            }
        }

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