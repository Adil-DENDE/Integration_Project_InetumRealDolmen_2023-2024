// CREATIE VAN DE WEB APP ENVIRONMENT
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ModelLibrary.Data;
using ModelLibrary.Models;
using RealDolmenAPI.Controllers;
using RealDolmenAPI.Error;
using RealDolmenAPI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IBenchService, BenchService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<AuthService>();


builder.Services.AddEndpointsApiExplorer();


//Om via de dbcontext data beheren
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ajout de la configuration d'Identity
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole<int>>();


builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>

    {

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminManagerUserPolicy", o =>
    {
        o.RequireAuthenticatedUser();
        o.RequireRole("admin", "manager", "user");
    })
    .AddPolicy("AdminManagerPolicy", o => 
    {
        o.RequireAuthenticatedUser();
        o.RequireRole("admin", "manager");
    })
    .AddPolicy("AdminUserPolicy", o => 
    { 
        o.RequireAuthenticatedUser();
        o.RequireRole("admin", "user");
    });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo { Version = "v1" });

    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            }, Array.Empty<string>()
        }
    });
});


var app = builder.Build();
UserController.Map(app);
BenchController.Map(app);
OccupationController.Map(app);
OccupationHistoryController.Map(app);

// SWAGGER ====== TOONT EEN SOORT PAGE MET DOCU VAN API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowAnyOrigin");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

//Globale foutafhandeling
ErrorHandlingConfig.UseGlobalErrorHandling(app);

app.Run();


