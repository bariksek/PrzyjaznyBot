﻿using BetService.Processors;
using Grpc.Core;

namespace BetService.Services
{
    public class BetServiceImplementation : BetService.BetServiceBase
    {
        private readonly ILogger<BetServiceImplementation> _logger;
        private readonly ICreateBetProcessor _createBetProcessor;
        private readonly ICreateUserBetProcessor _createUserBetProcessor;
        private readonly IGetBetProcessor _getBetProcessor;
        private readonly IGetBetsProcessor _getBetsProcessor;

        public BetServiceImplementation(ILogger<BetServiceImplementation> logger,
            ICreateBetProcessor createBetProcessor,
            ICreateUserBetProcessor createUserBetProcessor,
            IGetBetProcessor getBetProcessor,
            IGetBetsProcessor getBetsProcessor)
        {
            _logger = logger;
            _createBetProcessor = createBetProcessor;
            _createUserBetProcessor = createUserBetProcessor;
            _getBetProcessor = getBetProcessor;
            _getBetsProcessor = getBetsProcessor;
        }

        public override async Task<CreateBetResponse> CreateBet(CreateBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("CreateBet request handling started");

            return await _createBetProcessor.CreateBet(request, context.CancellationToken);
        }

        public override async Task<CreateUserBetResponse> CreateUserBet(CreateUserBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("CreateUserBet request handling started");

            return await _createUserBetProcessor.CreateUserBet(request, context.CancellationToken);
        }

        public override async Task<GetBetResponse> GetBet(GetBetRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetBet request handling started");

            return await _getBetProcessor.GetBet(request, context.CancellationToken);
        }

        public override async Task<GetBetsResponse> GetBets(GetBetsRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetBets request handling started");

            return await _getBetsProcessor.GetBets(request, context.CancellationToken);
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