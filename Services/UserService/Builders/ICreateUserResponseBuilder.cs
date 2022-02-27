namespace UserService.Builders
{
    public interface ICreateUserResponseBuilder
    {
        public CreateUserResponse Build(bool success, string message, Model.User? user);
    }
}
