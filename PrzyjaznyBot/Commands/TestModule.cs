using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class TestModule : BaseCommandModule
    {
        [Command("greet")]
        public async Task GreetComand(CommandContext ctx)
        {
            await ctx.RespondAsync("Greetings! Thank you for executing me!");
        }
    }
}