/*using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using PrzyjaznyBot.DAL;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class BetModule : BaseCommandModule
    {
        private readonly UserRepository UserRepository;
        private readonly BetRepository BetRepository;

        public BetModule()
        {
            UserRepository = new UserRepository();
            BetRepository = new BetRepository();
        }

        [Command("bet")]
        [Description("Command for joining the existing bet.")]
        public async Task BetCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition, [Description("Points value")]int value)
        {
            if (value <= 0)
            {
                await ctx.RespondAsync($"{ctx.Member.Mention} nice try ( ͡° ͜ʖ ͡°).");
                return;
            }

            var user = UserRepository.GetUser(ctx.Member);

            if (user == null)
            {
                await UserRepository.CreateNewUser(ctx.Member);
            }

            var bet = BetRepository.GetBet(id);

            if (bet == null || !bet.IsActive)
            {
                await ctx.RespondAsync($"Bet {id} not found or is not active.");
                return;
            }

            var result = await BetRepository.CreateUserBet(ctx.Member, id, condition, value);

            if (result == null)
            {
                await ctx.RespondAsync($"Cannot create user bet.");
                return;
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine($"**{bet.Message}**");
            response.AppendLine($"{ctx.Member.Mention} just bet {value} for {condition}!");

            await ctx.RespondAsync(response.ToString());
        }

        [Command("create")]
        [Description("Command for creating a bet. Answers for now are just Yes or No.")]
        public async Task CreateCommand(CommandContext ctx, [Description("Bet message")]string message)
        {
            var author = UserRepository.GetUser(ctx.Member);
            var bet = await BetRepository.CreateBet(author, message);

            StringBuilder response = new StringBuilder();
            response.AppendLine($"{ctx.Member.Mention} created new bet - **{bet.Id}** - \"{message}\"");
            response.AppendLine($"If you want to join - type: !bet {bet.Id} (Yes/No) (number of points).");
            response.AppendLine($"For example: !bet 1 Yes 50");

            await ctx.RespondAsync(response.ToString());
        }

        [Command("finish")]
        [Description("Command for finishing a bet. Result can be only Yes or No.")]
        public async Task FinishCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition)
        {
            var result = await BetRepository.FinishBet(id, ctx.Member, condition);

            if (!result)
            {
                await ctx.RespondAsync($"Bet cannot be finished.");
                return;
            }

            await ctx.RespondAsync("Bet finished!");
        }
    }
}
*/