using Microsoft.EntityFrameworkCore;
using UserService.Builders;
using UserService.DAL;

namespace UserService.Processors
{
    public class CreateUserProcessor : Processor<CreateUserRequest, CreateUserResponse>
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly ICreateUserResponseBuilder _createUserResponseBuilder;

        public CreateUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            ICreateUserResponseBuilder createUserResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _createUserResponseBuilder = createUserResponseBuilder;
        }

        protected override async Task<CreateUserResponse> HandleRequest(CreateUserRequest request, CancellationToken cancellationToken)
        {
            if (request.DiscordUserId <= 0 || request?.Username is null || request.Username == string.Empty)
            {
                return _createUserResponseBuilder.Build(false, "DiscordId must be greater than 0 and Username cannot be null or empty", null);
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();

            if(postgreSqlContext.Users.Any(u => u.DiscordUserId == request.DiscordUserId))
            {
                return _createUserResponseBuilder.Build(false, $"User with DiscordId: {request.DiscordUserId} already exists", null);
            }

            Model.User user = new()
            {
                DiscordUserId = request.DiscordUserId,
                Username = request.Username,
                Points = 0,
                LastDailyRewardClaimDateTime = DateTime.UtcNow
            };

            postgreSqlContext.Users.Add(user);
            var result = await postgreSqlContext.SaveChangesAsync(cancellationToken);

            if (result == 0)
            {
                return _createUserResponseBuilder.Build(false, $"Cannot add user with DiscordId: {request.DiscordUserId} and Username: {request.Username}", null);
            }

            return _createUserResponseBuilder.Build(true, "User created", postgreSqlContext.Users.Single(u => u.DiscordUserId == user.DiscordUserId));
        }

        protected override Task<CreateUserResponse> HandleException(Exception ex)
        {
            return Task.FromResult(_createUserResponseBuilder.Build(false, "Exception occured during processing", null));
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               