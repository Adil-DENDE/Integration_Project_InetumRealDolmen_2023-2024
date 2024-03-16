using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;

namespace ModelLibrary.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) { }


		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=IntegrationProject;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
		}

		// OM TE INTERAGEREN MET DE TABLE USER 
		public DbSet<User> User => Set<User>();
		// OM TE INTERAGEREN MET DE TABLE PROJECT
		public DbSet<Project> Project => Set<Project>();
        // OM TE INTERAGEREN MET DE TABLE BENCH
        public DbSet<Bench> Bench => Set<Bench>();
        // OM TE INTERAGEREN MET DE TABLE OCCUPATION
        public DbSet<Occupation> Occupation => Set<Occupation>();
    }
}
