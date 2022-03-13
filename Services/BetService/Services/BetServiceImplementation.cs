using BetService.Processors;
using Grpc.Core;

namespace BetService.Services
{
    public class BetServiceImplementation : BetService.BetServiceBase
    {
        private readonly ILogger<BetServiceImplementation> _logger;
        private readonly IProcessor<CreateBetRequest, CreateBetResponse> _createBetProcessor;
        private readonly IProcessor<CreateUserBetRequest, CreateUserBetResponse> _createUserBetProcessor;
        private readonly IProcessor<GetBetRequest, GetBetResponse> _getBetProcessor;
        private readonly IProcessor<GetBetsRequest, GetBetsResponse> _getBetsProcessor;
        private readonly IProcessor<GetUserBetsRequest, GetUserBetsResponse> _getUserBetsProcessor;
        private readonly IProcessor<StopBetRequest, StopBetResponse> _stopBetProcessor;
        private readonly IProcessor<FinishBetRequest, FinishBetResponse> _finishBetProcessor;

        public BetServiceImplementation(ILogger<BetServiceImplementation> logger,
            IProcessor<CreateBetRequest, CreateBetResponse> createBetProcessor,
            IProcessor<CreateUserBetRequest, CreateUserBetResponse> createUserBetProcessor,
            IProcessor<GetBetRequest, GetBetResponse> getBetProcessor,
            IProcessor<GetBetsRequest, GetBetsResponse> getBetsProcessor,
            IProcessor<GetUserBetsRequest, GetUserBetsResponse> getUserBetsProcessor,
            IProcessor<StopBetRequest, StopBetResponse> stopBetProcessor, 
            IProcessor<FinishBetRequest, FinishBetResponse> finishBetProcessor)
        {
            _logger = logger;
            _createBetProcessor = createBetProcessor;
            _createUserBetProcessor = createUserBetProcessor;
            _getBetProcessor = getBetProcessor;
            _getBetsProcessor = getBetsProcessor;
            _getUserBetsProcessor = getUserBetsProcessor;
            _stopBetProcessor = stopBetProcessor;
            _finishBetProcessor = finishBetProcessor;
        }

        public override async Task<CreateBetResponse> CreateBet(CreateBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("CreateBet request handling started");

            return await _createBetProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<CreateUserBetResponse> CreateUserBet(CreateUserBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("CreateUserBet request handling started");

            return await _createUserBetProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<GetBetResponse> GetBet(GetBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetBet request handling started");

            return await _getBetProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<GetBetsResponse> GetBets(GetBetsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetBets request handling started");

            return await _getBetsProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<GetUserBetsResponse> GetUserBets(GetUserBetsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetUserBets request handling started");

            return await _getUserBetsProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<FinishBetResponse> FinishBet(FinishBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("FinishBet request handling started");

            return await _finishBetProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<StopBetResponse> StopBet(StopBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("StopBet request handling started");

            return await _stopBetProcessor.Process(request, context.CancellationToken);
        }
    }
}
