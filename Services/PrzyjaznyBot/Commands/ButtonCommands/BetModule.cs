using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Text;

namespace PrzyjaznyBot.Commands.ButtonCommands
{
    public class BetModule
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

        public async Task BetCommand(ComponentInteractionCreateEventArgs e, string id, BetService.Condition condition)
        {
            var createUserBetRequest = new BetService.CreateUserBetRequest
            {
                BetId = int.Parse(id),
                Condition = condition,
                DiscordId = e.Interaction.User.Id
            };

            var createUserBetResponse = await _betServiceClient.CreateUserBetAsync(createUserBetRequest);

            var builder = new DiscordInteractionResponseBuilder();
            builder.AsEphemeral(true);

            if (!createUserBetResponse.Success)
            {
                builder.WithContent(createUserBetResponse.Message);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            StringBuilder response = new StringBuilder();
            response.AppendLine($"{e.Interaction.User.Mention} just bet on {condition} for bet with Id: {id}!");

            builder.WithContent(response.ToString());
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }

        public async Task BetInfoCommand(ComponentInteractionCreateEventArgs e, string id)
        {
            var betId = int.Parse(id);

            var getBetInfoRequest = new BetService.GetUserBetsRequest
            {
                BetId = betId
            };

            var getBetInfoResponse = await _betServiceClient.GetUserBetsAsync(getBetInfoRequest);

            var builder = new DiscordInteractionResponseBuilder();
            builder.AsEphemeral(true);

            if (!getBetInfoResponse.Success)
            {
                builder.WithContent(getBetInfoResponse.Message);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            if (!getBetInfoResponse.UserBetList.Any())
            {
                builder.WithContent($"Bet id: {id} - **No bets yet!**");
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            var getBetRequest = new BetService.GetBetRequest
            {
                BetId = betId
            };

            var getBetResponse = await _betServiceClient.GetBetAsync(getBetRequest);

            if (!getBetResponse.Success)
            {
                builder.WithContent(getBetResponse.Message);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            double firstConditionPercentage = getBetInfoResponse.UserBetList.Where(ub => ub.Condition == BetService.Condition.Yes).Count() * HUNDRED / getBetInfoResponse.UserBetList.Count();

            StringBuilder betInfoMessage = new();
            int position = 0;

            betInfoMessage.AppendLine($"**Bet id: {id} - {getBetResponse.BetValue.Bet.Message}**");
            betInfoMessage.AppendLine($"Total stake: {getBetResponse.BetValue.Bet.Stake * getBetInfoResponse.UserBetList.Count()}");
            betInfoMessage.AppendLine($"Yes: {firstConditionPercentage:N2}% - No: {HUNDRED - firstConditionPercentage:N2}%");

            foreach (var userBet in getBetInfoResponse.UserBetList.OrderByDescending(ub => ub.Condition == BetService.Condition.Yes))
            {
                var getUserRequest = new UserService.GetUserRequest
                {
                    UserId = userBet.UserId
                };

                var getUserResponse = await _userServiceClient.GetUserAsync(getUserRequest);

                position++;

                if (!getUserResponse.Success)
                {
                    betInfoMessage.AppendLine($"{position}. Unable to fetch user with id: {userBet.UserId}.");
                    continue;
                }

                betInfoMessage.AppendLine($"{position}. {getUserResponse.UserValue.User.Username} - {userBet.Condition}.");
            };

            builder.WithContent(betInfoMessage.ToString());
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }

        public async Task ShowAllBetsCommand(ComponentInteractionCreateEventArgs e)
        {
            var getBetsRequest = new BetService.GetBetsRequest
            {
                ShowNotActive = false
            };

            var getUsersResponse = await _betServiceClient.GetBetsAsync(getBetsRequest);
            var builder = new DiscordInteractionResponseBuilder();
            builder.AsEphemeral(true);

            if (!getUsersResponse.Success)
            {
                builder.WithContent(getUsersResponse.Message);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            StringBuilder betsMessage = new StringBuilder();
            int position = 0;

            foreach (var bet in getUsersResponse.BetList.OrderByDescending(u => u.IsActive).ThenByDescending(u => u.DateTime))
            {
                position++;
                betsMessage.AppendLine($"{position}. BetId: {bet.Id} - {bet.Message} - Stopped: {bet.IsStopped} - Active: {bet.IsActive} - Stake: {bet.Stake:N2}");
            };

            builder.WithContent(betsMessage.ToString());
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
    }
}
