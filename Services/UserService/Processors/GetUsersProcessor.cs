using Microsoft.EntityFrameworkCore;
using UserService.Builders;
using UserService.DAL;

namespace UserService.Processors
{
    public class GetUsersProcessor : IProcessor<GetUsersRequest, GetUsersResponse>
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IGetUsersResponseBuilder _getUsersResponseBuilder;

        public GetUsersProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IGetUsersResponseBuilder getUsersResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _getUsersResponseBuilder = getUsersResponseBuilder;
        }

        public Task<GetUsersResponse> Process(GetUsersRequest request, CancellationToken cancellationToken)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var users = IsAnyIdProvided(request) ? 
                postgreSqlContext.Users.Where(u => request.DiscordUserIds.Contains(u.DiscordUserId) || request.UserIds.Contains(u.Id)) : 
                postgreSqlContext.Users;

            return Task.FromResult(_getUsersResponseBuilder.Build(true, $"Found {users.Count()} users", users.ToList()));
        }

        private static bool IsAnyIdProvided(GetUsersRequest request)
        {
            return request.DiscordUserIds.Any() || request.UserIds.Any();
        }
    }
}
