using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using PrzyjaznyBot.API;
using PrzyjaznyBot.Commands.ButtonCommands;
using PrzyjaznyBot.Commands.TextCommands;
namespace PrzyjaznyBot
{
    class Program
    {
        static void Main()
        {
            var serviceProvider = CreateServiceProvider();
            MainAsync(serviceProvider).GetAwaiter().GetResult();
        }

        static async Task MainAsync(IServiceProvider serviceProvider)
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = await Decrypt(serviceProvider, Environment.GetEnvironmentVariable("DiscordToken")),
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
                using var scope = serviceProvider.CreateScope();
                var buttonResponseHelper = scope.ServiceProvider.GetRequiredService<IButtonResponseHelper>();
                await buttonResponseHelper.Resolve(e);
            };

            commands.RegisterCommands<UserModule>();
            commands.RegisterCommands<Commands.TextCommands.BetModule>();
            commands.RegisterCommands<LolModule>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task<string> Decrypt(IServiceProvider serviceProvider, string? cypher)
        {
            if(cypher is null)
            {
                return string.Empty;
            }

            using var scope = serviceProvider.CreateScope();
            var encryptionServiceClient = scope.ServiceProvider.GetRequiredService<EncryptionService.EncryptionServiceClient>();
            var decryptRequest = new DecryptRequest
            {
                Cipher = cypher
            };

            var decryptResponse = await encryptionServiceClient.DecryptAsync(decryptRequest);

            return decryptResponse.Message;
        }

        private static IServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddGrpcClient<EncryptionService.EncryptionServiceClient>(options =>
            {
                options.Address = new Uri(Environment.GetEnvironmentVariable("EncryptionServiceAddress") ?? "");
            });
            services.AddGrpcClient<UserService.UserService.UserServiceClient>(options =>
            {
                options.Address = new Uri(Environment.GetEnvironmentVariable("UserServiceAddress") ?? "");
            });
            services.AddGrpcClient<BetService.BetService.BetServiceClient>(options =>
            {
                options.Address = new Uri(Environment.GetEnvironmentVariable("BetServiceAddress") ?? "");
            });
            services.AddTransient<IButtonResponseHelper, ButtonResponseHelper>();
            services.AddTransient<ILolApi, LolApi>();

            return services.BuildServiceProvider();
        }
    }
}
