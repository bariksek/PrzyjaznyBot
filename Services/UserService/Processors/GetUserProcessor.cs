using Microsoft.EntityFrameworkCore;
using UserService.DAL;

namespace UserService.Processors
{
    public class GetUserProcessor : IGetUserProcessor
    {
        private readonly IDbContextFactory<PostgreSqlContext> _postgreSqlContextFactory;

        public GetUserProcessor(IDbContextFactory<PostgreSqlContext> postgreSqlContextFactory)
        {
            _postgreSqlContextFactory = postgreSqlContextFactory;
        }

        public Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
