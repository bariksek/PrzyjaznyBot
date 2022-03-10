using BetService.Builders;
using BetService.DAL;
using BetService.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BetService.Processors
{
    public class CreateUserBetProcessor : IProcessor<CreateUserBetRequest, CreateUserBetResponse>
    {
        private readonly UserService.UserService.UserServiceClient _userServiceClient;
        private readonly ICreateUserBetResponseBuilder _createUserBetResponseBuilder;
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;

        public CreateUserBetProcessor(UserService.UserService.UserServiceClient userServiceClient,
            ICreateUserBetResponseBuilder createUserBetResponseBuilder,
            IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            _userServiceClient = userServiceClient;
            _createUserBetResponseBuilder = createUserBetResponseBuilder;
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        public async Task<CreateUserBetResponse> Process(CreateUserBetRequest request, CancellationToken cancellationToken)
        {
            if (request.DiscordId <= 0 || request.BetId <= 0)
            {
                return _createUserBetResponseBuilder.Build(false, "DiscordId and BetId must be greater than 0", null);
            }

            var getUserResponse = await GetUser(request.DiscordId, cancellationToken);

            if (!getUserResponse.Success)
            {
                return _createUserBetResponseBuilder.Build(false, $"Cannot find user with DiscordId: {request.DiscordId}", null);
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bet = postgreSqlContext.Bets.FirstOrDefault(b => b.Id == request.BetId && b.IsActive && !b.IsStopped);

            if (bet == null)
            {
                return _createUserBetResponseBuilder.Build(false, $"Bet with Id: {request.BetId} not found, stopped or already finished", null);
            }

            if (postgreSqlContext.UserBets.Any(ub => ub.BetId == bet.Id && ub.UserId == getUserResponse.UserValue.User.Id))
            {
                return _createUserBetResponseBuilder.Build(false, $"User {getUserResponse.UserValue.User.Username} already made UserBet for BetId: {bet.Id}", null);
            }

            if (getUserResponse.UserValue.User.Points < bet.Stake)
            {
                return _createUserBetResponseBuilder.Build(false, "User doesn't have enough points", null);
            }

            var userBet = CreateNewUserBet(getUserResponse.UserValue.User.Id, bet.Id, request.Condition.Map());

            postgreSqlContext.UserBets.Add(userBet);

            var updateUserResponse = await SubstractPointsFromUser(getUserResponse.UserValue.User, bet.Stake, cancellationToken);

            if (!updateUserResponse.Success)
            {
                return _createUserBetResponseBuilder.Build(false, updateUserResponse.Message, null);
            }

            var result = await postgreSqlContext.SaveChangesAsync(cancellationToken);

            if(result == 0)
            {
                return _createUserBetResponseBuilder.Build(false, "Unknow error during user bet creation", null);
            }

            return _createUserBetResponseBuilder.Build(true, "UserBet created", userBet);
        }

        private async Task<UserService.UpdateUserResponse> SubstractPointsFromUser(UserService.User user, double points, CancellationToken cancellationToken)
        {
            var updatedUser = new UserService.User
            {
                DiscordUserId = user.DiscordUserId,
                Id = user.Id,
                LastDailyRewardClaimDateTime = user.LastDailyRewardClaimDateTime,
                Username = user.Username,
                Points = user.Points - points
            };

            var updateUserRequest = new UserService.UpdateUserRequest
            {
                DiscordUserId = user.DiscordUserId,
                User = updatedUser
            };

            return await _userServiceClient.UpdateUserAsync(updateUserRequest, cancellationToken: cancellationToken);
        }

        private static Model.UserBet CreateNewUserBet(int userId, int betId, Common.Condition condition)
        {
            return new Model.UserBet
            {
                UserId = userId,
                BetId = betId,
                Condition = condition
            };
        }

        private async Task<UserService.GetUserResponse> GetUser(ulong discordId, CancellationToken cancellationToken)
        {
            var getUserRequest = new UserService.GetUserRequest
            {
                DiscordUserId = discordId
            };

            return await _userServiceClient.GetUserAsync(getUserRequest, cancellationToken: cancellationToken);
        }
    }
}
