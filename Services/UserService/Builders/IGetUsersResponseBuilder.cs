namespace UserService.Builders
{
    public interface IGetUsersResponseBuilder
    {
        public GetUsersResponse Build(bool success, string message, IEnumerable<Model.User> users);
    }
}
