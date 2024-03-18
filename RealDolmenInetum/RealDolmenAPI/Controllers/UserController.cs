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

            // GET: Haal een specifieke gebruiker op op basis van ID
            userGroup.MapGet("/{id:int}", async (int id, AppDbContext db) => await db.User.FirstOrDefaultAsync(u=>u.Id==id) is User user ? Results.Ok(user) : Results.NotFound());

            // GET: Zoek gebruikers op basis van email
            userGroup.MapGet("/search", async (AppDbContext db, string email) => await db.User.Where(u => EF.Functions.Like(u.Email, $"%{email}%")).ToListAsync());
        }
    }
}
