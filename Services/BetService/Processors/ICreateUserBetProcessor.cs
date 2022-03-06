namespace BetService.Processors
{
    public interface ICreateUserBetProcessor
    {
        public Task<CreateUserBetResponse> CreateUserBet(CreateUserBetRequest request, CancellationToken cancellationToken);
    }
}
