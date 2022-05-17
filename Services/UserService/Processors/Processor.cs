namespace UserService.Processors
{
    public abstract class Processor<TRequest, TResponse> : IProcessor<TRequest, TResponse>
    {
        public async Task<TResponse> Process(TRequest request, CancellationToken cancellationToken)
        {
            try
            {
                return await HandleRequest(request, cancellationToken);
            }
            catch (Exception ex)
            {
                return await HandleException(ex);
            }
        }

        protected abstract Task<TResponse> HandleRequest(TRequest request, CancellationToken cancellationToken);

        protected abstract Task<TResponse> HandleException(Exception ex);
    }
}
