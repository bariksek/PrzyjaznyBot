using DSharpPlus;
using DSharpPlus.CommandsNext;
using PrzyjaznyBot.Commands;
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
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "ODY5NDg3Mjc0MTg5MDIxMjE1.YP-7IA.nEB252VorB2S6tYSqkaaxvL2EGg",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });

            commands.RegisterCommands<UserModule>();
            commands.RegisterCommands<BetModule>();

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
                             new User{ Id=1, DiscordUserId=53253245235325, Username="jaszczur1337", Value=21.37 },
                             new User{ Id=2, DiscordUserId=2353425345325, Username="pudzian2", Value=0 },
                             new User{ Id=3, DiscordUserId=322345234535, Username="huanpablo3", Value=335.1 },
                             new User{ Id=4, DiscordUserId=869487274189021215, Username="przyjazny-bot", Value=500 },
                             new User{ Id=5, DiscordUserId=303260146384109568, Username="bariks", Value=50 },
                        });
                    dbContext.SaveChanges();
                }

                foreach (var user in dbContext.Users)
                {
                    Console.WriteLine($"UserId={user.Id}\tDiscordUserId={user.DiscordUserId}\tNickname={user.Username}\tValue={user.Value}\t");
                }
            }
        }
    }
}
