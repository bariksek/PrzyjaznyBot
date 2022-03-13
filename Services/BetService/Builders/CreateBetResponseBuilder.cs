namespace BetService.Builders
{
    public class CreateBetResponseBuilder : ResponseBuilderBase, ICreateBetResponseBuilder
    {
        public CreateBetResponse Build(bool success, string message, Model.Bet? bet)
        {
            return new CreateBetResponse
            {
                Success = success,
                Message = message,
                BetValue = MapToNullableBet(bet)
            };
        }
    }
}
