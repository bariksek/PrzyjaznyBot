using BetService.Builders;
using BetService.DAL;
using BetService.Mappers;
using Microsoft.EntityFrameworkCore;

namespace BetService.Processors
{
    public class FinishBetProcessor : IProcessor<FinishBetRequest, FinishBetResponse>
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IFinishBetResponseBuilder _finishBetResponseBuilder;
        private readonly UserService.UserService.UserServiceClient _userServiceClient;

        public FinishBetProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory, 
            IFinishBetResponseBuilder finishBetResponseBuilder,
            UserService.UserService.UserServiceClient userServiceClient)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _finishBetResponseBuilder = finishBetResponseBuilder;
            _userServiceClient = userServiceClient;
        }

        public async Task<FinishBetResponse> Process(FinishBetRequest request, CancellationToken cancellationToken)
        {
            if (request.BetId <= 0 || request.DiscordId <= 0)
            {
                return _finishBetResponseBuilder.Build(false, "BetId and DiscordId must be greater than 0");
            }

            var getUserResponse = await GetUser(request.DiscordId, cancellationToken);

            if (!getUserResponse.Success)
            {
                return _finishBetResponseBuilder.Build(false, getUserResponse.Message);
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bet = postgreSqlContext.Bets.FirstOrDefault(b => b.Id == request.BetId && b.IsActive == true);

            if (bet == null)
            {
                return _finishBetResponseBuilder.Build(false, $"Bet with Id: {request.BetId} not found or already finished");
            }

            if (bet.AuthorId != getUserResponse.UserValue.User.Id)
            {
                return _finishBetResponseBuilder.Build(false, $"User {getUserResponse.UserValue.User.Username} is not an author of bet with Id: {request.BetId}");
            }

            bet.IsActive = false;

            var userBets = postgreSqlContext.UserBets.Where(ub => ub.BetId == bet.Id);
            var updateUsersPointsResponses = await UpdateUsersPoints(bet, userBets, request.Condition.Map(), cancellationToken);

            if (updateUsersPointsResponses.Any(r => !r.Success))
            {
                var aggregatedFailedResponsesMessages = updateUsersPointsResponses.Where(r => !r.Success).Select(r => r.Message).Aggregate((a, b) => $"{a};{b}");
                return _finishBetResponseBuilder.Build(false, aggregatedFailedResponsesMessages);
            }

            var result = await postgreSqlContext.SaveChangesAsync(cancellationToken);

            if (result == 0)
            {
                return _finishBetResponseBuilder.Build(false, "Unknow error during bet finishing");
            }

            return _finishBetResponseBuilder.Build(true, "Bet finished");
        }

        private async Task<IEnumerable<UserService.UpdateUserResponse>> UpdateUsersPoints(Model.Bet bet, 
            IEnumerable<Model.UserBet> userBets, 
            Common.Condition condition, 
            CancellationToken cancellationToken)
        {
            var roundPrecision = 2;
            var successUserBets = userBets.Where(v => v.Condition == condition).ToList();

            if (!successUserBets.Any())
            {
                return new List<UserService.UpdateUserResponse>();
            }

            var failUserBets = userBets.Where(v => v.Condition != condition).ToList();
            var prizePool = failUserBets.Count * bet.Stake;
            var prizePerUser = Math.Round(prizePool / successUserBets.Count, roundPrecision);

            var getUsersResponse = await GetUsers(successUserBets.Select(ub => ub.UserId), cancellationToken);

            var userUpdateTasks = getUsersResponse.UserList.Select(u => UpdateUser(u, prizePerUser + bet.Stake, cancellationToken)).ToList();

            return await Task.WhenAll(userUpdateTasks);
        }

        private async Task<UserService.UpdateUserResponse> UpdateUser(UserService.User user, double prize, CancellationToken cancellationToken)
        {
            var updatedUser = new UserService.User
            {
                DiscordUserId = user.DiscordUserId,
                Id = user.Id,
                LastDailyRewardClaimDateTime = user.LastDailyRewardClaimDateTime,
                Username = user.Username,
                Points = user.Points + prize
            };

            var updateUserRequest = new UserService.UpdateUserRequest
            {
                DiscordUserId = user.DiscordUserId,
                User = updatedUser
            };

            return await _userServiceClient.UpdateUserAsync(updateUserRequest, cancellationToken: cancellationToken);
        }

        private async Task<UserService.GetUsersResponse> GetUsers(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            var getUsersRequest = new UserService.GetUsersRequest
            {
                UserIds = { ids.ToList() }
            };

            return await _userServiceClient.GetUsersAsync(getUsersRequest, cancellationToken: cancellationToken);
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
