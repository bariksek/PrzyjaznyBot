using BetService.Builders;
using BetService.DAL;
using Microsoft.EntityFrameworkCore;

namespace BetService.Processors
{
    public class GetBetProcessor : Processor<GetBetRequest, GetBetResponse>
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IGetBetResponseBuilder _getBetResponseBuilder;

        public GetBetProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IGetBetResponseBuilder getBetResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _getBetResponseBuilder = getBetResponseBuilder;
        }

        protected override Task<GetBetResponse> HandleRequest(GetBetRequest request, CancellationToken cancellationToken)
        {
            if (request.BetId <= 0)
            {
                return Task.FromResult(_getBetResponseBuilder.Build(false, "BetId must be greater than 0", null));
            }
            
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bet = postgreSqlContext.Bets.FirstOrDefault(b => b.Id == request.BetId);

            if (bet == null)
            {
                return Task.FromResult(_getBetResponseBuilder.Build(false, "Bet not found", null));
            }

            return Task.FromResult(_getBetResponseBuilder.Build(true, "Bet found", bet));
        }

        protected override Task<GetBetResponse> HandleException(Exception ex)
        {
            return Task.FromResult(_getBetResponseBuilder.Build(false, "Exception occured during processing", null));
        }
    }
}
