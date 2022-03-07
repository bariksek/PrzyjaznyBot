namespace BetService.Builders
{
    public interface IGetUserBetsResponseBuilder
    {
        public GetUserBetsResponse Build(bool success, string message, IEnumerable<Model.UserBet> userBets);
    }
}
