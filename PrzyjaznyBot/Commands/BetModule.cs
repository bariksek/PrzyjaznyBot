using Castle.Core.Internal;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.DTO.BetRepository;
using PrzyjaznyBot.DTO.UserRepository;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class BetModule : BaseCommandModule
    {
        public const double HUNDRED = 100;
        
        private readonly BetRepository BetRepository;
        private readonly UserRepository UserRepository;

        public BetModule()
        {
            BetRepository = new BetRepository();
            UserRepository = new UserRepository();
        }

        [Command("bets")]
        [Description("Command to show existing bets.")]
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

        [Command("betinfo")]
        [Description("Command to show information about bet.")]
        public async Task BetInfoCommand(CommandContext ctx, [Description("Bet id")] int id)
        {
            var getBetInfoRequest = new GetBetInfoRequest
            {
                BetId = id
            };

            GetBetInfoResponse getBetInfoResponse = BetRepository.GetUserBets(getBetInfoRequest);

            if (!getBetInfoResponse.Success)
            {
                await ctx.RespondAsync(getBetInfoResponse.Message);
                return;
            }

            if (getBetInfoResponse.UserBets.IsNullOrEmpty())
            {
                await ctx.RespondAsync($"Bet id: {id} - **No bets yet!**");
                return;
            }

            var getBetRequest = new GetBetRequest
            {
                BetId = getBetInfoResponse.UserBets.First().BetId
            };

            var getBetResponse = BetRepository.GetBet(getBetRequest);

            if (!getBetResponse.Success)
            {
                await ctx.RespondAsync(getBetResponse.Message);
                return;
            }

            double firstConditionPercentage = getBetInfoResponse.UserBets.Where(ub => ub.Condition == Condition.Yes).Count() * HUNDRED / getBetInfoResponse.UserBets.Count();

            StringBuilder betInfoMessage = new StringBuilder();
            int position = 0;

            betInfoMessage.AppendLine($"**Bet id: {id} - {getBetResponse.Bet.Message}**");
            betInfoMessage.AppendLine($"Yes: {firstConditionPercentage:N2}% - No: {(HUNDRED - firstConditionPercentage):N2}%");

            foreach (var userBet in getBetInfoResponse.UserBets.OrderByDescending(ub => ub.Condition == Condition.Yes))
            {
                var getUserRequest = new GetUserRequest
                {
                    UserId = userBet.UserId
                };

                var getUserResponse = UserRepository.GetUser(getUserRequest);

                position++;
                betInfoMessage.AppendLine($"{position}. {getUserResponse.User.Username} - {userBet.Condition}.");
            };

            await ctx.RespondAsync(betInfoMessage.ToString());
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
