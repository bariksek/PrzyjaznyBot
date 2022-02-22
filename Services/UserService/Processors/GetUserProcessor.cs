using Microsoft.EntityFrameworkCore;
using UserService.DAL;
using Google.Protobuf.WellKnownTypes;
using UserService.Mappers;

namespace UserService.Processors
{
    public class GetUserProcessor : IGetUserProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;

        public GetUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        public Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            if (request.DiscordUserId == 0)
            {
                return Task.FromResult(new GetUserResponse
                {
                    Success = false,
                    Message = "DiscordId must be greater than 0",
                    UserValue =
                    {
                        Null = NullValue.NullValue
                    }
                });
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var user = postgreSqlContext.Users.SingleOrDefault(u => u.DiscordUserId == request.DiscordUserId);

            if (user == null)
            {
                return Task.FromResult(new GetUserResponse
                {
                    Success = false,
                    Message = "DiscordId must be greater than 0",
                    UserValue =
                    {
                        Null = NullValue.NullValue
                    }
                });
            }

            return Task.FromResult(new GetUserResponse
            {
                Success = true,
                Message = "User found",
                UserValue =
                {
                    User = user.Map()
                }
            });
        }
    }
}
