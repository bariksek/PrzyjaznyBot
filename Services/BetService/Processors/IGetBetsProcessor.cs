namespace BetService.Processors
{
    public interface IGetBetsProcessor
    {
        public Task<GetBetsResponse> GetBets(GetBetsRequest request, CancellationToken cancellationToken);
    }
}
