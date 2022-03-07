using BetService.Builders;
using BetService.DAL;
using Microsoft.EntityFrameworkCore;

namespace BetService.Processors
{
    public class CreateBetProcessor : ICreateBetProcessor
    {
        private readonly UserService.UserService.UserServiceClient _userServiceClient;
        private readonly ICreateBetResponseBuilder _createBetResponseBuilder;
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContext;

        public CreateBetProcessor(UserService.UserService.UserServiceClient userServiceClient,
            ICreateBetResponseBuilder createBetResponseBuilder,
            IDbContextFactory<PostgreSqlContext> postgreSqlContext)
        {
            _userServiceClient = userServiceClient;
            _createBetResponseBuilder = createBetResponseBuilder;
            _postgreSqlContext = postgreSqlContext;
        }

        public async Task<CreateBetResponse> CreateBet(CreateBetRequest request, CancellationToken cancellationToken)
        {
            if (request.DiscordId <= 0)
            {
                return _createBetResponseBuilder.Build(false, "DiscordId must be greater than 0", null);
            }

            var getUserResponse = await GetUser(request.DiscordId, cancellationToken);

            if(!getUserResponse.Success)
            {
                return _createBetResponseBuilder.Build(false, $"Cannot find user with DiscordId: {request.DiscordId}", null);
            }

            var bet = CreateNewBet(getUserResponse.UserValue.User.Id, request.Message, request.Stake);

            using var postgreSqlContext = _postgreSqlContext.CreateDbContext();
            postgreSqlContext.Bets.Add(bet);
            var result = await postgreSqlContext.SaveChangesAsync(cancellationToken);

            if(result == 0)
            {
                return _createBetResponseBuilder.Build(false, "Unknow error during bet creation", null);
            }

            return _createBetResponseBuilder.Build(true, "Bet created", bet);
        }

        private async Task<UserService.GetUserResponse> GetUser(ulong discordId, CancellationToken cancellationToken)
        {
            var getUserRequest = new UserService.GetUserRequest
            {
                DiscordUserId = discordId
            };

            return await _userServiceClient.GetUserAsync(getUserRequest, cancellationToken: cancellationToken);
        }

        private static Model.Bet CreateNewBet(int authorId, string message, double stake)
        {
            return new Model.Bet
            {
                AuthorId = authorId,
                IsStopped = false,
                IsActive = true,
                Message = message,
                DateTime = DateTime.UtcNow,
                Stake = stake
            };
        }
    }
}
