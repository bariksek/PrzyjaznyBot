﻿using Microsoft.EntityFrameworkCore;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.DTO.BetRepository;
using PrzyjaznyBot.DTO.UserRepository;
using PrzyjaznyBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrzyjaznyBot.DAL
{
    public class BetRepository : IBetRepository
    {
        private readonly IUserRepository UserRepository;
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;

        public BetRepository(IUserRepository userRepository, IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            UserRepository = userRepository;
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        async public Task<CreateBetResponse> CreateBet(CreateBetRequest request)
        {
            var getUserRequest = new GetUserRequest
            {
                DiscordId = request.DiscordId
            };

            var getUserResponse = UserRepository.GetUser(getUserRequest);

            if (!getUserResponse.Success)
            {
                return new CreateBetResponse
                {
                    Success = false,
                    Message = $"Cannot find user with DiscordId: {request.DiscordId}"
                };
            }

            var bet = new Bet
            {
                AuthorId = getUserResponse.User.Id,
                IsStopped = false,
                IsActive = true,
                Message = request.Message,
                DateTime = DateTime.Now,
                Stake = request.Stake
            };

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            postgreSqlContext.Bets.Add(bet);
            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new CreateBetResponse
                {
                    Success = false,
                    Message = "Unknow error during bet creation"
                };
            }

            return new CreateBetResponse
            {
                Success = true,
                Message = "Bet created",
                Bet = bet
            };
        }

        async public Task<CreateUserBetResponse> CreateUserBet(CreateUserBetRequest request)
        {
            var getUserRequest = new GetUserRequest
            {
                DiscordId = request.DiscordId
            };

            var getUserResponse = UserRepository.GetUser(getUserRequest);

            if (!getUserResponse.Success)
            {
                return new CreateUserBetResponse
                {
                    Success = false,
                    Message = $"Cannot find user with DiscordId: {request.DiscordId}"
                };
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bet = postgreSqlContext.Bets.FirstOrDefault(b => b.Id == request.BetId && b.IsActive && !b.IsStopped);

            if (bet == null)
            {
                return new CreateUserBetResponse
                {
                    Success = false,
                    Message = "Bet not found, stopped or already finished"
                };
            }

            var userBet = new UserBet
            {
                UserId = getUserResponse.User.Id,
                BetId = request.BetId,
                Condition = (Condition)Enum.Parse(typeof(Condition), request.Condition)
            };


            postgreSqlContext.Add(userBet);

            var substractPointsRequest = new SubstractPointsRequest
            {
                DiscordId = request.DiscordId,
                Value = bet.Stake
            };

            var substractPointsResponse = await UserRepository.SubstractPoints(substractPointsRequest);

            if (!substractPointsResponse.Success)
            {
                return new CreateUserBetResponse
                {
                    Success = false,
                    Message = substractPointsResponse.Message
                };
            }

            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new CreateUserBetResponse
                {
                    Success = false,
                    Message = "Unknow error during user bet creation"
                };
            }

            return new CreateUserBetResponse
            {
                Success = true,
                Message = "Bet created",
                UserBet = userBet
            };
        }

        public GetBetResponse GetBet(GetBetRequest request)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bet = postgreSqlContext.Bets.FirstOrDefault(b => b.Id == request.BetId);

            if (bet == null)
            {
                return new GetBetResponse
                {
                    Success = false,
                    Message = "Bet not found"
                };
            }

            return new GetBetResponse
            {
                Success = true,
                Message = "Bet found",
                Bet = bet
            };
        }

        public GetBetInfoResponse GetUserBets(GetBetInfoRequest request)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var userBets = postgreSqlContext.UserBets.Where(b => b.BetId == request.BetId).ToList();

            if (userBets == null)
            {
                return new GetBetInfoResponse
                {
                    Success = false,
                    Message = "UserBets not found"
                };
            }

            return new GetBetInfoResponse
            {
                Success = true,
                Message = "Bet found",
                UserBets = userBets
            };
        }

        async public Task<FinishBetResponse> FinishBet(FinishBetRequest request)
        {
            var getUserRequest = new GetUserRequest
            {
                DiscordId = request.DiscordId
            };

            var getUserResponse = UserRepository.GetUser(getUserRequest);

            if (!getUserResponse.Success)
            {
                return new FinishBetResponse
                {
                    Success = false,
                    Message = getUserResponse.Message
                };
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bet = postgreSqlContext.Bets.FirstOrDefault(b => b.Id == request.BetId && b.IsActive == true);

            if (bet == null)
            {
                return new FinishBetResponse
                {
                    Success = false,
                    Message = $"Bet with Id: {request.BetId} not found or already finished"
                };
            }

            if (bet.AuthorId != getUserResponse.User.Id)
            {
                return new FinishBetResponse
                {
                    Success = false,
                    Message = $"User {getUserResponse.User.Username} is not an author of bet with Id: {request.BetId}"
                };
            }

            bet.IsActive = false;

            var userBets = postgreSqlContext.UserBets.Where(ub => ub.BetId == bet.Id);
            var addPointsResponses = await GetAddPointsResponsesAsync(bet, userBets, request.Condition);

            if (addPointsResponses != null && addPointsResponses.Any(response => !response.Success))
            {
                return new FinishBetResponse
                {
                    Success = false,
                    Message = "One or more add points operation failed"
                };
            }

            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new FinishBetResponse
                {
                    Success = false,
                    Message = "Unknow error during bet finishing"
                };
            }

            return new FinishBetResponse
            {
                Success = true,
                Message = "Bet finished",
                Bet = bet
            };
        }

        async public Task<StopBetResponse> StopBet(StopBetRequest request)
        {
            var getUserRequest = new GetUserRequest
            {
                DiscordId = request.DiscordId
            };

            var getUserResponse = UserRepository.GetUser(getUserRequest);

            if (!getUserResponse.Success)
            {
                return new StopBetResponse
                {
                    Success = false,
                    Message = getUserResponse.Message
                };
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bet = postgreSqlContext.Bets.FirstOrDefault(b => b.Id == request.BetId && b.IsActive == true);

            if (bet == null)
            {
                return new StopBetResponse
                {
                    Success = false,
                    Message = $"Bet with Id: {request.BetId} is already not active for betting."
                };
            }

            if (bet.AuthorId != getUserResponse.User.Id)
            {
                return new StopBetResponse
                {
                    Success = false,
                    Message = $"User {getUserResponse.User.Username} is not an author of bet with Id: {request.BetId}"
                };
            }

            bet.IsStopped = true;

            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new StopBetResponse
                {
                    Success = false,
                    Message = "Unknow error during bet finishing"
                };
            }

            return new StopBetResponse
            {
                Success = true,
                Message = $"Bet {request.BetId} is not active for betting.",
                Bet = bet
            };
        }

        async public Task<GetBetsResponse> GetBets(GetBetsRequest request)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var bets = request.ShowNotActive ? postgreSqlContext.Bets.ToList() : postgreSqlContext.Bets.Where(b => b.IsActive).ToList();

            if (bets == null || bets.Count() == 0)
            {
                return new GetBetsResponse
                {
                    Success = false,
                    Message = "No bets found",
                    Bets = null
                };
            }

            return new GetBetsResponse
            {
                Success = true,
                Message = "Bets found",
                Bets = bets
            };
        }

        private async Task<IEnumerable<AddPointsResponse>> GetAddPointsResponsesAsync(Bet bet, IEnumerable<UserBet> userBets, string condition)
        {
            var roundPrecision = 2;
            var successUserBets = userBets.Where(v => v.Condition == (Condition)Enum.Parse(typeof(Condition), condition));
            if (successUserBets.Count() == 0)
            {
                return null;
            }

            var failUserBets = userBets.Where(v => v.Condition != (Condition)Enum.Parse(typeof(Condition), condition));
            var prizePool = failUserBets.Count() * bet.Stake;
            var prizePerUser = Math.Round(prizePool / successUserBets.Count(), roundPrecision);
            var userUpdateTasks = new List<Task<AddPointsResponse>>();
            foreach (var userBet in successUserBets)
            {
                var addPointsRequest = new AddPointsRequest
                {
                    UserId = userBet.UserId,
                    Value = prizePerUser + bet.Stake
                };

                userUpdateTasks.Add(UserRepository.AddPoints(addPointsRequest));
            }

            return await Task.WhenAll(userUpdateTasks);
        }
    }
}