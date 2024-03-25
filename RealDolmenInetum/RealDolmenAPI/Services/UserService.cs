using ModelLibrary.Data;
using ModelLibrary.Models;

namespace RealDolmenAPI.Services;

//interface => blauwdruk
public interface IUserService  
{
    public int GetIdByEmail(string email);

}

//implementatie
public class UserService : IUserService
{
    private readonly AppDbContext db;

    public UserService(AppDbContext db)
    {
        this.db = db;
    }


    public int GetIdByEmail(string email)
    {
       var user = db.User.FirstOrDefault(u => u.Email == email);


        return user != null ? user.Id : -1;
    }
}

