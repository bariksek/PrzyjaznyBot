using Castle.Core.Internal;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.DTO.BetRepository;
using PrzyjaznyBot.DTO.UserRepository;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands.ButtonCommands
{
    public class BetModule
    {
        public const double HUNDRED = 100;

        public IBetRepository BetRepository { get; }
        public IUserRepository UserRepository { get; }

        public BetModule(IBetRepository betRepository, IUserRepository userRepository)
        {
            BetRepository = betRepository;
            UserRepository = userRepository;
        }

        public async Task BetCommand(ComponentInteractionCreateEventArgs e, string id, Condition condition)
        {
            var createUserBetRequest = new CreateUserBetRequest
            {
                BetId = Int32.Parse(id),
                Condition = condition.ToString(),
                DiscordId = e.Interaction.User.Id
            };

            var createUserBetResponse = await BetRepository.CreateUserBet(createUserBetRequest);

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
            var getBetInfoRequest = new GetBetInfoRequest
            {
                BetId = Int32.Parse(id)
            };

            GetBetInfoResponse getBetInfoResponse = BetRepository.GetUserBets(getBetInfoRequest);

            var builder = new DiscordInteractionResponseBuilder();
            builder.AsEphemeral(true);

            if (!getBetInfoResponse.Success)
            {
                builder.WithContent(getBetInfoResponse.Message);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            if (getBetInfoResponse.UserBets.IsNullOrEmpty())
            {
                builder.WithContent($"Bet id: {id} - **No bets yet!**");
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            var getBetRequest = new GetBetRequest
            {
                BetId = getBetInfoResponse.UserBets.First().BetId
            };

            var getBetResponse = BetRepository.GetBet(getBetRequest);

            if (!getBetResponse.Success)
            {
                builder.WithContent(getBetResponse.Message);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            double firstConditionPercentage = getBetInfoResponse.UserBets.Where(ub => ub.Condition == Condition.Yes).Count() * HUNDRED / getBetInfoResponse.UserBets.Count();

            StringBuilder betInfoMessage = new StringBuilder();
            int position = 0;

            betInfoMessage.AppendLine($"**Bet id: {id} - {getBetResponse.Bet.Message}**");
            betInfoMessage.AppendLine($"Total stake: {getBetResponse.Bet.Stake * getBetInfoResponse.UserBets.Count()}");
            betInfoMessage.AppendLine($"Yes: {firstConditionPercentage:N2}% - No: {HUNDRED - firstConditionPercentage:N2}%");

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

            builder.WithContent(betInfoMessage.ToString());
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }

        public async Task ShowAllBetsCommand(ComponentInteractionCreateEventArgs e)
        {
            var getBetsRequest = new GetBetsRequest
            {
                ShowNotActive = false
            };

            var getUsersResponse = await BetRepository.GetBets(getBetsRequest);
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

            foreach (var bet in getUsersResponse.Bets.OrderByDescending(u => u.IsActive).ThenByDescending(u => u.DateTime))
            {
                position++;
                betsMessage.AppendLine($"{position}. BetId: {bet.Id} - {bet.Message} - Stopped: {bet.IsStopped} - Active: {bet.IsActive} - Stake: {bet.Stake:N2}");
            };

            builder.WithContent(betsMessage.ToString());
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
    }
}
