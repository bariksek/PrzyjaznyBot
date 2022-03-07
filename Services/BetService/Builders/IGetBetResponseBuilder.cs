namespace BetService.Builders
{
    public interface IGetBetResponseBuilder
    {
        public GetBetResponse Build(bool success, string message, Model.Bet? bet);
    }
}
