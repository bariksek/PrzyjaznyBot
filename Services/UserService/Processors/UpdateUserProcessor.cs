using Microsoft.EntityFrameworkCore;
using UserService.DAL;
using UserService.Builders;

namespace UserService.Processors
{
    public class UpdateUserProcessor : IUpdateUserProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IUpdateUserResponseBuilder _updateUserResponseBuilder;

        public UpdateUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IUpdateUserResponseBuilder updateUserResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _updateUserResponseBuilder = updateUserResponseBuilder;
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request)
        {
            if (request.DiscordUserId <= 0 || request.User is null)
            {
                return _updateUserResponseBuilder.Build(false, "DiscordId must be greater than 0 and user cannot be null", null);
            }
            
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var userToUpdate = postgreSqlContext.Users.SingleOrDefault(u => u.DiscordUserId == request.DiscordUserId);
            if (userToUpdate is null)
            {
                return _updateUserResponseBuilder.Build(false, $"User with DiscordId: {request.DiscordUserId} doesn't exist", null);
            }

            if (request.User.Points < 0 || request.User.Username == null || request.User.Username == string.Empty || request.User.LastDailyRewardClaimDateTime == null)
            {
                return _updateUserResponseBuilder.Build(false, "Cannot update user with provided values", null);
            }

            userToUpdate.Points = request.User.Points;
            userToUpdate.Username = request.User.Username;
            userToUpdate.LastDailyRewardClaimDateTime = request.User.LastDailyRewardClaimDateTime.ToDateTime();

            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return _updateUserResponseBuilder.Build(false, $"Cannot update user with DiscordId: {request.DiscordUserId}", null);
            }

            return _updateUserResponseBuilder.Build(true, $"User with DisordId: {request.DiscordUserId} updated", userToUpdate);
        }
    }
}
