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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configFetcher = new ConfigFetcher();
            var config = configFetcher.GetConfig();

            optionsBuilder.UseNpgsql(config.DbConnection);
        }

    }
}
