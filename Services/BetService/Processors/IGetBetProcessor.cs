namespace BetService.Processors
{
    public interface IGetBetProcessor
    {
        public Task<GetBetResponse> GetBet(GetBetRequest request, CancellationToken cancellationToken);
    }
}
