using Anomaly.Data.Entities;
using Microsoft.EntityFrameworkCore;
using static Anomaly.Data.Constraints.UserEntityConstraints;

namespace Anomaly.Data
{
    public class DataContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; } = null!;

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserEntity>().ToTable(table =>
                table.HasCheckConstraint("Nickname", $"{NICKNAME_MIN_LENGTH} <= LENGTH(\"Nickname\")"));

            modelBuilder.Entity<UserEntity>().HasAlternateKey(user => user.Nickname);
            modelBuilder.Entity<UserEntity>().HasAlternateKey(user => user.Email);
        }
    }
}
