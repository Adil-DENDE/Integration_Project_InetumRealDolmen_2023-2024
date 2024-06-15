namespace RealDolmenInetum.Components.Layout
{
    public partial class NavMenu
    {
        private async Task Logout()
        {
            await LocalStorage.RemoveItemAsync("authToken");
            auth.ClearToken();
            Navigation.NavigateTo("/", forceLoad: true);
        }
    }
}