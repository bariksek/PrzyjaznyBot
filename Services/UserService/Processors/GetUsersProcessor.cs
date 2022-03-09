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
            var users = IsAnyIdProvided(request) ? postgreSqlContext.Users.Where(u => IsUserRequested(request, u)) : postgreSqlContext.Users;

            return Task.FromResult(_getUsersResponseBuilder.Build(true, $"Found {users.Count()} users", users.ToList()));
        }

        private static bool IsUserRequested(GetUsersRequest request, Model.User user)
        {
            return request.DiscordUserIds.Contains(user.DiscordUserId) || request.UserIds.Contains(user.Id);
        }

        private static bool IsAnyIdProvided(GetUsersRequest request)
        {
            return request.DiscordUserIds.Any() || request.UserIds.Any();
        }
    }
}
