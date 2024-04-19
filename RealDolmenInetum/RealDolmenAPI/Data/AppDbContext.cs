using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModelLibrary.Models;

namespace ModelLibrary.Data
{
	public class AppDbContext : IdentityDbContext<User, IdentityRole<int>,int>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options)
		: base(options) 
        { 

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // ROLLEN
           // var manager = new IdentityRole("manager");
           //  manager.NormalizedName = "manager";

           //  var normalUser = new IdentityRole("admin");
           //  normalUser.NormalizedName = "normalUser";

           //  modelBuilder.Entity<IdentityRole>().HasData(manager, normalUser);

            // gecombineerde Primary key voor Project_User
            modelBuilder.Entity<Project_User>()
                .HasKey(pu => new { pu.User_Id, pu.Project_Id });
        }

        // OM TE INTERAGEREN MET DE TABLE USER 
        public DbSet<User> User => Set<User>();
		// OM TE INTERAGEREN MET DE TABLE PROJECT
		public DbSet<Project> Project => Set<Project>();
        // OM TE INTERAGEREN MET DE TABLE BENCH
        public DbSet<Bench> Bench => Set<Bench>();
        // OM TE INTERAGEREN MET DE TABLE OCCUPATION
        public DbSet<Occupation> Occupation => Set<Occupation>();
        // OM TE INTERAGEREN MET DE TABLE USERPROJECT
        public DbSet<Project_User> Project_User => Set<Project_User>();
        // OM TE INTERAGEREN MET DE TABLE OccupationHistory
        public DbSet<OccupationHistory> OccupationHistory => Set<OccupationHistory>();
        public DbSet<Niveau> niveaus => Set<Niveau>();
    }
}
