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
            app.MapGet("/Users/Bench", async (AppDbContext db) =>
            {
                // TABLE USER JOINEN MET TABLE BENCH
                var usersOpBench = await db.User
                                        .Join(db.Bench,
                                            user => user.Id,
                                            bench => bench.User_id,
                                            (user, bench) => new { User = user, Bench = bench })
                                        // CREATION D'une INSTANCE KIES ZELF DE DATA DIE GETOOND WORD
                                        .Where(u => u.Bench.End_bench == null)
                                        .Select(u => new {
                                            UserId = u.User.Id,
                                            BenchId = u.Bench.Id,
                                            Username = u.User.First_Name + " " + u.User.Last_Name,
                                            Mail = u.User.Email,
                                            NiveauId = u.User.Niveau_Id,
                                            EndBench = u.Bench.End_bench,
                                            StartBench = u.Bench.Start_bench,
                                        })
                                        .ToListAsync();
                return usersOpBench;
            });

            app.MapPost("/bench/add", async (UserBenchDto userBenchDto, IUserService userService, IBenchService benchService) =>
            {
                // Check if userBench exists!
                if (userBenchDto == null) return Results.BadRequest("Data is ongeldig!");

                // Get userId based on email
               var userId = userService.GetIdByEmail(userBenchDto.Email);
                // Debug purposes
                //return Results.BadRequest(bench.ToString());
                var bench = new Bench(userId, userBenchDto.StartBench);
                // Impossible, database should use an auto-increment field for the id field.
                // ID can not be determined from here!
                //db.Bench.Add(bench);
                benchService.Add(bench);
                //use bench service
                //benchService.Add()

                return Results.Ok("User added to the bench successfully");
            });
        }
    }
}
