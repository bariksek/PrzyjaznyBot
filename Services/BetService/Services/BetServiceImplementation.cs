using Grpc.Core;

namespace BetService.Services
{
    public class BetServiceImplementation : BetService.BetServiceBase
    {
        private readonly ILogger<BetServiceImplementation> _logger;

        public BetServiceImplementation(ILogger<BetServiceImplementation> logger)
        {
            _logger = logger;
        }

        public override Task<CreateBetResponse> CreateBet(CreateBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("CreateBet request handling started");

            return base.CreateBet(request, context);
        }

        public override Task<CreateUserBetResponse> CreateUserBet(CreateUserBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("CreateUserBet request handling started");

            return base.CreateUserBet(request, context);
        }

        public override Task<GetBetResponse> GetBet(GetBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetBet request handling started");

            return base.GetBet(request, context);
        }

        public override Task<GetBetsResponse> GetBets(GetBetsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetBets request handling started");

            return base.GetBets(request, context);
        }

        public override Task<GetUserBetsResponse> GetUserBets(GetUserBetsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetUserBets request handling started");

            return base.GetUserBets(request, context);
        }

        public override Task<FinishBetResponse> FinishBet(FinishBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("FinishBet request handling started");

            return base.FinishBet(request, context);
        }

        public override Task<StopBetResponse> StopBet(StopBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("StopBet request handling started");

            return base.StopBet(request, context);
        }
    }
}
