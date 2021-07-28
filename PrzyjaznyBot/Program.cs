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
                Token = "ODY5NDg3Mjc0MTg5MDIxMjE1.YP-7IA.LCCn7yzSp2s0n_-CvONW9IauVSk",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });

            commands.RegisterCommands<UserModule>();
            
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
                             new User{ UserId=1, DiscordUserId = 53253245235325, Nickname="jaszczur1337", Value=21.37 },
                             new User{ UserId=2, DiscordUserId = 2353425345325, Nickname="pudzian2", Value=0 },
                             new User{ UserId=3, DiscordUserId = 322345234535, Nickname="huanpablo3", Value=335.1 },
                             new User{ UserId=4, DiscordUserId = 869487274189021215, Nickname="przyjazny-bot", Value=500 },
                             new User{ UserId=5, DiscordUserId = 303260146384109568, Nickname="bariks", Value=50 },
                        });
                    dbContext.SaveChanges();
                }

                foreach (var user in dbContext.Users)
                {
                    Console.WriteLine($"UserId={user.UserId}\tDiscordUserId={user.DiscordUserId}\tNickname={user.Nickname}\tValue={user.Value}\t");
                }
            }
        }
    }
}
