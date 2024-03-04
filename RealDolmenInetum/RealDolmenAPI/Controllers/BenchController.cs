using ModelLibrary.Data;
using Microsoft.EntityFrameworkCore;

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
                                        .Select(u => new {
                                            UserId = u.User.Id,
                                            BenchId = u.Bench.Id,
                                            Username = u.User.First_Name + " " + u.User.Last_Name,
                                            mail = u.User.Email,
                                            NiveauId = u.User.Niveau_Id
                                        })
                                        .ToListAsync();
                return usersOpBench;
            });
        }
    }
}
