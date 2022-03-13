namespace BetService.Builders
{
    public class StopBetResponseBuilder : IStopBetResponseBuilder
    {
        public StopBetResponse Build(bool success, string message)
        {
            return new StopBetResponse
            {
                Success = success,
                Message = message
            };
        }
    }
}
