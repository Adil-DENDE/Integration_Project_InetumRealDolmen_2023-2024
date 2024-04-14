using Microsoft.EntityFrameworkCore;
using ModelLibrary.Data;
using ModelLibrary.Models;

namespace RealDolmenAPI.Controllers
{
    public class UserController
    {
    
        public static void Map(WebApplication app)
        {
            // Gebruik MapGroup om een groep te definiëren
            var userGroup = app.MapGroup("/user");

            // GET: Haal alle gebruikers op
            userGroup.MapGet("/", async (AppDbContext db) => await db.User.ToListAsync());

            // GET: Haal een specifieke gebruiker op op basis van ID // AANPASSEN // TODO // Nog joinen met project table
            userGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
            {
                // de verschillende groupjoins wouden niet werken dus ik heb het zo gedaan:
                // 1. Simpeler maken van de query
                // 2. Vermijden van het probleem waarbij EF Core niet in staat is om rare GroupJoins naar SQL te vertale
                var userAndBench = await db.User
                    .Where(u => u.Id == id)
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
                            CurrentBenchManagerId = b.Currentbenchmanager_id
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
