using BetService.Builders;
using BetService.DAL;
using BetService.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BetService.Processors
{
    public class CreateUserBetProcessor : ICreateUserBetProcessor
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

        public async Task<CreateUserBetResponse> CreateUserBet(CreateUserBetRequest request, CancellationToken cancellationToken)
        {
            if (request.DiscordId <= 0 || request.BetId <= 0)
            {
                return _createUserBetResponseBuilder.Build(false, "DiscordId and BetId must be greater than 0", null);
            }

            var getUserRequest = new UserService.GetUserRequest
            {
                DiscordUserId = request.DiscordId
            };

            var getUserResponse = await _userServiceClient.GetUserAsync(getUserRequest, cancellationToken: cancellationToken);

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

            if (getUserResponse.UserValue.User.Points < bet.Stake)
            {
                return _createUserBetResponseBuilder.Build(false, "User doesn't have enough points", null);
            }

            var userBet = new Model.UserBet
            {
                UserId = getUserResponse.UserValue.User.Id,
                BetId = bet.Id,
                Condition = request.Condition.Map()
            };

            postgreSqlContext.UserBets.Add(userBet);

            var updatedUser = new UserService.User
            {
                DiscordUserId = getUserResponse.UserValue.User.DiscordUserId,
                Id = getUserResponse.UserValue.User.Id,
                LastDailyRewardClaimDateTime = getUserResponse.UserValue.User.LastDailyRewardClaimDateTime,
                Username = getUserResponse.UserValue.User.Username,
                Points = getUserResponse.UserValue.User.Points - bet.Stake
            };

            var updateUserRequest = new UserService.UpdateUserRequest
            {
                DiscordUserId = request.DiscordId,
                User = updatedUser
            };

            var updateUserResponse = await _userServiceClient.UpdateUserAsync(updateUserRequest, cancellationToken: cancellationToken);

            if (!updateUserResponse.Success)
            {
                return _createUserBetResponseBuilder.Build(false, updateUserResponse.Message, null);
            }

            var result = await postgreSqlContext.SaveChangesAsync(cancellationToken: cancellationToken);

            if(result == 0)
            {
                return _createUserBetResponseBuilder.Build(false, "Unknow error during user bet creation", null);
            }

            return _createUserBetResponseBuilder.Build(true, "UserBet created", userBet);
        }
    }
}
