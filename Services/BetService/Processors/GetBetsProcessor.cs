using BetService.Builders;
using BetService.DAL;
using Microsoft.EntityFrameworkCore;

namespace BetService.Processors
{
    public class GetBetsProcessor : IGetBetsProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IGetBetsResponseBuilder _getBetsResponseBuilder;

        public GetBetsProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IGetBetsResponseBuilder getBetsResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _getBetsResponseBuilder = getBetsResponseBuilder;
        }

        public Task<GetBetsResponse> GetBets(GetBetsRequest request, CancellationToken cancellationToken)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bets = request.ShowNotActive ? postgreSqlContext.Bets.ToList() : postgreSqlContext.Bets.Where(b => b.IsActive).ToList();

            return Task.FromResult(_getBetsResponseBuilder.Build(true, $"Found {bets.Count} bets", bets));
        }
    }
}
