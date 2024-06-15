namespace RealDolmenInetum.Components.Pages.Bencher
{
    public partial class ActiviteitenBeheer
    {
        private string ErrorMessage;
        private int userBenchId;
        private int occupation;



        protected override async Task OnInitializedAsync()
        {
            await getUserBenchId();
        }


        // METHODE DIE EEN USER CHECKT OF HIJ WEL EEN BENCH ID HEEFT EN ALS HIJ WEL EEN OCCUPATION HEEFT 
        // DE NAAM MOET VERANDEREN VAN DE METHODE 
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
                    userBenchId = user.benchId;
                    occupation = user.occupationId;
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



    }
}