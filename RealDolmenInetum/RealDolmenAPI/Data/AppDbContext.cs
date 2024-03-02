using Microsoft.EntityFrameworkCore;
using RealDolmenAPI.Models;

namespace RealDolmenAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=tcp:sqlserverintegrationproject.database.windows.net,1433;Initial Catalog=IntegrationProject;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=\"Active Directory Default\";");
        }

        // OM TE INTERAGEREN MET DE TABLE USER 
        public DbSet<User> Users => Set<User>();
    }
}
