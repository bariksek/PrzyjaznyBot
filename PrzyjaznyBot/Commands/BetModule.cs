using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.EntityFrameworkCore;
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
        UserDA UserDA { get; set; }

        public BetModule()
        {
            UserDA = new UserDA();
        }

        [Command("bet")]
        [Description("Command for joining the existing bet.")]
        public async Task BetCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition, [Description("Points value")]int value)
        {
            using (var dbContext = new MyDbContext())
            {
                if (value <= 0)
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} nice try ( ͡° ͜ʖ ͡°).");
                    return;
                }

                var user = UserDA.GetUser(dbContext, ctx.Member);

                if (await user == null)
                {
                    user = UserDA.CreateNewUser(dbContext, ctx.Member);
                }

                Bet bet = dbContext.Bets.FirstOrDefault(b => b.Id == id && b.IsActive == true);

                if (bet == null)
                {
                    await ctx.RespondAsync($"Bet {id} not found or is not active.");
                    return;
                }

                User selectedUser = await user;

                UserBet userBet = new UserBet
                {
                    User = selectedUser,
                    Bet = bet,
                    Condition = (Condition)Enum.Parse(typeof(Condition), condition),
                    Value = value
                };

                if (selectedUser.Value < value)
                {
                    await ctx.RespondAsync($"{ctx.Member.Mention} doesn't have enough points to transfer.");
                    return;
                }
                else
                {
                    selectedUser.Value -= value;
                }

                var result = dbContext.UserBets.AddAsync(userBet);
                var result2 = dbContext.SaveChangesAsync();

                StringBuilder response = new StringBuilder();
                response.AppendLine($"**{bet.Message}**");
                response.AppendLine($"{ctx.Member.Mention} just bet {value} for {condition}!");

                await ctx.RespondAsync(response.ToString());
            }
        }

        [Command("create")]
        [Description("Command for creating a bet. Answers for now are just Yes or No.")]
        public async Task CreateCommand(CommandContext ctx, [Description("Bet message")]string message)
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

        [Command("finish")]
        [Description("Command for finishing a bet. Result can be only Yes or No.")]
        public async Task FinishCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition)
        {
            using (var dbContext = new MyDbContext())
            {
                var bet = dbContext.Bets.FirstOrDefault(b => b.Id == id && b.IsActive == true);

                if (bet == null)
                {
                    await ctx.RespondAsync($"Bet {id} not found or is not active.");
                    return;
                }

                if (bet.Author.DiscordUserId != ctx.Member.Id)
                {
                    await ctx.RespondAsync($"You are not the author of the bet!");
                    return;
                }

                bet.IsActive = false;

                var validUserBets = dbContext.UserBets.Where(ub => ub.Bet.Id == bet.Id);
                var successUserBets = validUserBets.Where(v => v.Condition == (Condition)Enum.Parse(typeof(Condition), condition));
                var failUserBets = validUserBets.Where(v => v.Condition != (Condition)Enum.Parse(typeof(Condition), condition));

                if (successUserBets.Count() == 0)
                {
                    await dbContext.SaveChangesAsync();
                    await ctx.RespondAsync("0 winners! Congratulations ( ͡° ͜ʖ ͡°)");
                    return;
                }

                var prizePool = failUserBets.Sum(f => f.Value);
                var prizePerUser = prizePool / successUserBets.Count();

                foreach (var userBet in successUserBets)
                {
                    User user = dbContext.Users.FirstOrDefault(u => u.Id == userBet.User.Id);

                    user.Value += userBet.Value + prizePerUser;
                }

                await dbContext.SaveChangesAsync();
                await ctx.RespondAsync("Bet finished!");
            }
        }
    }
}
