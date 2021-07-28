using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class BetModule : BaseCommandModule
    {
        [Command("bet")]
        public async Task BetCommand(CommandContext ctx, int id, string condition, int value)
        {
            using (var dbContext = new MyDbContext())
            {
                if (value < 0)
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} nice try ( ͡° ͜ʖ ͡°).");
                    return;
                }

                User user = dbContext.Users.FirstOrDefault(u => u.DiscordUserId == ctx.Member.Id);
                Bet bet = dbContext.Bets.FirstOrDefault(b => b.Id == id);

                UserBet userBet = new UserBet
                {
                    User = user,
                    Bet = bet,
                    Condition = (Condition) Enum.Parse(typeof(Condition), condition),
                    Value = value
                };

                if (user.Value < value)
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} doesn't have enough points to transfer.");
                    return;
                }
                else
                {
                    user.Value -= value;
                }

                await dbContext.UserBets.AddAsync(userBet);
                await dbContext.SaveChangesAsync();

                StringBuilder response = new StringBuilder();
                response.AppendLine($"**{bet.Message}**");
                response.AppendLine($"{ctx.Member.Mention} just bet {value} for {condition}!");

                await ctx.RespondAsync(response.ToString());
            }
        }

        [Command("create")]
        public async Task CreateCommand(CommandContext ctx, string message)
        {
            using (var dbContext = new MyDbContext())
            {
                Bet bet = new Bet
                {
                    Author = dbContext.Users.FirstOrDefault(u => u.DiscordUserId == ctx.Member.Id),
                    IsActive = true,
                    Message = message
                };

                await dbContext.Bets.AddAsync(bet);
                var result = await dbContext.SaveChangesAsync();

                StringBuilder response = new StringBuilder();
                response.AppendLine($"{ctx.Member.Mention} created new bet - **{bet.Id}** - \"{message}\"");
                response.AppendLine($"If you want to join - type: !bet {bet.Id} (Yes/No) (number of points).");
                response.AppendLine($"For example: !bet 1 Yes 50");

                await ctx.RespondAsync(response.ToString());
            }
        }

    }
}
