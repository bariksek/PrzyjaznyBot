using PrzyjaznyBot.Model;
using System.Linq;
using System.Threading.Tasks;
using PrzyjaznyBot.DTO.UserRepository;
using Microsoft.EntityFrameworkCore;

namespace PrzyjaznyBot.DAL
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;

        public UserRepository(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        async public Task<CreateUserResponse> CreateNewUser(CreateUserRequest request)
        {
            if(request.Points < 0)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = "Points have to be greater than or equal to 0"
                };
            }

            User user = new User
            {
                DiscordUserId = request.DiscordId,
                Username = request.Username,
                Points = request.Points,
                LastDailyRewardClaimDateTime = request.LastDailyRewardClaimDateTime
            };

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            postgreSqlContext.Users.Add(user);
            var result = await postgreSqlContext.SaveChangesAsync();

            if(result == 0)
            {
                return new CreateUserResponse
                {
                    Success = false,
                    Message = $"Cannot add user with DiscordId: {request.DiscordId} and Username: {request.Username}",
                    CreatedUser = null
                };
            }

            return new CreateUserResponse
            {
                Success = true,
                Message = "User created",
                CreatedUser = user
            };
        }

        public GetUserResponse GetUser(GetUserRequest request)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var user = request.DiscordId > 0 ?
                postgreSqlContext.Users.FirstOrDefault(u => u.DiscordUserId == request.DiscordId) :
                postgreSqlContext.Users.FirstOrDefault(u => u.Id == request.UserId);

            if (user == null)
            {
                return new GetUserResponse
                {
                    Success = false,
                    Message = $"Cannot find user with DiscordId: {request.DiscordId} or Id: {request.UserId}.",
                    User = null
                };
            }

            return new GetUserResponse
            {
                Success = true,
                Message = "User found.",
                User = user
            };
        }

        async public Task<TransferPointsResponse> TransferPoints(TransferPointsRequest request)
        {
            if (request.Value <= 0)
            {
                return new TransferPointsResponse
                {
                    Success = false,
                    Message = "Points have to be greater than 0"
                };
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var senderUser = request.SenderDiscordId > 0 ?
                postgreSqlContext.Users.FirstOrDefault(u => u.DiscordUserId == request.SenderDiscordId) :
                postgreSqlContext.Users.FirstOrDefault(u => u.Id == request.SenderUserId);
            var targetUser = request.ReceiverDiscordId > 0 ?
                postgreSqlContext.Users.FirstOrDefault(u => u.DiscordUserId == request.ReceiverDiscordId) :
                postgreSqlContext.Users.FirstOrDefault(u => u.Id == request.ReceiverUserId);

            if (senderUser == null || targetUser == null)
            {
                return new TransferPointsResponse
                {
                    Success = false,
                    Message = $"Sender(DiscordId: {request.SenderDiscordId} or Id: {request.SenderUserId}) or target(DiscordId: {request.ReceiverDiscordId} or Id: {request.ReceiverUserId}) user not found."
                };
            }

            if (senderUser.Points < request.Value)
            {
                return new TransferPointsResponse
                {
                    Success = false,
                    Message = $"User with ID: {senderUser.Username} doesn't have enough points to transfer."
                };
            }

            senderUser.Points -= request.Value;
            targetUser.Points += request.Value;

            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new TransferPointsResponse
                {
                    Success = false,
                    Message = $"Unknown error during points transfer"
                };
            }

            return new TransferPointsResponse
            {
                Success = true,
                Message = "Points transfered.",
                TransferedPoints = request.Value
            };
        }

        async public Task<AddPointsResponse> AddPoints(AddPointsRequest request)
        {
            if (request.Value <= 0)
            {
                return new AddPointsResponse
                {
                    Success = false,
                    Message = "Points have to be greater than 0"
                };
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var user = request.DiscordId > 0 ?
                postgreSqlContext.Users.FirstOrDefault(u => u.DiscordUserId == request.DiscordId) :
                postgreSqlContext.Users.FirstOrDefault(u => u.Id == request.UserId);

            if (user == null)
            {
                return new AddPointsResponse
                {
                    Success = false,
                    Message = $"User with DiscordId: {request.DiscordId} or Id: {request.UserId} not found"
                };
            }

            user.Points += request.Value;
            user.LastDailyRewardClaimDateTime = request.IsDailyReward ? System.DateTime.Now : user.LastDailyRewardClaimDateTime;

            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new AddPointsResponse
                {
                    Success = false,
                    Message = $"Unknown error during adding points"
                };
            }

            return new AddPointsResponse
            {
                Success = true,
                Message = "Points added.",
                AddedPoints = request.Value
            };
        }

        async public Task<SubstractPointsResponse> SubstractPoints(SubstractPointsRequest request)
        {
            if (request.Value <= 0)
            {
                return new SubstractPointsResponse
                {
                    Success = false,
                    Message = "Points have to be greater than 0"
                };
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var user = request.DiscordId > 0 ?
                postgreSqlContext.Users.FirstOrDefault(u => u.DiscordUserId == request.DiscordId) :
                postgreSqlContext.Users.FirstOrDefault(u => u.Id == request.UserId);

            if (user == null)
            {
                return new SubstractPointsResponse
                {
                    Success = false,
                    Message = $"User with DiscordId: {request.DiscordId} or Id: {request.UserId} not found"
                };
            }

            if (user.Points < request.Value)
            {
                return new SubstractPointsResponse
                {
                    Success = false,
                    Message = $"User with DiscordId: {user.Username} does not have enough points to substract"
                };
            }
            user.Points -= request.Value;
            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new SubstractPointsResponse
                {
                    Success = false,
                    Message = $"Unknown error during substracting points"
                };
            }

            return new SubstractPointsResponse
            {
                Success = true,
                Message = "Points substracted",
                SubstractedPoints = request.Value
            };
        }

        public GetUsersResponse GetUsers(GetUsersRequest request)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var users = request.DiscordIds == null ? postgreSqlContext.Users.ToList() : postgreSqlContext.Users.Where(u => request.DiscordIds.Contains(u.DiscordUserId)).ToList();

            if (users == null || users.Count() == 0)
            {
                return new GetUsersResponse
                {
                    Success = false,
                    Message = "No users found",
                    Users = null
                };
            }

            return new GetUsersResponse
            {
                Success = true,
                Message = "Users found",
                Users = users
            };
        }
    }
}
