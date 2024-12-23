using Microsoft.AspNetCore.Components;
using ModelLibrary.Dto;
using Newtonsoft.Json;

namespace RealDolmenInetum.Components.Pages.Manager
{
    public partial class UpdateBenchManagerDialog
    {
        [Parameter] public string CurrentEmail { get; set; }
        public UserBenchDto formModel = new UserBenchDto();
        private List<UserDto> suggestedUsers = new List<UserDto>();
        private bool searchPerformed = false;
        public string ErrorMessage { get; set; }


        // Methode om geforceerd een zoekopdracht uit te voeren wanneer op de button wordt geklikt.
        private async Task PerformSearch()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(formModel.Email))
                {
                    suggestedUsers = await SearchUsersByEmail(formModel.Email);
                    searchPerformed = true;
                    ErrorMessage = "";
                }
                else
                {
                    suggestedUsers.Clear();
                    searchPerformed = false;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Zoeken mislukt. Controleer de verbinding en probeer het opnieuw.";
                Console.WriteLine($"Zoekfout: {ex.Message}");
                suggestedUsers.Clear();
                searchPerformed = false;
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

        private async Task Submit()
        {

        }

        private void SelectUser(UserDto user)
        {
            // Logica om de geselecteerde gebruiker te verwerken
            formModel.Email = user.Email;
            // Eventuele verdere acties na selectie
        }

    }
}