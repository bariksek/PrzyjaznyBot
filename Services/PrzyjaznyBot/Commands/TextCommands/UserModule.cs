using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Text;
using Google.Protobuf.WellKnownTypes;

namespace PrzyjaznyBot.Commands.TextCommands
{
    public class UserModule : BaseCommandModule
    {
        private readonly UserService.UserService.UserServiceClient _userServiceClient;
        private readonly double RewardPoints = 25.0;

        public UserModule(UserService.UserService.UserServiceClient userServiceClient)
        {
            _userServiceClient = userServiceClient;
        }

        [Command("elo")]
        [Description("Command used for simply saying hello to the bot ;)")]
        public static async Task EloCommand(CommandContext ctx)
        {
            await ctx.RespondAsync($"Elo {ctx.Member.Mention} mordo, jak tam zdróweczko?");
        }

        [Command("init")]
        [Description("Command used for creating a user in database.")]
        public async Task InitCommand(CommandContext ctx)
        {
            var getUserRequest = new UserService.GetUserRequest
            {
                DiscordUserId = ctx.Member.Id
            };

            var getUserResponse = _userServiceClient.GetUser(getUserRequest);

            if (getUserResponse.Success)
            {
                await ctx.RespondAsync($"User **{getUserResponse.UserValue.User.Username}** already exists.");
                return;
            }

            var createUserRequest = new UserService.CreateUserRequest
            {
                DiscordUserId = ctx.Member.Id,
                Username = ctx.Member.Username
            };

            var createUserResponse = await _userServiceClient.CreateUserAsync(createUserRequest);

            if (!createUserResponse.Success)
            {
                await ctx.RespondAsync(createUserResponse.Message);
                return;
            }

            await ctx.RespondAsync($"New user **{createUserResponse.UserValue.User.Username}** has been successfully created.");
        }

        [Command("transfer")]
        [Description("Command for transfering money to another user.")]
        public async Task TransferCommand(CommandContext ctx, [Description("Tagged discord member - @user")]DiscordMember targetMember, [Description("Points value")] double value)
        {
            var getUsersRequest = new UserService.GetUsersRequest
            {
                DiscordUserIds = { new List<ulong> { ctx.Member.Id, targetMember.Id } }
            };

            var getUsersResponse = await _userServiceClient.GetUsersAsync(getUsersRequest);

            if(!getUsersResponse.Success)
            {
                await ctx.RespondAsync(getUsersResponse.Message);
                return;
            }

            var senderUser = getUsersResponse.UserList.FirstOrDefault(u => u.DiscordUserId == ctx.Member.Id);
            if (senderUser is null)
            {
                await ctx.RespondAsync($"Cannot find sender user with Id: {ctx.Member.Id}");
                return;
            }

            var receiverUser = getUsersResponse.UserList.FirstOrDefault(u => u.DiscordUserId == targetMember.Id);
            if (receiverUser is null)
            {
                await ctx.RespondAsync($"Cannot find receiver user with Id: {targetMember.Id}");
                return;
            }

            if (senderUser.Points < value)
            {
                await ctx.RespondAsync($"{senderUser.Username} doesn't have enough points to transfer");
                return;
            }

            var updateUserRequests = new List<UserService.UpdateUserRequest>
            {
                new UserService.UpdateUserRequest
                {
                    DiscordUserId = senderUser.DiscordUserId,
                    User = new UserService.User
                    {
                        DiscordUserId = senderUser.DiscordUserId,
                        Id = senderUser.Id,
                        LastDailyRewardClaimDateTime = senderUser.LastDailyRewardClaimDateTime,
                        Username = senderUser.Username,
                        Points = senderUser.Points - value
                    }
                },
                new UserService.UpdateUserRequest
                {
                    DiscordUserId = receiverUser.DiscordUserId,
                    User = new UserService.User
                    {
                        DiscordUserId = receiverUser.DiscordUserId,
                        Id = receiverUser.Id,
                        LastDailyRewardClaimDateTime = receiverUser.LastDailyRewardClaimDateTime,
                        Username = receiverUser.Username,
                        Points = receiverUser.Points + value
                    }
                }
            };

            var updateUserTasks = updateUserRequests.Select(rq => _userServiceClient.UpdateUserAsync(rq).ResponseAsync).ToList();

            var updateUserResponses = await Task.WhenAll(updateUserTasks);

            if(updateUserResponses.Any(rs => !rs.Success))
            {
                StringBuilder errorMessage = new();
                errorMessage.AppendLine("There was an error during user update. Error messages:");
                foreach(var failedResponse in updateUserResponses.Where(rs => !rs.Success))
                {
                    errorMessage.AppendLine(failedResponse.Message);
                }
                await ctx.RespondAsync(errorMessage.ToString());
                return;
            }

            await ctx.RespondAsync($"{ctx.Member.Mention} successfully sent {value:N2} points to {targetMember.Mention}.");
        }

        [Command("stats")]
        [Description("Command for showing statistics about points and users.")]
        public async Task StatsCommand(CommandContext ctx)
        {
            var getUsersRequest = new UserService.GetUsersRequest();
            var getUsersResponse = _userServiceClient.GetUsers(getUsersRequest);

            if (!getUsersResponse.Success)
            {
                await ctx.RespondAsync(getUsersResponse.Message);
                return;
            }

            StringBuilder statsMessage = new StringBuilder();
            int position = 0;

            foreach (var user in getUsersResponse.UserList.OrderByDescending(u => u.Points))
            {
                position++;
                statsMessage.AppendLine($"{position}. {user.Username} - {user.Points:N2} pkt.");
            };

            await ctx.RespondAsync(statsMessage.ToString());
        }

        [Command("daily")]
        [Description("Command for gaining daily points as reward.")]
        public async Task DailyCommand(CommandContext ctx)
        {
            var getUserRequest = new UserService.GetUserRequest
            {
                DiscordUserId = ctx.Member.Id
            };

            var getUserResponse = _userServiceClient.GetUser(getUserRequest);

            if (!getUserResponse.Success)
            {
                await ctx.RespondAsync($"User **{getUserResponse.UserValue.User.Username}** does not exist. Use !init first.");
                return;
            }

            var lastDailyRewardClaimDateTime = DateTime.SpecifyKind(getUserResponse.UserValue.User.LastDailyRewardClaimDateTime.ToDateTime(), DateTimeKind.Utc);
            var now = DateTime.UtcNow;

            if (now.Date <= lastDailyRewardClaimDateTime.Date)
            {
                var nextDay = now.AddDays(1);
                var timespanToNextDay = nextDay.Date - now;

                await ctx.RespondAsync($"You have already used this command today. Remaining time: **{timespanToNextDay.Hours}**:**{timespanToNextDay.Minutes:D2}**.");
                return;
            }

            var updateUserRequest = new UserService.UpdateUserRequest
            {
                DiscordUserId = ctx.Member.Id,
                User = new UserService.User
                {
                    DiscordUserId = getUserResponse.UserValue.User.DiscordUserId,
                    Id = getUserResponse.UserValue.User.Id,
                    LastDailyRewardClaimDateTime = now.ToTimestamp(),
                    Username = getUserResponse.UserValue.User.Username,
                    Points = getUserResponse.UserValue.User.Points + RewardPoints
                }
            };

            var updateUserResponse = await _userServiceClient.UpdateUserAsync(updateUserRequest);

            if (!updateUserResponse.Success)
            {
                await ctx.RespondAsync(updateUserResponse.Message);
                return;
            }

            await ctx.RespondAsync($"**{RewardPoints:N2}** points have been successfully added to **{getUserResponse.UserValue.User.Username}** wallet.");
        }
    }
}