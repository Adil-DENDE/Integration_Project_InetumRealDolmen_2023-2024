using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ModelLibrary.Dto;

namespace RealDolmenInetum.Components.Pages.Login
{
    public partial class Login
    {
        private UserLoginDto userLoginModel = new UserLoginDto();
        private string errorMessage = "";
        private bool shouldShowInvalidLoginAlert = false;

        private async Task HandleLogin(EditContext editContext)
        {
            var response = await Http.PostAsJsonAsync("https://localhost:7256/user/login", userLoginModel);
            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                token.Trim().Trim('"');

                auth.SetToken(token);
                await LocalStorage.SetItemAsync("authToken", token);

                NavigationManager.NavigateTo("/home");
            }
            else
            {
                errorMessage = "Inloggen mislukt. Controleer uw inloggegevens en probeer het opnieuw.";
          
            }
        }
    }
}