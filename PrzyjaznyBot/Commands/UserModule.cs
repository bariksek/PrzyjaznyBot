using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using PrzyjaznyBot.DAL;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class UserModule : BaseCommandModule
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

        [Command("stats")]
        public async Task StatsComand(CommandContext ctx)
        {
            using (var dbContext = new MyDbContext())
            {
                var sortedUsers = dbContext.Users.OrderByDescending(u => u.Value);
                
                StringBuilder statsMessage = new StringBuilder();
                int position = 0;

                foreach(var user in sortedUsers)
                {
                    position++;
                    statsMessage.AppendLine($"{position}. {user.Nickname} - {user.Value} pkt.");
                };

                await ctx.RespondAsync(statsMessage.ToString());
            }
        }
    }
}