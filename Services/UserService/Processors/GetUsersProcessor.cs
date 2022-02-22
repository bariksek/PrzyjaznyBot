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

        public async Task<GetUsersResponse> GetUsers(GetUsersRequest request)
        {
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var users = request.DiscordUserIds.Any() ? postgreSqlContext.Users.Where(u => request.DiscordUserIds.Contains(u.DiscordUserId)) : postgreSqlContext.Users;

            return new()
            {
                Success = true,
                Message = "",
                UserList = { users.Select(u => u.Map()) }
            };
        }
    }
}
