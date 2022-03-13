namespace BetService.Builders
{
    public interface IGetBetsResponseBuilder
    {
        public GetBetsResponse Build(bool success, string message, IEnumerable<Model.Bet> bets);
    }
}
