using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using PrzyjaznyBot.Common;
using System.Text;

namespace PrzyjaznyBot.Commands.TextCommands
{
    public class BetModule : BaseCommandModule
    {
        public const double HUNDRED = 100;

        private readonly BetService.BetService.BetServiceClient _betServiceClient;
        private readonly UserService.UserService.UserServiceClient _userServiceClient;

        public BetModule(BetService.BetService.BetServiceClient betServiceClient,
            UserService.UserService.UserServiceClient userServiceClient)
        {
            _betServiceClient = betServiceClient;
            _userServiceClient = userServiceClient;
        }

        [Command("bets")]
        [Description("Command to show existing bets.")]
        public async Task ShowAllBetsCommand(CommandContext ctx, [Description("Show finished bets - true. Default false.")] bool showNotActive = false)
        {
            var getBetsRequest = new BetService.GetBetsRequest
            {
                ShowNotActive = showNotActive
            };

            var getUsersResponse = await _betServiceClient.GetBetsAsync(getBetsRequest);

            if (!getUsersResponse.Success)
            {
                await ctx.RespondAsync(getUsersResponse.Message);
                return;
            }

            StringBuilder betsMessage = new();
            int position = 0;

            foreach (var bet in getUsersResponse.BetList.OrderByDescending(u => u.IsActive).ThenByDescending(u => u.DateTime).ToList())
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
            var getUserBetsRequest = new BetService.GetUserBetsRequest
            {
                BetId = id
            };

            var getUserBetsResponse = await _betServiceClient.GetUserBetsAsync(getUserBetsRequest);

            var builder = new DiscordMessageBuilder();
            builder.AddComponents(new DiscordComponent[]
            {
                new DiscordButtonComponent(ButtonStyle.Success, $"{ButtonCustomId.CreateYes}+{id}", "Yes"),
                new DiscordButtonComponent(ButtonStyle.Danger, $"{ButtonCustomId.CreateNo}+{id}", "No"),
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{ButtonCustomId.CreateInfo}+{id}", "Info"),
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{ButtonCustomId.CreateShowAllBets}+{id}", "All bets"),
            });

            if (!getUserBetsResponse.Success)
            {
                await ctx.RespondAsync(getUserBetsResponse.Message);
                return;
            }

            if (!getUserBetsResponse.UserBetList.Any())
            {
                builder.WithContent($"Bet id: {id} - **No bets yet!**".ToString());
                await builder.SendAsync(ctx.Channel);
                return;
            }

            var getBetRequest = new BetService.GetBetRequest
            {
                BetId = getUserBetsResponse.UserBetList.First().BetId
            };

            var getBetResponse = _betServiceClient.GetBet(getBetRequest);

            if (!getBetResponse.Success)
            {
                await ctx.RespondAsync(getBetResponse.Message);
                return;
            }

            double firstConditionPercentage = getUserBetsResponse.UserBetList.Where(ub => ub.Condition == BetService.Condition.Yes).Count() * HUNDRED / getUserBetsResponse.UserBetList.Count;

            StringBuilder betInfoMessage = new();
            int position = 0;

            betInfoMessage.AppendLine($"**Bet id: {id} - {getBetResponse.BetValue.Bet.Message}**");
            betInfoMessage.AppendLine($"Total stake: {getBetResponse.BetValue.Bet.Stake * getUserBetsResponse.UserBetList.Count}");
            betInfoMessage.AppendLine($"Yes: {firstConditionPercentage:N2}% - No: {HUNDRED - firstConditionPercentage:N2}%");

            foreach (var userBet in getUserBetsResponse.UserBetList.OrderByDescending(ub => ub.Condition == BetService.Condition.Yes))
            {
                var getUserRequest = new UserService.GetUserRequest
                {
                    UserId = userBet.UserId
                };

                var getUserResponse = _userServiceClient.GetUser(getUserRequest);

                var username = getUserResponse.Success ? getUserResponse.UserValue.User.Username : "Unknown user";

                position++;
                betInfoMessage.AppendLine($"{position}. {username} - {userBet.Condition}.");
            };

            builder.WithContent(betInfoMessage.ToString());

            await builder.SendAsync(ctx.Channel);
        }

        [Command("bet")]
        [Description("Command for joining the existing bet.")]
        public async Task BetCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition)
        {
            var formatedCondition = condition.First().ToString().ToUpper() + condition[1..].ToLower();
            var createUserBetRequest = new BetService.CreateUserBetRequest
            {
                BetId = id,
                Condition = formatedCondition switch
                {
                    "Yes" => BetService.Condition.Yes,
                    "No" => BetService.Condition.No,
                    _ => BetService.Condition.No
                },
                DiscordId = ctx.Member.Id
            };

            var createUserBetResponse = await _betServiceClient.CreateUserBetAsync(createUserBetRequest);

            if (!createUserBetResponse.Success)
            {
                await ctx.RespondAsync(createUserBetResponse.Message);
                return;
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine($"{ctx.Member.Mention} just bet on {formatedCondition} for bet with Id: {id}!");
            await ctx.RespondAsync(response.ToString());
        }

        [Command("betcreate")]
        [Description("Command for creating a bet. Answers for now are just Yes or No.")]
        public async Task CreateCommand(CommandContext ctx, [Description("Bet message")]string message, [Description("Bet stake")]double stake)
        {
            if (stake <= 0)
            {
                await ctx.RespondAsync("Stake has to be greater than 0");
                return;
            }

            var createBetRequest = new BetService.CreateBetRequest
            {
                DiscordId = ctx.Member.Id,
                Message = message,
                Stake = stake
            };

            var createBetResponse = await _betServiceClient.CreateBetAsync(createBetRequest);

            if (!createBetResponse.Success)
            {
                await ctx.RespondAsync(createBetResponse.Message);
                return;
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine($"{ctx.Member.Mention} created new bet - **{createBetResponse.BetValue.Bet.Id}** - \"{message}\"");
            response.AppendLine($"Stake for that bet is: {stake:N2}");
            response.AppendLine($"If you want to join - type: !bet {createBetResponse.BetValue.Bet.Id} (Yes/No).");

            var builder = new DiscordMessageBuilder().WithContent(response.ToString());
            builder.AddComponents(new DiscordComponent[]
            {
                new DiscordButtonComponent(ButtonStyle.Success, $"{ButtonCustomId.CreateYes}+{createBetResponse.BetValue.Bet.Id}", "Yes"),
                new DiscordButtonComponent(ButtonStyle.Danger, $"{ButtonCustomId.CreateNo}+{createBetResponse.BetValue.Bet.Id}", "No"),
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{ButtonCustomId.CreateInfo}+{createBetResponse.BetValue.Bet.Id}", "Info"),
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{ButtonCustomId.CreateShowAllBets}+{createBetResponse.BetValue.Bet.Id}", "All bets"),
            });

            await builder.SendAsync(ctx.Channel);
        }

        [Command("betfinish")]
        [Description("Command for finishing a bet. Result can be only Yes or No.")]
        public async Task FinishCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition)
        {
            var formatedCondition = condition.First().ToString().ToUpper() + condition.Substring(1).ToLower();
            var finishBetRequest = new BetService.FinishBetRequest
            {
                BetId = id,
                Condition = formatedCondition switch
                {
                    "Yes" => BetService.Condition.Yes,
                    "No" => BetService.Condition.No,
                    _ => BetService.Condition.No
                },
                DiscordId = ctx.Member.Id
            };

            var finishBetResponse = await _betServiceClient.FinishBetAsync(finishBetRequest);

            if (!finishBetResponse.Success)
            {
                await ctx.RespondAsync(finishBetResponse.Message);
                return;
            }

            await ctx.RespondAsync($"Bet {id} finished! Check your rewards!");
        }

        [Command("betstop")]
        [Description("Command to stop betting for a bet. Only author of bet can do that.")]
        public async Task StopCommand(CommandContext ctx, [Description("Bet id")] int id)
        {
            var stopBetRequest = new BetService.StopBetRequest
            {
                BetId = id,
                DiscordId = ctx.Member.Id
            };

            var finishBetResponse = await _betServiceClient.StopBetAsync(stopBetRequest);

            if (!finishBetResponse.Success)
            {
                await ctx.RespondAsync(finishBetResponse.Message);
                return;
            }

            await ctx.RespondAsync($"Bet {id} is now not active for betting!");
        }
    }
}
