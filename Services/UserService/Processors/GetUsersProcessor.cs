using Microsoft.EntityFrameworkCore;
using UserService.DAL;
using UserService.Mappers;

namespace UserService.Processors
{
    public class GetUsersProcessor : IGetUsersProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;

        public GetUsersProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        public Task<GetUsersResponse> GetUsers(GetUsersRequest request)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var users = request.DiscordUserIds.Any() ? postgreSqlContext.Users.Where(u => request.DiscordUserIds.Contains(u.DiscordUserId)) : postgreSqlContext.Users;

            return Task.FromResult(new GetUsersResponse
            {
                Success = true,
                Message = $"Found {users.Count()} users",
                UserList = { users.Select(u => u.Map()).ToList() }
            });
        }
    }
}
