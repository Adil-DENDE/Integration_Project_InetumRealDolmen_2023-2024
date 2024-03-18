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

            // GET: Haal een specifieke gebruiker op op basis van ID // AANPASSEN // TODO //
            userGroup.MapGet("/{id:int}", async (int id, AppDbContext db) =>
            {
                var userData = await db.User
                    .Where(u => u.Id == id)
                    .Join(db.Bench,
                        u => u.Id,
                        b => b.User_id,
                        (user, bench) => new
                        {
                            UserId = user.Id,
                            UserName = user.First_Name + " " + user.Last_Name,
                            ManagerId = user.Manager_Id,
                            UserEmail = user.Email,
                            BenchId = bench.Id,
                            StartBench = bench.Start_bench,
                            OccupationId = bench.Occupation_id,
                        })
                    .FirstOrDefaultAsync();

                if (userData != null)
                {
                    return Results.Ok(userData);
                }
                else
                {
                    return Results.NotFound();
                }
            });


            // GET: Zoek gebruikers op basis van email
            userGroup.MapGet("/search", async (AppDbContext db, string email) => await db.User.Where(u => EF.Functions.Like(u.Email, $"%{email}%")).ToListAsync());
        }
    }
}
