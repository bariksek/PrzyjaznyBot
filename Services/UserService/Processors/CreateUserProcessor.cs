using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using UserService.DAL;
using UserService.Mappers;

namespace UserService.Processors
{
    public class CreateUserProcessor : ICreateUserProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;

        public CreateUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
        {
            if (request.DiscordUserId == 0 || request?.Username is null || request.Username == string.Empty)
            {
                return new()
                {
                    Success = false,
                    Message = "DiscordId must be greater than 0 and Username cannot be null or empty",
                    UserValue = new()
                    {
                        Null = NullValue.NullValue
                    }
                };
            }

            Model.User user = new()
            {
                DiscordUserId = request.DiscordUserId,
                Username = request.Username,
                Points = 0,
                LastDailyRewardClaimDateTime = DateTime.UtcNow
            };

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            await postgreSqlContext.Users.AddAsync(user);
            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new()
                {
                    Success = false,
                    Message = $"Cannot add user with DiscordId: {request.DiscordUserId} and Username: {request.Username}",
                    UserValue = new()
                    {
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
                    User = user.Map()
                }
            };
        }
    }
}
