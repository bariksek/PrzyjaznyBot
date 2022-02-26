namespace UserService.Builders
{
    public class CreateUserResponseBuilder : ResponseBuilderBase, ICreateUserResponseBuilder
    {
        public CreateUserResponse Build(bool success, string message, Model.User? user)
        {
            return new CreateUserResponse
            {
                Success = success,
                Message = message,
                UserValue = MapToNullableUser(user)
            };
        }
    }
}
