using Microsoft.EntityFrameworkCore;
using UserService.DAL;
using UserService.Builders;

namespace UserService.Processors
{
    public class GetUserProcessor : IGetUserProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IGetUserResponseBuilder _getUserResponseBuilder;

        public GetUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IGetUserResponseBuilder getUserResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _getUserResponseBuilder = getUserResponseBuilder;
        }

        public Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            if (request.DiscordUserId <= 0)
            {
                return Task.FromResult(_getUserResponseBuilder.Build(false, "DiscordId must be greater than 0", null));
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var user = postgreSqlContext.Users.SingleOrDefault(u => u.DiscordUserId == request.DiscordUserId);

            if (user == null)
            {
                return Task.FromResult(_getUserResponseBuilder.Build(false, "User not found", null));
            }

            return Task.FromResult(_getUserResponseBuilder.Build(true, "User found", user));
        }
    }
}
