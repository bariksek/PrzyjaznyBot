using Microsoft.EntityFrameworkCore;
using UserService.DAL;

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
            throw new NotImplementedException();
        }
    }
}
