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
                Token = "token",
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
                             new User{ UserId=1, Nickname="jaszczur1337", Value=21.37 },
                             new User{ UserId=2, Nickname="pudzian2", Value=0 },
                             new User{ UserId=3, Nickname="huanpablo3", Value=335.1 },
                             new User{ UserId=4, Nickname="monster99", Value=21.37 },
                        });
                    dbContext.SaveChanges();
                }

                foreach (var user in dbContext.Users)
                {
                    Console.WriteLine($"UserId={user.UserId}\tNickname={user.Nickname}\tValue={user.Value}\t");
                }
            }
        }
    }
}
