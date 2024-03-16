using ModelLibrary.Data;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;

namespace RealDolmenAPI.Controllers
{
    public class OccupationController
    {
        public static void Map(WebApplication app)
        {
            // Gebruik MapGroup om een groep te definiëren
            var occupationGroup = app.MapGroup("/occupation");

            // Endpoint om alle ocupations te zien
            occupationGroup.MapGet("/", async (AppDbContext db) => await db.Occupation.ToListAsync());

            // If you need to retrieve a single occupation by ID in the future, you can add a similar endpoint as shown for users
            occupationGroup.MapGet("/{id:int}", async (int id, AppDbContext db) => await db.Occupation.FirstOrDefaultAsync(o => o.Id == id) is Occupation occupation ? Results.Ok(occupation) : Results.NotFound());

        }
    }
}
