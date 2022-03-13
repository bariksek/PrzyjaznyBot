namespace BetService.Builders
{
    public interface ICreateBetResponseBuilder
    {
        public CreateBetResponse Build(bool success, string message, Model.Bet? bet);
    }
}
