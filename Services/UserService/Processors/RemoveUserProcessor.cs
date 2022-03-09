using Microsoft.EntityFrameworkCore;
using UserService.Builders;
using UserService.DAL;

namespace UserService.Processors
{
    public class RemoveUserProcessor : IProcessor<RemoveUserRequest, RemoveUserResponse>
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IRemoveUserResponseBuilder _removeUserResponseBuilder;

        public RemoveUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IRemoveUserResponseBuilder removeUserResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _removeUserResponseBuilder = removeUserResponseBuilder;
        }

        public async Task<RemoveUserResponse> Process(RemoveUserRequest request, CancellationToken cancellationToken)
        {
            if (request.DiscordUserId <= 0)
            {
                return _removeUserResponseBuilder.Build(false, "DiscordId must be greater than 0");
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var userToRemove = postgreSqlContext.Users.SingleOrDefault(u => u.DiscordUserId == request.DiscordUserId);

            if (userToRemove is null)
            {
                return _removeUserResponseBuilder.Build(false, $"User with DiscordId: {request.DiscordUserId} doesn't exist");
            }

            postgreSqlContext.Users.Remove(userToRemove);

            var result = await postgreSqlContext.SaveChangesAsync(cancellationToken);

            if (result == 0)
            {
                return _removeUserResponseBuilder.Build(false, $"Cannot remove user with DiscordId: {request.DiscordUserId}");
            }

            return _removeUserResponseBuilder.Build(true, $"User with DisordId: {request.DiscordUserId} removed");
        }
    }
}
