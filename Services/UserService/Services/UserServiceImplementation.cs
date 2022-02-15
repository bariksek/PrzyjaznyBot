using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using UserService.DAL;

namespace UserService.Services
{
    public class UserServiceImplementation : UserService.UserServiceBase
    {
        private readonly ILogger<UserServiceImplementation> _logger;
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        public UserServiceImplementation(ILogger<UserServiceImplementation> logger,
            IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            _logger = logger;
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Create user request handling started");

            Model.User user = new()
            {
                DiscordUserId = request.DiscordId,
                Username = request.Username,
                Points = 0,
                LastDailyRewardClaimDateTime = DateTime.UtcNow
            };

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            postgreSqlContext.Users.Add(user);
            var result = await postgreSqlContext.SaveChangesAsync();

            if(result == 0)
            {
                return new()
                {
                    Success = false,
                    Message = $"Cannot add user with DiscordId: {request.DiscordId} and Username: {request.Username}",
                    UserValue = new() { 
                        Null = NullValue.NullValue
                    }
                };
            }

            return new()
            {
                Success = true,
                Message = "User created",
                UserValue = new()
                {
                    User = CreateUser(user)
                }
            };
        }

        private static User CreateUser(Model.User user)
        {
            if(user is null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return new()
            {
                DiscordUserId = user.DiscordUserId,
                Username = user.Username,
                Id = user.Id,
                Points = user.Points,
                LastDailyRewardClaimDateTime = user.LastDailyRewardClaimDateTime.ToTimestamp()
            };
        }
    }
}