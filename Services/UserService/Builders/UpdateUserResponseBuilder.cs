namespace UserService.Builders
{
    public class UpdateUserResponseBuilder : ResponseBuilderBase, IUpdateUserResponseBuilder
    {
        public UpdateUserResponse Build(bool success, string message, Model.User? user)
        {
            return new UpdateUserResponse
            {
                Success = success,
                Message = message,
                UserValue = MapToNullableUser(user)
            };
        }
    }
}
