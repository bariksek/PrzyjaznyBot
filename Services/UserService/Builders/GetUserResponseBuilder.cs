namespace UserService.Builders
{
    public class GetUserResponseBuilder : ResponseBuilderBase, IGetUserResponseBuilder
    {
        public GetUserResponse Build(bool success, string message, Model.User? user)
        {
            return new GetUserResponse
            {
                Success = success,
                Message = message,
                UserValue = MapToNullableUser(user)
            };
        }
    }
}
