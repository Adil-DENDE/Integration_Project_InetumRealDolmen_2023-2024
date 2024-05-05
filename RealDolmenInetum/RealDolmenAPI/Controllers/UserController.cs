using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ModelLibrary.Data;
using ModelLibrary.Dto;
using ModelLibrary.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealDolmenAPI.Controllers
{
    public class UserController
    {
    
        public static void Map(WebApplication app)
        {
            // Gebruik MapGroup om een groep te definiëren
            var userGroup = app.MapGroup("/user");

            // Gewoon om te testen
            userGroup.MapPost("/register", async (UserRegistrationDto model, UserManager<User> userManager) =>
            {
                var user = new User { UserName = model.Email, Email = model.Email, First_Name = model.First_Name, Last_Name = model.Last_Name };
                var result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    return Results.Ok("User account is gecreerd!");
                }
                else
                {
                    return Results.BadRequest(result.Errors);
                }
            });

            userGroup.MapPost("/account/create", async (string email, string password, string role, string first_name, string last_name, UserManager<User> userManager) =>
            {
                User User = await userManager.FindByEmailAsync(email);
                if (User != null) return Results.BadRequest(false);

                User user = new()
                {
                    First_Name = first_name,
                    Last_Name = last_name,
                    UserName = email,
                    PasswordHash = password,
                    Email = email, 
                };
                IdentityResult result = await userManager.CreateAsync(user, password);
                
                if (!result.Succeeded) return Results.BadRequest(false);
                Claim[] userClaims =
                [
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role),
                ];
                
                await userManager.AddClaimsAsync(user!, userClaims);
                return Results.Ok(true);
            });


            userGroup.MapPost("/login", async (UserLoginDto model, UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration) =>
            {
                User user = await userManager.FindByEmailAsync(model.Email);
                if (user == null) return Results.NotFound("User bestaat niet");

                SignInResult result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded) return Results.BadRequest("Invalid credentials");

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.First_Name + " " + user.Last_Name)
                    // ANDERE CLAIMS ?
            };

                var userClaims = await userManager.GetClaimsAsync(user);
                claims.AddRange(userClaims);

                var token = new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );

                return Results.Ok(new JwtSecurityTokenHandler().WriteToken(token));
            });





            userGroup.MapPost("/logout", async (HttpContext httpContext) =>
            {
                // JWT token wordt via de front end gedeleted
                return Results.Ok("Uitgelogd!");
            });




            // GET: Haal alle gebruikers op
            userGroup.MapGet("/", async (AppDbContext db) => await db.User.ToListAsync());

            // DIT IS HOE WE EEN ENDPOINT SECURE VOOR BEPAALDE ROLLEN. (test)
            userGroup.MapGet("test",
                [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminUserPolicy")] 
            async (AppDbContext db) => await db.User.ToListAsync());

            // GET: Haal een specifieke gebruiker op op basis van ID // AANPASSEN // TODO // Nog joinen met project table
            userGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
            {
                // de verschillende groupjoins wouden niet werken dus ik heb het zo gedaan:
                // 1. Simpeler maken van de query
                // 2. Vermijden van het probleem waarbij EF Core niet in staat is om rare GroupJoins naar SQL te vertale
                var userAndBench = await db.User
                    .Where(u => Convert.ToInt32(u.Id) == id)
                    .Select(u => new
                    {
                        u.Id,
                        UserName = u.First_Name + " " + u.Last_Name,
                        ManagerId = u.Manager_Id,
                        UserEmail = u.Email,
                        Bench = db.Bench
                        .Where(b => b.User_id == u.Id && b.End_bench == null)
                        .Select(b => new
                        {
                            b.Id,
                            b.Start_bench,
                            b.End_bench,
                            b.Occupation_id,
                            CurrentBenchManagerId = b.IsCurrentBenchManager
                        })
                        .FirstOrDefault()

                    })
                    .FirstOrDefaultAsync();

                if (userAndBench == null)
                {
                    return Results.NotFound();
                }

                // Probeer nu de projectdetails op te halen indien aanwezig // Aparte stap omdat niet elke gebruiker een project heeft // Maakt query ook simpeler

                var projectDetails = await db.Project_User
                    .Where(pu => pu.User_Id == id)
                    .SelectMany(pu => db.Project.Where(p => p.Id == pu.Project_Id).Select(p => p.Details))
                    .ToListAsync(); //selcteer alle projects en maakt er een enum van

                // De uiteindelijke response
                var response = new
                {
                    userAndBench.Id,
                    userAndBench.UserName,
                    userAndBench.ManagerId,
                    userAndBench.UserEmail,
                    BenchId = userAndBench.Bench?.Id,
                    StartBench = userAndBench.Bench?.Start_bench,
                    EndBench = userAndBench.Bench?.End_bench,
                    OccupationId = userAndBench.Bench?.Occupation_id,
                    CurrentBenchManagerId = userAndBench.Bench?.CurrentBenchManagerId,
                    ProjectDetails = projectDetails
                };

                return Results.Ok(response);
            });

            // GET: Zoek gebruikers op basis van email
            userGroup.MapGet("/search", async (AppDbContext db, string email) => await db.User.Where(u => EF.Functions.Like(u.Email, $"%{email}%")).ToListAsync());

            // GET: Gebruikersinformatie (voornaam, achternaam, email) op basis van ID
            userGroup.MapGet("/userInfo/{id:int}", async (int id, AppDbContext db) =>
            {
                var userInfo = await db.User
                    .Where(u => u.Id == id)
                    .Select(u => new
                    {
                        u.Id,
                        u.First_Name,
                        u.Last_Name,
                        u.Email
                    })
                    .FirstOrDefaultAsync();

                if (userInfo == null)
                {
                    return Results.NotFound($"Gebruiker met ID {id} niet gevonden.");
                }

                return Results.Ok(userInfo);
            });
        }
    }
}
