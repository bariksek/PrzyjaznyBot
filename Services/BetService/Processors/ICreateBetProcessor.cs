namespace BetService.Processors
{
    public interface ICreateBetProcessor
    {
        public Task<CreateBetResponse> CreateBet(CreateBetRequest request, CancellationToken cancellationToken);
    }
}
