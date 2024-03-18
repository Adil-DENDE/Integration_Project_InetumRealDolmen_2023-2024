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

            // GET: Haal alle occupations op
            occupationGroup.MapGet("/", async (AppDbContext db) => await db.Occupation.ToListAsync());

            // GET: Haal een specifieke occupation op op basis van ID
            occupationGroup.MapGet("/{id:int}", async (int id, AppDbContext db) => await db.Occupation.FirstOrDefaultAsync(o => o.Id == id) is Occupation occupation ? Results.Ok(occupation) : Results.NotFound());

        }
    }
}
