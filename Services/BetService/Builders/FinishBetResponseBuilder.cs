namespace BetService.Builders
{
    public class FinishBetResponseBuilder : IFinishBetResponseBuilder
    {
        public FinishBetResponse Build(bool success, string message)
        {
            return new FinishBetResponse
            {
                Success = success,
                Message = message
            };
        }
    }
}
