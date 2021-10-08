using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PrzyjaznyBot.API;
using PrzyjaznyBot.Commands.ButtonCommands;
using PrzyjaznyBot.Commands.TextCommands;
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
            var dbContextFactory = serviceProvider.GetService<IDbContextFactory<PostgreSqlContext>>();

            using var dbContext = dbContextFactory.CreateDbContext();
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

            discord.ComponentInteractionCreated += async (s, e) =>
            {
                var buttonResponseHelper = serviceProvider.GetService<IButtonResponseHelper>();
                await buttonResponseHelper.Resolve(e);
            };

            commands.RegisterCommands<UserModule>();
            commands.RegisterCommands<Commands.TextCommands.BetModule>();
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
            services.AddTransient<IButtonResponseHelper, ButtonResponseHelper>();
            services.AddTransient<ILolApi, LolApi>();
            services.AddDbContextFactory<PostgreSqlContext>();

            serviceProvider = services.BuildServiceProvider();
        }
    }
}
