using UserService.Mappers;

namespace UserService.Builders
{
    public class GetUsersResponseBuilder : ResponseBuilderBase, IGetUsersResponseBuilder
    {
        public GetUsersResponse Build(bool success, string message, IEnumerable<Model.User> users)
        {
            return new GetUsersResponse
            {
                Success = success,
                Message = message,
                UserList = { users.Select(u => u.Map()).ToList() }
            };
        }
    }
}
