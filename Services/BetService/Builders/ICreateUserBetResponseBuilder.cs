namespace BetService.Builders
{
    public interface ICreateUserBetResponseBuilder
    {
        public CreateUserBetResponse Build(bool success, string message, Model.UserBet? userBet);
    }
}
