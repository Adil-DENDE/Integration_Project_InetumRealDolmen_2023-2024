using ModelLibrary.Data;
using Microsoft.EntityFrameworkCore;
using RealDolmenAPI.Services;
using ModelLibrary.Dto;
using ModelLibrary.Models;

namespace RealDolmenAPI.Controllers
{
    public class BenchController
    {
        public static void Map(WebApplication app)
        {
            // MAPGROUP OM NIET ELKE KEER /user/bench te schrijven //
            var userBenchGroup = app.MapGroup("/user/bench");


            // GET: Haal gebruikers op de bench op
            userBenchGroup.MapGet("/", async (AppDbContext db) =>
            {
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



            // POST: Voeg een gebruiker toe aan de bench
            userBenchGroup.MapPost("/add", async (UserBenchDto userBenchDto, IUserService userService, IBenchService benchService) =>
            {
                try
                {
                    if (userBenchDto == null) return Results.BadRequest("Data is ongeldig!");

                    var userId = userService.GetIdByEmail(userBenchDto.Email);
                    var bench = new Bench(userId, userBenchDto.StartBench);
                    benchService.Add(bench);

                    return Results.Ok("User added to the bench successfully");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Een fout opgetreden: {ex.Message}");
                    return Results.Problem("Er bestaat geen user met deze e-mail adres.");
                }
            });

            // GET: Zoek een specifieke gebruiker op de bench op basis van e-mail
            userBenchGroup.MapGet("/search", async (AppDbContext db, string email) =>
            {
                try
                {
                    var results = await db.User
                        .Join(db.Bench, user => user.Id, bench => bench.User_id, (user, bench) => new { User = user, Bench = bench })
                        .Where(u => EF.Functions.Like(u.User.Email, $"%{email}%"))
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
        }
    }
}