using ModelLibrary.Data;
using Microsoft.EntityFrameworkCore;

namespace RealDolmenAPI.Controllers
{
    public class BenchController
    {
        public static void Map(WebApplication app)
        {
            app.MapGet("/Benchers", async (AppDbContext db) =>
            await db.Bench.ToListAsync());
        }
    }
}
