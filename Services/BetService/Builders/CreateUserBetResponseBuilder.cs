using Google.Protobuf.WellKnownTypes;

namespace BetService.Builders
{
    public class CreateUserBetResponseBuilder : ResponseBuilderBase, ICreateUserBetResponseBuilder
    {
        public CreateUserBetResponse Build(bool success, string message, Model.UserBet? userBet)
        {
            return new CreateUserBetResponse
            {
                Success = success,
                Message = message,
                UserBetValue = MapToNullableUserBet(userBet)
            };
        }
    }
}
