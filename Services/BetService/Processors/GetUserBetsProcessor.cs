using BetService.Builders;
using BetService.DAL;
using Microsoft.EntityFrameworkCore;

namespace BetService.Processors
{
    public class GetUserBetsProcessor : IGetUserBetsProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IGetUserBetsResponseBuilder _getUserBetsResponseBuilder;

        public GetUserBetsProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IGetUserBetsResponseBuilder getUserBetsResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _getUserBetsResponseBuilder = getUserBetsResponseBuilder;
        }

        public Task<GetUserBetsResponse> GetUserBets(GetUserBetsRequest request, CancellationToken cancellationToken)
        {
            if (request.BetId <= 0)
            {
                return Task.FromResult(_getUserBetsResponseBuilder.Build(false, "BetId must be greater than 0", new List<Model.UserBet>()));
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var userBets = postgreSqlContext.UserBets.Where(b => b.BetId == request.BetId).ToList();

            return Task.FromResult(_getUserBetsResponseBuilder.Build(true, $"Found {userBets.Count}", userBets));
        }
    }
}
