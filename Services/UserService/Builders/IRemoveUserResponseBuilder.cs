namespace UserService.Builders
{
    public interface IRemoveUserResponseBuilder
    {
        public RemoveUserResponse Build(bool success, string message);
    }
}
