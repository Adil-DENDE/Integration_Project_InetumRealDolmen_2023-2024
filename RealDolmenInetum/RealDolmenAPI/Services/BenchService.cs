using Microsoft.EntityFrameworkCore;
using ModelLibrary.Data;
using ModelLibrary.Models;

namespace RealDolmenAPI.Services;

public interface IBenchService //interface => blauwdruk 
{
    public int Add(Bench bench);
    Task UpdateEndBenchAsync(int benchId, DateTime endBench);
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

    public async Task UpdateEndBenchAsync(int benchId, DateTime endBench)
    {
        var bench = await db.Bench.FirstOrDefaultAsync(b => b.Id == benchId);
        if (bench != null)
        {
            bench.End_bench = endBench;
            await db.SaveChangesAsync();
        }
    }
}

