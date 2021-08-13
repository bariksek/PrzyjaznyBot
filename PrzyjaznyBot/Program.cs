using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.Configuration;
using PrzyjaznyBot.Commands;
using PrzyjaznyBot.Config;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PrzyjaznyBot
{
    class Program
    {
        static void Main(string[] args)
        {
            EstablishDbConnection();
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();

            var section = config.GetSection(nameof(AppConfig));
            var appConfig = section.Get<AppConfig>();

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = appConfig.Token ?? "token",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });

            commands.RegisterCommands<UserModule>();
            commands.RegisterCommands<BetModule>();
            commands.RegisterCommands<LolModule>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        static void EstablishDbConnection()
        {
            string dbName = "TestDatabase.db";

            if (File.Exists(dbName))
            {
                //return;
                File.Delete(dbName);
            }

            using (var dbContext = new MyDbContext())
            {
                //Ensure database is created
                dbContext.Database.EnsureCreated();

                if (!dbContext.Users.Any())
                {
                    dbContext.Users.AddRange(new User[]
                        {
                             new User{ Id=1, DiscordUserId=53253245235325, Username="jaszczur1337", Points=21.37, DateTime = DateTime.Now },
                             new User{ Id=2, DiscordUserId=2353425345325, Username="pudzian2", Points=0, DateTime = DateTime.Now  },
                             new User{ Id=3, DiscordUserId=322345234535, Username="huanpablo3", Points=335.1, DateTime = DateTime.Now },
                             new User{ Id=4, DiscordUserId=869487274189021215, Username="przyjazny-bot", Points=500, DateTime = DateTime.Now  },
                             new User{ Id=5, DiscordUserId=303260146384109568, Username="bariks", Points=50, DateTime = DateTime.Now.AddDays(-2)  },
                        });
                    dbContext.SaveChanges();
                }

                foreach (var user in dbContext.Users)
                {
                    Console.WriteLine($"UserId={user.Id}\tDiscordUserId={user.DiscordUserId}\tNickname={user.Username}\tValue={user.Points}\t");
                }
            }
        }
    }
}
