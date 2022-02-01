using PrzyjaznyBot.Model;

namespace PrzyjaznyBot.DTO.BetRepository
{
    public class CreateUserBetResponse : ResponseBase
    {
        public UserBet UserBet { get; set; }
    }
}
