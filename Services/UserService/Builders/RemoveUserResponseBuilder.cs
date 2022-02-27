namespace UserService.Builders
{
    public class RemoveUserResponseBuilder : ResponseBuilderBase, IRemoveUserResponseBuilder
    {
        public RemoveUserResponse Build(bool success, string message)
        {
            return new RemoveUserResponse
            {
                Success = success,
                Message = message
            };
        }
    }
}
