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
    public class BetRepository
    {

        private readonly UserRepository UserRepository;

        public BetRepository()
        {
            UserRepository = new UserRepository();
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
                IsActive = true,
                Message = request.Message
            };

            using var dbContext = new MyDbContext();
            dbContext.Bets.Add(bet);
            var result = await dbContext.SaveChangesAsync();

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
            if (request.Value <= 0)
            {
                return new CreateUserBetResponse
                {
                    Success = false,
                    Message = "Points have to be greater than 0"
                };
            }

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

            using var dbContext = new MyDbContext();

            var bet = dbContext.Bets.FirstOrDefault(b => b.Id == request.BetId && b.IsActive);

            if (bet == null)
            {
                return new CreateUserBetResponse
                {
                    Success = false,
                    Message = "Bet not found or already finished"
                };
            }

            var userBet = new UserBet
            {
                UserId = getUserResponse.User.Id,
                BetId = request.BetId,
                Condition = (Condition)Enum.Parse(typeof(Condition), request.Condition),
                Value = request.Value
            };

            
            dbContext.Add(userBet);

            var substractPointsRequest = new SubstractPointsRequest
            {
                DiscordId = request.DiscordId,
                Value = request.Value
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

            var result = await dbContext.SaveChangesAsync();

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
            using var dbContext = new MyDbContext();
            var bet = dbContext.Bets.FirstOrDefault(b => b.Id == request.BetId);

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

            using var dbContext = new MyDbContext();
            var bet = dbContext.Bets.FirstOrDefault(b => b.Id == request.BetId && b.IsActive == true);

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

            var userBets = dbContext.UserBets.Where(ub => ub.BetId == bet.Id);
            var addPointsResponses = await GetAddPointsResponsesAsync(userBets, request.Condition);

            if(addPointsResponses != null && addPointsResponses.Any(response => !response.Success))
            {
                return new FinishBetResponse
                {
                    Success = false,
                    Message = "One or more add points operation failed"
                };
            }

            var result = await dbContext.SaveChangesAsync();

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

        private async Task<IEnumerable<AddPointsResponse>> GetAddPointsResponsesAsync(IEnumerable<UserBet> userBets, string condition)
        {
            var roundPrecision = 2;
            var successUserBets = userBets.Where(v => v.Condition == (Condition)Enum.Parse(typeof(Condition), condition));
            if(successUserBets.Count() == 0)
            {
                return null;
            }

            var failUserBets = userBets.Where(v => v.Condition != (Condition)Enum.Parse(typeof(Condition), condition));
            var prizePool = failUserBets.Sum(f => f.Value);
            var prizePerUser = Math.Round(prizePool / successUserBets.Count(), roundPrecision);
            var userUpdateTasks = new List<Task<AddPointsResponse>>();
            foreach (var userBet in successUserBets)
            {
                var addPointsRequest = new AddPointsRequest
                {
                    UserId = userBet.UserId,
                    Value = prizePerUser + userBet.Value
                };

                userUpdateTasks.Add(UserRepository.AddPoints(addPointsRequest));
            }

            return await Task.WhenAll(userUpdateTasks);
        }
    }
}
