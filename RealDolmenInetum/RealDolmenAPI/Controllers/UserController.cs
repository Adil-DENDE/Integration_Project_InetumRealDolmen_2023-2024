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

            // ENDPOINT OM ALLE BENCHERS TE ZIEN //WE ZIEN NU NOG ALTIJD ALLE USERS (MANAGERS STAAN ER TUSSEN).
            userGroup.MapGet("/", async (AppDbContext db) => await db.User.ToListAsync());

            // ENDPOINT ALS WE DE DATA VAN EEN BENCHER ZOUDEN WILLEN ZIEN 
            userGroup.MapGet("/{id:int}", async (int id, AppDbContext db) => await db.User.FirstOrDefaultAsync(u=>u.Id==id) is User user ? Results.Ok(user) : Results.NotFound());

            // endpoint voor het zoeken van users op basis van email
            userGroup.MapGet("/search", async (AppDbContext db, string email) => await db.User.Where(u => EF.Functions.Like(u.Email, $"%{email}%")).ToListAsync());
        }
    }
}
