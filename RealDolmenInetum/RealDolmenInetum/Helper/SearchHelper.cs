using ModelLibrary.Dto;
using Newtonsoft.Json;

namespace RealDolmenInetum.Helper
{
    // moet nog geinplementeerd worden in 'toevoegen' en 'ManagerUpdateDialog'
    public static class SearchHelpers
    {
        public static bool IsValidEmail(string email)
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

        public static async Task<List<UserDto>> SearchUsersByEmail(HttpClient http, string email)
        {
            try
            {
                var response = await http.GetAsync($"https://localhost:7256/user/search?email={email}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<UserDto>>(content) ?? new List<UserDto>();
                }
                else
                {
                    Console.WriteLine($"Serverfout bij zoeken: {await response.Content.ReadAsStringAsync()}");
                    return new List<UserDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error bij zoeken: {ex.Message}");
                return new List<UserDto>();
            }
        }

        public static async Task<string> FetchUserEmailById(HttpClient http, int userId)
        {
            if (userId > 0)
            {
                try
                {
                    var response = await http.GetAsync($"https://localhost:7256/user/userInfo/{userId}");
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var userInfo = JsonConvert.DeserializeObject<UserDto>(content);
                        return userInfo?.Email ?? string.Empty;
                    }
                    else
                    {
                        Console.WriteLine($"Gebruiker met ID {userId} niet gevonden.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error bij het ophalen van gebruikersinformatie: {ex.Message}");
                }
            }
            return string.Empty;
        }
    }

}
