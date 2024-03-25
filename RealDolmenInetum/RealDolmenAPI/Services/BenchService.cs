using Microsoft.EntityFrameworkCore;
using ModelLibrary.Data;
using ModelLibrary.Models;

namespace RealDolmenAPI.Services;
//interface => blauwdruk 
//
public interface IBenchService  
{
    public int Add(Bench bench);
    Task UpdateEndBenchAsync(int benchId, DateTime endBench);
    Bench GetActiveBenchForUser(int userId);
}
//implementatie
public class BenchService : IBenchService
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

    public async Task UpdateEndBenchAsync(int benchId, DateTime endBench)
    {
        var bench = await db.Bench.FirstOrDefaultAsync(b => b.Id == benchId);
        if (bench != null)
        {
            bench.End_bench = endBench;
            await db.SaveChangesAsync();
        }
    }

    public Bench GetActiveBenchForUser(int userId) => db.Bench.FirstOrDefault(b => b.User_id == userId && b.End_bench == null);
}

