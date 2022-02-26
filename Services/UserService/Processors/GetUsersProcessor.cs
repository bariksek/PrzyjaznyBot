using Microsoft.EntityFrameworkCore;
using UserService.Builders;
using UserService.DAL;

namespace UserService.Processors
{
    public class GetUsersProcessor : IGetUsersProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IGetUsersResponseBuilder _getUsersResponseBuilder;

        public GetUsersProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IGetUsersResponseBuilder getUsersResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _getUsersResponseBuilder = getUsersResponseBuilder;
        }

        public Task<GetUsersResponse> GetUsers(GetUsersRequest request)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var users = request.DiscordUserIds.Any() ? postgreSqlContext.Users.Where(u => request.DiscordUserIds.Contains(u.DiscordUserId)) : postgreSqlContext.Users;

            return Task.FromResult(_getUsersResponseBuilder.Build(true, $"Found {users.Count()} users", users.ToList()));
        }
    }
}
