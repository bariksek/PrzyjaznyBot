using Microsoft.EntityFrameworkCore;
using PrzyjaznyBot.Model;
using System.Reflection;

namespace PrzyjaznyBot.DAL
{
    public class MyDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=TestDatabase.db", options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names
            modelBuilder.Entity<User>().ToTable("User", "test");
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
