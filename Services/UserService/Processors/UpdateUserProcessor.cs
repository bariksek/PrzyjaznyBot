using Microsoft.EntityFrameworkCore;
using UserService.DAL;
using Google.Protobuf.WellKnownTypes;
using UserService.Mappers;

namespace UserService.Processors
{
    public class UpdateUserProcessor : IUpdateUserProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;

        public UpdateUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request)
        {
            if (request.DiscordUserId == 0 || request.User is null)
            {
                return new()
                {
                    Success = false,
                    Message = "DiscordId must be greater than 0 and user cannot be null",
                    UserValue = new()
                    {
                        Null = NullValue.NullValue
                    }
                };
            }
            
            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var userToUpdate = postgreSqlContext.Users.SingleOrDefault(u => u.DiscordUserId == request.DiscordUserId);
            if (userToUpdate is null)
            {
                return new()
                {
                    Success = false,
                    Message = $"User with DiscordId: {request.DiscordUserId} doesn't exist",
                    UserValue = new()
                    {
                        Null = NullValue.NullValue
                    }
                };
            }

            userToUpdate.Points = request.User.Points;
            userToUpdate.Username = request.User.Username;
            userToUpdate.LastDailyRewardClaimDateTime = request.User.LastDailyRewardClaimDateTime.ToDateTime();

            var result = await postgreSqlContext.SaveChangesAsync();

            if (result == 0)
            {
                return new()
                {
                    Success = false,
                    Message = $"Cannot update user with DiscordId: {request.DiscordUserId}",
                    UserValue = new()
                    {
                        Null = NullValue.NullValue
                    }
                };
            }

            return new()
            {
                Success = true,
                Message = $"User with DisordId: {request.DiscordUserId} updated",
                UserValue = new()
                {
                    User = userToUpdate.Map()
                }
            };
        }
    }
}
