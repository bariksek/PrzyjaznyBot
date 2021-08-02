﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.DTO.UserRepository;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class UserModule : BaseCommandModule
    {
        private readonly UserRepository UserRepository;
        private readonly double InitialPoints = 100.0;
        private readonly double RewardPoints = 25.0;

        public UserModule()
        {
            UserRepository = new UserRepository();
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
            var getUserRequest = new GetUserRequest
            {
                DiscordId = ctx.Member.Id
            };

            var getUserResponse = UserRepository.GetUser(getUserRequest);

            if (getUserResponse.Success)
            {
                await ctx.RespondAsync($"User **{getUserResponse.User.Username}** already exists.");
                return;
            }

            var createUserRequest = new CreateUserRequest
            {
                DiscordId = ctx.Member.Id,
                Username = ctx.Member.Username,
                Points = InitialPoints,
                DateTime = System.DateTime.Now
            };

            var createUserResponse =  await UserRepository.CreateNewUser(createUserRequest);

            if(!createUserResponse.Success)
            {
                await ctx.RespondAsync(createUserResponse.Message);
                return;
            }

            await ctx.RespondAsync($"New user **{createUserResponse.CreatedUser.Username}** has been successfully created.");
        }

        [Command("transfer")]
        [Description("Command for transfering money to another user.")]
        public async Task TransferCommand(CommandContext ctx, [Description("Tagged discord member - @user")]DiscordMember targetMember, [Description("Points value")] double value)
        {
            var transferPointsRequest = new TransferPointsRequest
            {
                SenderDiscordId = ctx.Member.Id,
                ReceiverDiscordId = targetMember.Id,
                Value = value
            };

            var transferPointsResponse = await UserRepository.TransferPoints(transferPointsRequest);

            if (!transferPointsResponse.Success)
            {
                await ctx.RespondAsync(transferPointsResponse.Message);
                return;
            }

            await ctx.RespondAsync($"{ctx.Member.Mention} successfully sent {value} points to {targetMember.Mention}.");
        }

        [Command("stats")]
        [Description("Command for showing statistics about points and users.")]
        public async Task StatsCommand(CommandContext ctx)
        {
            var getUsersRequest = new GetUsersRequest();
            var getUsersResponse = UserRepository.GetUsers(getUsersRequest);

            if (!getUsersResponse.Success)
            {
                await ctx.RespondAsync(getUsersResponse.Message);
                return;
            }

            StringBuilder statsMessage = new StringBuilder();
            int position = 0;

            foreach (var user in getUsersResponse.Users.OrderByDescending(u => u.Points))
            {
                position++;
                statsMessage.AppendLine($"{position}. {user.Username} - {user.Points} pkt.");
            };

            await ctx.RespondAsync(statsMessage.ToString());
        }

        [Command("reward")]
        [Description("Command for showing statistics about points and users.")]
        public async Task RewardCommand(CommandContext ctx)
        {
            var getUserRequest = new GetUserRequest
            {
                DiscordId = ctx.Member.Id
            };

            var getUserResponse = UserRepository.GetUser(getUserRequest);

            if (!getUserResponse.Success)
            {
                await ctx.RespondAsync($"User **{getUserResponse.User.Username}** does not exist. Use !init first.");
                return;
            }

            var timespan =  System.DateTime.Now - getUserResponse.User.DateTime;
            var totalDays = timespan.TotalDays;

            if(totalDays <= 1)
            {   
                var readableHours = System.Math.Floor(24 - timespan.TotalHours);
                var readableMinutes = System.Math.Floor(60 - timespan.TotalMinutes);

                await ctx.RespondAsync($"You have already used this command today. Remaining time: **{readableHours}**:**{readableMinutes}**.");
                return;
            }

            var addPointsRequest = new AddPointsRequest
            {
                DiscordId = ctx.Member.Id,
                Value = RewardPoints
            };

            var addPointsResponse = await UserRepository.AddPoints(addPointsRequest);

            if(!addPointsResponse.Success)
            {
                await ctx.RespondAsync(addPointsResponse.Message);
                return;
            }

            await ctx.RespondAsync($"**{RewardPoints}** points have been successfully added to **{getUserResponse.User.Username}** wallet.");
        }
    }
}