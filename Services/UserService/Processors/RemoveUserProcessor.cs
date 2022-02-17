using Microsoft.EntityFrameworkCore;
using UserService.DAL;

namespace UserService.Processors
{
    public class RemoveUserProcessor : IRemoveUserProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;

        public RemoveUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        public async Task<RemoveUserResponse> RemoveUser(RemoveUserRequest request)
        {
            if (request.DiscordUserId == 0)
            {
                return new()
                {
                    Success = false,
                    Message = "DiscordId must be greater than 0"
                };
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var userToRemove = postgreSqlContext.Users.SingleOrDefault(u => u.DiscordUserId == request.DiscordUserId);
            if (userToRemove is null)
            {
                return new()
                {
                    Success = false,
                    Message = $"User with DiscordId: {request.DiscordUserId} doesn't exist"
                };
            }
            postgreSqlContext.Users.Remove(userToRemove);
            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new()
                {
                    Success = false,
                    Message = $"Cannot remove user with DiscordId: {request.DiscordUserId}"
                };
            }

            return new()
            {
                Success = true,
                Message = $"User with DisordId: {request.DiscordUserId} removed"
            };
        }
    }
}
