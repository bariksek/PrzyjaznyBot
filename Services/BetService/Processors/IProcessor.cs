namespace BetService.Processors
{
    public interface IProcessor<TRequest, TResponse>
    {
        public Task<TResponse> Process(TRequest request, CancellationToken cancellationToken);
    }
}
