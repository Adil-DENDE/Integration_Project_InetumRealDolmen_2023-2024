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

        }
    }
}
