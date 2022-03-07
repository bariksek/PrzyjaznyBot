namespace BetService.Processors
{
    public interface IGetUserBetsProcessor
    {
        public Task<GetUserBetsResponse> GetUserBets(GetUserBetsRequest request, CancellationToken cancellationToken);
    }
}
