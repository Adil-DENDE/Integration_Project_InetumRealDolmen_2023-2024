using ModelLibrary.Data;
using ModelLibrary.Models;

namespace RealDolmenAPI.Services;

public interface IBenchService //interface => blauwdruk 
{
    public int Add(Bench bench);
}

public class BenchService : IBenchService//implementatie
{
    private readonly AppDbContext db;

    public BenchService(AppDbContext db)
    {
        this.db = db;
    }

    public int Add(Bench bench)
    {
        db.Bench.Add(bench);
        db.SaveChanges();
        return 0;
    }


    public string GetEmailById(Bench bench, User user)
    {

        bench.User_id = user.Id;
        return user.Email;
    }
    //public string GetEmailByIdC(int id)
    //{
    //    db.Bench.
    //    return user.Email;
    //}

}

