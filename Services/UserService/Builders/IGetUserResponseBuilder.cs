namespace UserService.Builders
{
    public interface IGetUserResponseBuilder
    {
        public GetUserResponse Build(bool success, string message, Model.User? user);
    }
}
