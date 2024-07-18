using Microsoft.EntityFrameworkCore;
using TestGymBot.DataAccess.Entities;
using TestGymBot.DataAccess.Configurations;

namespace TestGymBot.DataAccess

{
    public class TgBotDbContext : DbContext
    {
        public TgBotDbContext()
        {

        }
        public TgBotDbContext(DbContextOptions options) : base(options) { 
        ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<PersonEntity> Persons { get; set; }
        public DbSet<RecordEntity> Records { get; set; }
        public DbSet<PersonPropsEntity> PersonProps { get; set; }
        public DbSet<TimeEntity> Times { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PersonConfiguration());
            modelBuilder.ApplyConfiguration(new PersonPropsConfiguration());
            modelBuilder.ApplyConfiguration(new TimeConfiguration());
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = MYBOOK_ZENITH\\SQLEXPRESS;Database=TestGymDb;Trusted_Connection=True;TrustServerCertificate=Yes");
        }
    }
}
