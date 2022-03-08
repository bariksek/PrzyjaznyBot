namespace BetService.Processors
{
    public interface IStopBetProcessor
    {
        public Task<StopBetResponse> StopBet(StopBetRequest request, CancellationToken cancellationToken);
    }
}
