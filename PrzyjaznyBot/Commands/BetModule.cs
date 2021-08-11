using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.DTO.BetRepository;
using System.Linq;
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

        [Command("bets")]
        [Description("Command for joining the existing bet.")]
        public async Task BetsCommand(CommandContext ctx, [Description("Show finished bets - true. Default false.")] bool showNotActive = false)
        {
            var getBetsRequest = new GetBetsRequest
            {
                ShowNotActive = showNotActive
            };

            var getUsersResponse = await BetRepository.GetBets(getBetsRequest);

            if (!getUsersResponse.Success)
            {
                await ctx.RespondAsync(getUsersResponse.Message);
                return;
            }

            StringBuilder betsMessage = new StringBuilder();
            int position = 0;

            foreach (var bet in getUsersResponse.Bets.OrderByDescending(u => u.IsActive).ThenByDescending(u => u.DateTime))
            {
                position++;
                betsMessage.AppendLine($"{position}. BetId: {bet.Id} - {bet.Message} - Stopped: {bet.IsStopped} - Active: {bet.IsActive} - Stake: {bet.Stake:N2}");
            };

            await ctx.RespondAsync(betsMessage.ToString());
        }

        [Command("bet")]
        [Description("Command for joining the existing bet.")]
        public async Task BetCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition)
        {
            var createUserBetRequest = new CreateUserBetRequest
            {
                BetId = id,
                Condition = condition,
                DiscordId = ctx.Member.Id
            };

            var createUserBetResponse = await BetRepository.CreateUserBet(createUserBetRequest);

            if (!createUserBetResponse.Success)
            {
                await ctx.RespondAsync(createUserBetResponse.Message);
                return;
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine($"{ctx.Member.Mention} just bet on {condition} for bet with Id: {id}!");
            await ctx.RespondAsync(response.ToString());
        }

        [Command("create")]
        [Description("Command for creating a bet. Answers for now are just Yes or No.")]
        public async Task CreateCommand(CommandContext ctx, [Description("Bet message")]string message, [Description("Bet stake")]double stake)
        {
            if(stake <= 0)
            {
                await ctx.RespondAsync("Stake has to be greater than 0");
                return;
            }

            var createBetRequest = new CreateBetRequest
            {
                DiscordId = ctx.Member.Id,
                Message = message,
                Stake = stake
            };

            var createBetResponse = await BetRepository.CreateBet(createBetRequest);

            if (!createBetResponse.Success)
            {
                await ctx.RespondAsync(createBetResponse.Message);
                return;
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine($"{ctx.Member.Mention} created new bet - **{createBetResponse.Bet.Id}** - \"{message}\"");
            response.AppendLine($"Stake for that bet is: {stake:N2}");
            response.AppendLine($"If you want to join - type: !bet {createBetResponse.Bet.Id} (Yes/No).");
            response.AppendLine($"For example: !bet 1 Yes");

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

            await ctx.RespondAsync($"Bet {id} finished! Check your rewards!");
        }

        [Command("stopbet")]
        [Description("Command to stop betting for a bet. Only author of bet can do that.")]
        public async Task StopCommand(CommandContext ctx, [Description("Bet id")] int id)
        {
            var stopBetRequest = new StopBetRequest
            {
                BetId = id,
                DiscordId = ctx.Member.Id
            };

            var finishBetResponse = await BetRepository.StopBet(stopBetRequest);

            if (!finishBetResponse.Success)
            {
                await ctx.RespondAsync(finishBetResponse.Message);
                return;
            }

            await ctx.RespondAsync($"Bet {id} is now not active for betting!");
        }
    }
}
