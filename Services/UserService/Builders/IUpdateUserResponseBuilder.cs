namespace UserService.Builders
{
    public interface IUpdateUserResponseBuilder
    {
        public UpdateUserResponse Build(bool success, string message, Model.User? user);
    }
}
