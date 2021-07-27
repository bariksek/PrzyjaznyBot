using DSharpPlus;
using DSharpPlus.CommandsNext;
using PrzyjaznyBot.Commands;
using System.Threading.Tasks;

namespace PrzyjaznyBot
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "ODY5NDg3Mjc0MTg5MDIxMjE1.YP-7IA.V3paj0TqBb6L19B4_7HkMB6JUps",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });

            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "!" }
            });

            commands.RegisterCommands<TestModule>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
