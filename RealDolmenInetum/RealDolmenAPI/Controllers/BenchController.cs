﻿using ModelLibrary.Data;
using Microsoft.EntityFrameworkCore;
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
                                            bench => bench.user_id,
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

            app.MapPost("/bench/add", async (AppDbContext db, UserBench userBench) =>
            {
                // Check if userBench exists!
                if (userBench == null) return Results.BadRequest("Data is ongeldig!");

                // Convert userBench to Bench
                var bench = Bench.UserbenchToBench(userBench);

                // Debug purposes
                //return Results.BadRequest(bench.ToString());

                // Impossible, database should use an auto-increment field for the id field.
                // ID can not be determined from here!
                db.Bench.Add(bench);
                await db.SaveChangesAsync();

                return Results.Ok("User added to the bench successfully");
            });
        }
    }
}
