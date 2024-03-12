using ModelLibrary.Data;
using ModelLibrary.Models;

namespace RealDolmenAPI.Services;

public interface IUserService //interface => blauwdruk 
{
    public int GetIdByEmail(string email);

}

public class UserService : IUserService//implementatie
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

