using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class TestModule : BaseCommandModule
    {
        [Command("elo")]
        public async Task EloComand(CommandContext ctx)
        {
            await ctx.RespondAsync($"Elo {ctx.Member.Mention} mordo, jak tam zdróweczko?");
        }
        
        [Command("bet")]
        public async Task BetCommand(CommandContext ctx, int id, int win, int value)
        {
            await ctx.RespondAsync($"Ostrożnie! {ctx.Member.Mention} obstawia {value} złotych polskich na status: {win}!");
        }
    }
}