using BetService.Mappers;

namespace BetService.Builders
{
    public class GetBetsResponseBuilder : ResponseBuilderBase, IGetBetsResponseBuilder
    {
        public GetBetsResponse Build(bool success, string message, IEnumerable<Model.Bet> bets)
        {
            return new GetBetsResponse
            {
                Success = success,
                Message = message,
                BetList = { bets.Select(b => b.Map()).ToList() }
            };
        }
    }
}
