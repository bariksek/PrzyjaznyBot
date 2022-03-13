using Google.Protobuf.WellKnownTypes;
using UserService.Mappers;

namespace UserService.Builders
{
    public abstract class ResponseBuilderBase
    {
        protected static NullableUser MapToNullableUser(Model.User? user)
        {
            if (user == null)
            {
                return new NullableUser
                {
                    Null = NullValue.NullValue
                };
            }

            return new NullableUser
            {
                User = user.Map()
            };
        }
    }
}
