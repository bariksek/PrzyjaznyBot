using BetService.Mappers;
using Google.Protobuf.WellKnownTypes;

namespace BetService.Builders
{
    public abstract class ResponseBuilderBase
    {
        protected NullableBet MapToNullableBet(Model.Bet? bet)
        {
            if (bet == null)
            {
                return new NullableBet
                {
                    Null = NullValue.NullValue
                };
            }

            return new NullableBet
            {
                Bet = bet.Map()
            };
        }

        protected NullableUserBet MapToNullableUserBet(Model.UserBet? userBet)
        {
            if (userBet == null)
            {
                return new NullableUserBet
                {
                    Null = NullValue.NullValue
                };
            }

            return new NullableUserBet
            {
                UserBet = userBet.Map()
            };
        }
    }
}
