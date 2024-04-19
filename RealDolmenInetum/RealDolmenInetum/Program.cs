using RealDolmenInetum.Components;
using MudBlazor.Services;
using MudBlazor;
using RealDolmenAPI.Services;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Data;
using Blazored.LocalStorage;
var builder = WebApplication.CreateBuilder(args);

// Template: Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
}
);

builder.Services.AddBlazoredLocalStorage();


// ZODAT DE INJECT WERKT IN HET PROJECT
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Configurez la chaîne de connexion ou les options ici
    options.UseSqlServer(builder.Configuration.GetConnectionString("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=IntegrationProject;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));
});

builder.Services.AddScoped<IUserService, UserService>();
// ZODAT DE GESAVED TOKEN OVERAL KAN GEBRUIK WORDEN RESOLVED:
builder.Services.AddSingleton<AuthService>();


var app = builder.Build();

// Template: Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // Template: The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();
