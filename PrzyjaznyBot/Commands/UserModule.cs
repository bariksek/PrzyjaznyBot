using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using PrzyjaznyBot.DAL;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class UserModule : BaseCommandModule
    {
        [Command("elo")]
        [Description("Command used for simply saying hello to the bot ;)")]
        public async Task EloComand(CommandContext ctx)
        {
            await ctx.RespondAsync($"Elo {ctx.Member.Mention} mordo, jak tam zdróweczko?");
        }

        [Command("transfer")]
        [Description("Command for transfering money to another user.")]
        public async Task TransferComand(CommandContext ctx, [Description("Tagged discord member - @user")]DiscordMember targetMember, [Description("Points value")] double value)
        {
            if (value < 0)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} nice try ( ͡° ͜ʖ ͡°).");
                return;
            }

            using (var dbContext = new MyDbContext())
            {
                var users = dbContext.Users;

                var target = users.FirstOrDefault(u => u.DiscordUserId == targetMember.Id);
                var sender = users.FirstOrDefault(u => u.DiscordUserId == ctx.Member.Id);

                if (sender.Value < value)
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} doesn't have enough points to transfer.");
                }
                else
                {
                    sender.Value -= value;
                    target.Value += value;

                    var result = await dbContext.SaveChangesAsync();
                    await ctx.RespondAsync($"{ctx.Member.Mention} successfully sent {value} points to {targetMember.Mention}.");
                }
            }
        }

        [Command("stats")]
        [Description("Command for showing statistics about points and users.")]
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