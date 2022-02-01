﻿using Castle.Core.Internal;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.DTO.BetRepository;
using PrzyjaznyBot.DTO.UserRepository;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands.TextCommands
{
    public class BetModule : BaseCommandModule
    {
        public const double HUNDRED = 100;

        private readonly IBetRepository BetRepository;
        private readonly IUserRepository UserRepository;

        public BetModule(IBetRepository betRepository, IUserRepository userRepository)
        {
            BetRepository = betRepository;
            UserRepository = userRepository;
        }

        [Command("bets")]
        [Description("Command to show existing bets.")]
        public async Task ShowAllBetsCommand(CommandContext ctx, [Description("Show finished bets - true. Default false.")] bool showNotActive = false)
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

            var builder = new DiscordMessageBuilder();
            builder.AddComponents(new DiscordComponent[]
            {
                new DiscordButtonComponent(ButtonStyle.Success, $"{ButtonCustomId.CreateYes}+{id}", "Yes"),
                new DiscordButtonComponent(ButtonStyle.Danger, $"{ButtonCustomId.CreateNo}+{id}", "No"),
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{ButtonCustomId.CreateInfo}+{id}", "Info"),
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{ButtonCustomId.CreateShowAllBets}+{id}", "All bets"),
            });

            if (!getBetInfoResponse.Success)
            {
                await ctx.RespondAsync(getBetInfoResponse.Message);
                return;
            }

            if (getBetInfoResponse.UserBets.IsNullOrEmpty())
            {
                builder.WithContent($"Bet id: {id} - **No bets yet!**".ToString());
                await builder.SendAsync(ctx.Channel);
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

            await builder.SendAsync(ctx.Channel);
        }

        [Command("bet")]
        [Description("Command for joining the existing bet.")]
        public async Task BetCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition)
        {
            var formatedCondition = condition.First().ToString().ToUpper() + condition.Substring(1).ToLower();
            var createUserBetRequest = new CreateUserBetRequest
            {
                BetId = id,
                Condition = formatedCondition,
                DiscordId = ctx.Member.Id
            };

            var createUserBetResponse = await BetRepository.CreateUserBet(createUserBetRequest);

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

            var builder = new DiscordMessageBuilder().WithContent(response.ToString());
            builder.AddComponents(new DiscordComponent[]
            {
                new DiscordButtonComponent(ButtonStyle.Success, $"{ButtonCustomId.CreateYes}+{createBetResponse.Bet.Id}", "Yes"),
                new DiscordButtonComponent(ButtonStyle.Danger, $"{ButtonCustomId.CreateNo}+{createBetResponse.Bet.Id}", "No"),
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{ButtonCustomId.CreateInfo}+{createBetResponse.Bet.Id}", "Info"),
                new DiscordButtonComponent(ButtonStyle.Secondary, $"{ButtonCustomId.CreateShowAllBets}+{createBetResponse.Bet.Id}", "All bets"),
            });

            await builder.SendAsync(ctx.Channel);
        }

        [Command("betfinish")]
        [Description("Command for finishing a bet. Result can be only Yes or No.")]
        public async Task FinishCommand(CommandContext ctx, [Description("Bet id")] int id, [Description("Yes or No")]string condition)
        {
            var formatedCondition = condition.First().ToString().ToUpper() + condition.Substring(1).ToLower();
            var finishBetRequest = new FinishBetRequest
            {
                BetId = id,
                Condition = formatedCondition,
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

        [Command("betstop")]
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