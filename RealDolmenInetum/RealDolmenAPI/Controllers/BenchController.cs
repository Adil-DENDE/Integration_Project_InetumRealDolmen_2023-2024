using ModelLibrary.Data;
using Microsoft.EntityFrameworkCore;
using RealDolmenAPI.Services;
using ModelLibrary.Dto;
using ModelLibrary.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace RealDolmenAPI.Controllers
{
    public class BenchController
    {
        public static void Map(WebApplication app)
        {
            // MAPGROUP OM NIET ELKE KEER /user/bench te schrijven // aanpassen
            var userBenchGroup = app.MapGroup("/user/bench");


            // GET: Haal gebruikers op de bench op
            userBenchGroup.MapGet("/", [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "AdminUserPolicy")]
            async (AppDbContext db) => {
                try
                {
                    // TABLE USER JOINEN MET TABLE BENCH
                    var usersOpBench = await db.User
                    .Join(db.Bench,
                    user => user.Id,
                    bench => bench.User_id,
                    (user, bench) => new { User = user, Bench = bench })
                    // CREATION D'une INSTANCE KIES ZELF DE DATA DIE GETOOND WORD
                    .Where(u => u.Bench.End_bench == null)
                    .Select(u => new
                    {
                        UserId = u.User.Id,
                        BenchId = u.Bench.Id,
                        Username = u.User.First_Name + " " + u.User.Last_Name,
                        Mail = u.User.Email,
                        NiveauId = u.User.Niveau_Id,
                        EndBench = u.Bench.End_bench,
                        StartBench = u.Bench.Start_bench,
                    })
                    .ToListAsync();
                    return Results.Ok(usersOpBench); // Gebruik Results.Ok om code 200 succes aan te geven
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Een fout opgetreden tijdens het ophalen van de gegevens: {ex.Message}");


                    return Results.Problem("Er is een fout opgetreden bij het ophalen van de gebruikers op de bench. Probeer het later opnieuw.");
                }
            });

            // GET: Vindt bench met user id waar End_bench gelijk is aan null
            userBenchGroup.MapGet("/user-bench/{userId:int}", async (int userId, AppDbContext db) =>
            {
                try
                {
                    var userBench = await db.Bench
                        .Where(bench => bench.User_id == userId && bench.End_bench == null)
                        .Select(bench => new
                        {
                            BenchId = bench.Id,
                            StartBench = bench.Start_bench,
                            EndBench = bench.End_bench, // Dit zal null zijn, omdat je zoekt naar actieve benches
                            UserId = bench.User_id
                        })
                        .FirstOrDefaultAsync();

                    if (userBench != null)
                    {
                        return Results.Ok(userBench);
                    }
                    else
                    {
                        return Results.NotFound($"Geen actieve bench gevonden voor gebruiker met ID: {userId}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Een fout opgetreden: {ex.Message}");
                    return Results.Problem("Er is een fout opgetreden bij het ophalen van de bench voor de gebruiker. Probeer het later opnieuw.");
                }
            });

            // POST: Voeg een gebruiker toe aan de bench
            userBenchGroup.MapPost("/add", async (UserBenchDto userBenchDto, IUserService userService, IBenchService benchService) =>
            {
                try
                {
                    if (userBenchDto == null)
                        return Results.BadRequest("Data is ongeldig!");

                    var userId = userService.GetIdByEmail(userBenchDto.Email);

                    // Controleer of de gebruiker al op de bench zit met een NULL End_bench
                    // Als de user al op de bench is met NULL = foutmelding
                    var existingBench = benchService.GetActiveBenchForUser(userId);
                    if (existingBench != null && existingBench.End_bench == null)
                        return Results.BadRequest("Gebruiker zit al op de bank en heeft een lopende sessie.");

                    // Voeg de gebruiker toe aan de bank
                    var bench = new Bench(userId, userBenchDto.StartBench);
                    benchService.Add(bench);

                    return Results.Ok("Gebruiker succesvol toegevoegd aan de bank.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Er is een fout opgetreden: {ex.Message}");
                    return Results.Problem("Er is een fout opgetreden bij het toevoegen van de gebruiker aan de bank.");
                }
            });

            // GET: Zoek een specifieke gebruiker op de bench op basis van e-mail
            userBenchGroup.MapGet("/search", async (AppDbContext db, string email) =>
            {
                try
                {
                    var results = await db.User
                        .Join(db.Bench, user => user.Id, bench => bench.User_id, (user, bench) => new { User = user, Bench = bench })
                        .Where(u => EF.Functions.Like(u.User.Email, $"%{email}%") && u.Bench.End_bench == null)
                        .Select(u => new
                        {
                            UserId = u.User.Id,
                            BenchId = u.Bench.Id,
                            Username = u.User.First_Name + " " + u.User.Last_Name,
                            Mail = u.User.Email,
                            NiveauId = u.User.Niveau_Id,
                            EndBench = u.Bench.End_bench,
                            StartBench = u.Bench.Start_bench,
                        })
                        .ToListAsync();

                    return Results.Ok(results);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Een fout opgetreden tijdens het zoeken: {ex.Message}");
                    return Results.Problem("Er is een fout opgetreden bij het zoeken naar de gebruiker.");
                }
            });

            // PUT: Update de end_bench waarde van een bestaande Bench record
            userBenchGroup.MapPut("/end/{benchId:int}", async (int benchId, UpdateEndBenchDto dto, IBenchService benchService) =>
            {
                try
                {
                    await benchService.UpdateEndBenchAsync(benchId, dto.EndBench);
                    return Results.Ok("End bench date updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Een fout opgetreden: {ex.Message}");
                    return Results.Problem("Er is een fout opgetreden bij het bijwerken van de end bench.");
                }
            });

            // PUT: Update de occupation_id waarde van een bestaande Bench record met behulp van de type
            userBenchGroup.MapPut("/occupation/{benchId}", async (int benchId, UpdateOccupationDto dto, AppDbContext db) =>
            {
                var occupation = await db.Occupation.FirstOrDefaultAsync(o => o.Type == dto.Type);
                if (occupation == null)
                {
                    return Results.NotFound("Occupation type niet gevonden.");
                }

                var bench = await db.Bench.FindAsync(benchId);
                if (bench == null)
                {
                    return Results.NotFound("Bench niet gevonden.");
                }

                bench.Occupation_id = occupation.Id;
                await db.SaveChangesAsync();

                return Results.Ok("Bench occupation is geupdate.");
            });

            // PUT: Zet de occupation_id van een bench op NULL met behulp van bench_id
            userBenchGroup.MapPut("/clearOccupation/{benchId}", async (int benchId, AppDbContext db) =>
            {
                var bench = await db.Bench.FindAsync(benchId);
                if (bench == null)
                {
                    return Results.NotFound("Bench niet gevonden.");
                }

                bench.Occupation_id = null;
                await db.SaveChangesAsync();

                return Results.Ok("Activiteit beëindigd en bench occupation is null.");
            });

            // PUT: Update de currentbenchmanager_id van een bestaande Bench record
            userBenchGroup.MapPut("/updateManager/{benchId:int}/{isNewManager:bool}", async (int benchId, bool isNewManager, AppDbContext db) =>
            {
                try
                {
                    var bench = await db.Bench.FindAsync(benchId);
                    if (bench == null)
                    {
                        return Results.NotFound($"Bench met ID {benchId} niet gevonden.");
                    }

                    bench.IsCurrentBenchManager = isNewManager;
                    await db.SaveChangesAsync();

                    return Results.Ok($"Bench manager geüpdatet naar nieuwe manager met ID {isNewManager}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Een fout opgetreden: {ex.Message}");
                    return Results.Problem("Er is een fout opgetreden bij het bijwerken van de bench manager. Probeer het later opnieuw.");
                }
            });

            // GET: Haal de huidige benchmanager op
            userBenchGroup.MapGet("/CurrentBenchManager", async (AppDbContext db) =>
            {
                try
                {
                    var currentManager = await db.Bench
                        .Where(bench => bench.IsCurrentBenchManager == true && bench.End_bench == null)
                        .Join(db.Users, bench => bench.User_id, user => user.Id, (bench, user) => new { Bench = bench, User = user })
                        .Select(result => new
                        {
                            BenchId = result.Bench.Id,
                            StartBench = result.Bench.Start_bench,
                            EndBench = result.Bench.End_bench,
                            UserId = result.User.Id,
                            FirstName = result.User.First_Name,
                            LastName = result.User.Last_Name,
                            Email = result.User.Email,
                            OccupationId = result.Bench.Occupation_id,
                            IsCurrentBenchManager = result.Bench.IsCurrentBenchManager
                        })
                        .FirstOrDefaultAsync();

                    if (currentManager != null)
                    {
                        return Results.Ok(currentManager);
                    }
                    else
                    {
                        return Results.NotFound("Er is momenteel geen actieve bench manager.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Een fout opgetreden: {ex.Message}");
                    return Results.Problem("Er is een fout opgetreden bij het ophalen van de huidige bench manager. Probeer het later opnieuw.");
                }
            });

            userBenchGroup.MapPut("/updateManagers", async (int oldManagerId, int newManagerId, AppDbContext db) =>
            {
                using var transaction = await db.Database.BeginTransactionAsync();
                try
                {
                    var oldManager = await db.Bench.FindAsync(oldManagerId);
                    var newManager = await db.Bench.FindAsync(newManagerId);

                    if (oldManager != null && newManager != null)
                    {
                        oldManager.IsCurrentBenchManager = false;
                        newManager.IsCurrentBenchManager = true;
                        await db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return Results.Ok("Managers updated successfully.");
                    }

                    return Results.NotFound("One or both managers not found.");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    return Results.Problem("Failed to update managers.");
                }
            });

        }
    }
}