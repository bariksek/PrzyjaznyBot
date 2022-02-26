namespace UserService.Builders
{
    public interface IGetUsersResponeBuilder
    {
        public GetUsersResponse Build(bool success, string message, IEnumerable<Model.User> users);
    }
}
