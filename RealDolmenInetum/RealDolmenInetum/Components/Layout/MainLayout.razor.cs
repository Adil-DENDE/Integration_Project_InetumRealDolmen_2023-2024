namespace RealDolmenInetum.Components.Layout
{
    public partial class MainLayout
    {
        public string username;
        private string userId;
        private string rol;

        protected override async Task OnInitializedAsync()
        {
            if (auth.GetToken() != null)
            {
                username = auth.GetUsername();
                userId = auth.GetIdFromLoggedUser();
                rol = auth.GetRolFromLoggedUser();
            }



        }

    }
}