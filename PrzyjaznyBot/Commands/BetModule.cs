using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.DTO.BetRepository;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class BetModule : BaseCommandModule
    {
        private readonly BetRepository BetRepository;

        public BetModule()
        {
            BetRepository = new BetRepository();
        }

        [Command("bet")]
        [Description("Command for joining the existing bet.")]
        public async Task BetCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition, [Description("Points value")]int value)
        {
            if (value <= 0)
            {
                await ctx.RespondAsync("Points have to be greater than 0");
            }

            var createUserBetRequest = new CreateUserBetRequest
            {
                BetId = id,
                Condition = condition,
                DiscordId = ctx.Member.Id,
                Value = value
            };

            var createUserBetResponse = await BetRepository.CreateUserBet(createUserBetRequest);

            if (!createUserBetResponse.Success)
            {
                await ctx.RespondAsync(createUserBetResponse.Message);
                return;
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine($"{ctx.Member.Mention} just bet {value} for bet with Id: {id} on {condition}!");
            await ctx.RespondAsync(response.ToString());
        }

        [Command("create")]
        [Description("Command for creating a bet. Answers for now are just Yes or No.")]
        public async Task CreateCommand(CommandContext ctx, [Description("Bet message")]string message)
        {
            var createBetRequest = new CreateBetRequest
            {
                DiscordId = ctx.Member.Id,
                Message = message
            };

            var createBetResponse = await BetRepository.CreateBet(createBetRequest);

            if (!createBetResponse.Success)
            {
                await ctx.RespondAsync(createBetResponse.Message);
                return;
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine($"{ctx.Member.Mention} created new bet - **{createBetResponse.Bet.Id}** - \"{message}\"");
            response.AppendLine($"If you want to join - type: !bet {createBetResponse.Bet.Id} (Yes/No) (number of points).");
            response.AppendLine($"For example: !bet 1 Yes 50");

            await ctx.RespondAsync(response.ToString());
        }

        [Command("finish")]
        [Description("Command for finishing a bet. Result can be only Yes or No.")]
        public async Task FinishCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition)
        {
            var finishBetRequest = new FinishBetRequest
            {
                BetId = id,
                Condition = condition,
                DiscordId = ctx.Member.Id
            };

            var finishBetResponse = await BetRepository.FinishBet(finishBetRequest);

            if (!finishBetResponse.Success)
            {
                await ctx.RespondAsync(finishBetResponse.Message);
                return;
            }

            await ctx.RespondAsync("Bet finished!");
        }
    }
}
