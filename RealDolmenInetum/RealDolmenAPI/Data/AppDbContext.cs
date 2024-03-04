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
			optionsBuilder.UseSqlServer("Server=tcp:sqlserverintegrationproject.database.windows.net,1433;Initial Catalog=IntegrationProject;Persist Security Info=False;User ID=Adil;Password=Integrationproject123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
		}

		// OM TE INTERAGEREN MET DE TABLE USER 
		public DbSet<User> User => Set<User>();
		// OM TE INTERAGEREN MET DE TABLE PROJECT
		public DbSet<Project> Project => Set<Project>();
	}
}
