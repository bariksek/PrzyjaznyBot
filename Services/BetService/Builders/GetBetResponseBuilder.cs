namespace BetService.Builders
{
    public class GetBetResponseBuilder : ResponseBuilderBase, IGetBetResponseBuilder
    {
        public GetBetResponse Build(bool success, string message, Model.Bet? bet)
        {
            return new GetBetResponse
            {
                Success = success,
                Message = message,
                BetValue = MapToNullableBet(bet)
            };
        }
    }
}
