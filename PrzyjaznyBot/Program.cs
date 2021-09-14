using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PrzyjaznyBot.API;
using PrzyjaznyBot.Commands;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.DAL;
using System;
using System.Threading.Tasks;

namespace PrzyjaznyBot
{
    class Program
    {
        private static IServiceProvider serviceProvider;

        static void Main(string[] args)
        {
            ConfigureServices();
            PrepareDatabase();
            MainAsync().GetAwaiter().GetResult();
        }

        private static void PrepareDatabase()
        {
            var dbContext = serviceProvider.GetService<PostgreSqlContext>();

            dbContext.Database.Migrate();
        }

        static async Task MainAsync()
        {
            var configFetcher = serviceProvider.GetService<IConfigFetcher>();
            var appConfig = configFetcher.GetConfig();

            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = appConfig.Token ?? "token",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" },
                Services = serviceProvider
            });

            commands.RegisterCommands<UserModule>();
            commands.RegisterCommands<BetModule>();
            commands.RegisterCommands<LolModule>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static void ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddTransient<IConfigFetcher, ConfigFetcher>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IBetRepository, BetRepository>();
            services.AddTransient<ILolApi, LolApi>();
            services.AddDbContext<PostgreSqlContext>();

            serviceProvider = services.BuildServiceProvider();
        }
    }
}
