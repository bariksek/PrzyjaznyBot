using Microsoft.EntityFrameworkCore;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.Model;

namespace PrzyjaznyBot.DAL
{
    public class PostgreSqlContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Bet> Bets { get; set; }
        public DbSet<UserBet> UserBets { get; set; }

        private readonly string _dbConnection;

        public PostgreSqlContext(IConfigFetcher configFetcher)
        {
            var appConfig = configFetcher.GetConfig();
            _dbConnection = appConfig.DbConnection;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_dbConnection);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Bet>().ToTable("Bet");
            modelBuilder.Entity<Bet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserBet>().ToTable("UserBet");
            modelBuilder.Entity<UserBet>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
