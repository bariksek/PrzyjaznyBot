namespace BetService.Processors
{
    public interface IFinishBetProcessor
    {
        public Task<FinishBetResponse> FinishBet(FinishBetRequest request, CancellationToken cancellationToken);
    }
}
