using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.Model;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class UserModule : BaseCommandModule
    {
        UserDA UserDA { get; set; }

        public UserModule()
        {
            UserDA = new UserDA();
        }

        [Command("elo")]
        [Description("Command used for simply saying hello to the bot ;)")]
        public async Task EloCommand(CommandContext ctx)
        {
            await ctx.RespondAsync($"Elo {ctx.Member.Mention} mordo, jak tam zdróweczko?");
        }

        [Command("init")]
        [Description("Command used for creating a user in database.")]
        public async Task InitCommand(CommandContext ctx)
        {
            using (var dbContext = new MyDbContext())
            {
                var user = UserDA.GetUser(dbContext, ctx.Member);

                if (await user == null)
                {
                    user = UserDA.CreateNewUser(dbContext, ctx.Member);

                    var newUser = await user;
                    await ctx.RespondAsync($"New user **{newUser.Username}** has been successfully created.");

                    return;
                }

                var existingUser = await user;
                await ctx.RespondAsync($"User **{existingUser.Username}** already exists.");
            }
        }

        [Command("transfer")]
        [Description("Command for transfering money to another user.")]
        public async Task TransferCommand(CommandContext ctx, [Description("Tagged discord member - @user")]DiscordMember targetMember, [Description("Points value")] double value)
        {
            if (value <= 0)
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
        public async Task StatsCommand(CommandContext ctx)
        {
            using (var dbContext = new MyDbContext())
            {
                var sortedUsers = dbContext.Users.OrderByDescending(u => u.Value);
                
                StringBuilder statsMessage = new StringBuilder();
                int position = 0;

                foreach(var user in sortedUsers)
                {
                    position++;
                    statsMessage.AppendLine($"{position}. {user.Username} - {user.Value} pkt.");
                };

                await ctx.RespondAsync(statsMessage.ToString());
            }
        }
    }
}