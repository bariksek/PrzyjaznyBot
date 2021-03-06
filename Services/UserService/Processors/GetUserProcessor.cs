using Microsoft.EntityFrameworkCore;
using UserService.DAL;
using UserService.Builders;

namespace UserService.Processors
{
    public class GetUserProcessor : Processor<GetUserRequest, GetUserResponse>
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;
        private readonly IGetUserResponseBuilder _getUserResponseBuilder;

        public GetUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory,
            IGetUserResponseBuilder getUserResponseBuilder)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
            _getUserResponseBuilder = getUserResponseBuilder;
        }

        protected override async Task<GetUserResponse> HandleRequest(GetUserRequest request, CancellationToken cancellationToken)
        {
            return request.IdCase switch
            {
                GetUserRequest.IdOneofCase.UserId => await GetUserByUserId(request),
                GetUserRequest.IdOneofCase.DiscordUserId => await GetUserByDiscordId(request),
                _ => _getUserResponseBuilder.Build(false, "You must provide UserId or DiscordUserId", null)
            };
        }

        protected override Task<GetUserResponse> HandleException(Exception ex)
        {
            return Task.FromResult(_getUserResponseBuilder.Build(false, "Exception occured during processing", null));
        }

        private Task<GetUserResponse> GetUserByUserId(GetUserRequest request)
        {
            if (request.UserId <= 0)
            {
                return Task.FromResult(_getUserResponseBuilder.Build(false, "UserId must be greate than 0", null));
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var user = postgreSqlContext.Users.SingleOrDefault(u => u.Id == request.UserId);

            if (user == null)
            {
                return Task.FromResult(_getUserResponseBuilder.Build(false, "User not found", null));
            }

            return Task.FromResult(_getUserResponseBuilder.Build(true, "User found", user));
        }

        private Task<GetUserResponse> GetUserByDiscordId(GetUserRequest request)
        {
            if (request.DiscordUserId <= 0)
            {
                return Task.FromResult(_getUserResponseBuilder.Build(false, "DiscordUserId must be greate than 0", null));
            }

            using var postgreSqlContext = _postgreSqlContextFactory.CreateDbContext();
            var user = postgreSqlContext.Users.SingleOrDefault(u => u.DiscordUserId == request.DiscordUserId);

            if (user == null)
            {
                return Task.FromResult(_getUserResponseBuilder.Build(false, "User not found", null));
            }

            return Task.FromResult(_getUserResponseBuilder.Build(true, "User found", user));
        }
    }
}
