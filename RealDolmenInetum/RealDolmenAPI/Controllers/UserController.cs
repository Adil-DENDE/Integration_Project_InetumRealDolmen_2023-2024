using Microsoft.EntityFrameworkCore;
using ModelLibrary.Data;
using ModelLibrary.Models;

namespace RealDolmenAPI.Controllers
{
    public class UserController
    {
        public static void Map(WebApplication app)
        {
            // ENDPOINT OM ALLE BENCHERS TE ZIEN //WE ZIEN NU NOG ALTIJD ALLE USERS (MANAGERS STAAN ER TUSSEN).
            app.MapGet("/User", async (AppDbContext db) => await db.User.ToListAsync());

            // ENDPOINT ALS WE DE DATA VAN EEN BENCHER ZOUDEN WILLEN ZIEN 
            app.MapGet("/User/{id:int}", async (int id, AppDbContext db) => await db.User.FirstOrDefaultAsync(u=>u.Id==id) is User user ? Results.Ok(user) : Results.NotFound());

        }
    }
}
