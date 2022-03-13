using BetService.Mappers;

namespace BetService.Builders
{
    public class GetUserBetsResponseBuilder : ResponseBuilderBase, IGetUserBetsResponseBuilder
    {
        public GetUserBetsResponse Build(bool success, string message, IEnumerable<Model.UserBet> userBets)
        {
            return new GetUserBetsResponse
            {
                Success = success,
                Message = message,
                UserBetList = { userBets.Select(b => b.Map()).ToList() }
            };
        }
    }
}
