﻿using BetService.Builders;
using BetService.DAL;
using Microsoft.EntityFrameworkCore;

namespace BetService.Processors
{
    public class StopBetProcessor : IStopBetProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IStopBetResponseBuilder _stopBetResponseBuilder;
        private readonly UserService.UserService.UserServiceClient _userServiceClient;

        public StopBetProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IStopBetResponseBuilder stopBetResponseBuilder,
            UserService.UserService.UserServiceClient userServiceClient)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _stopBetResponseBuilder = stopBetResponseBuilder;
            _userServiceClient = userServiceClient;
        }

        public async Task<StopBetResponse> StopBet(StopBetRequest request, CancellationToken cancellationToken)
        {
            if(request.BetId <= 0)
            {
                return _stopBetResponseBuilder.Build(false, "BetId must be greater than 0");
            }

            var getUserResponse = await GetUser(request.DiscordId, cancellationToken);

            if (!getUserResponse.Success)
            {
                return _stopBetResponseBuilder.Build(false, getUserResponse.Message);
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bet = postgreSqlContext.Bets.FirstOrDefault(b => b.Id == request.BetId && b.IsActive == true);

            if (bet == null)
            {
                return _stopBetResponseBuilder.Build(false, $"Bet with Id: {request.BetId} is not found or already stopped");
            }

            if (bet.AuthorId != getUserResponse.UserValue.User.Id)
            {
                return _stopBetResponseBuilder.Build(false, $"User {getUserResponse.UserValue.User.Username} is not an author of bet with Id: {request.BetId}");
            }

            bet.IsStopped = true;

            var result = await postgreSqlContext.SaveChangesAsync(cancellationToken);

            if (result == 0)
            {
                return _stopBetResponseBuilder.Build(false, "Unknow error during bet finishing");
            }

            return _stopBetResponseBuilder.Build(true, $"Bet {request.BetId} is not active for betting.");
        }

        private async Task<UserService.GetUserResponse> GetUser(ulong discordId, CancellationToken cancellationToken)
        {
            var getUserRequest = new UserService.GetUserRequest
            {
                DiscordUserId = discordId
            };

            return await _userServiceClient.GetUserAsync(getUserRequest, cancellationToken: cancellationToken);
        }
    }
}
