using PrzyjaznyBot.Model;

namespace PrzyjaznyBot.DTO.BetRepository
{
    public class CreateBetResponse : ResponseBase
    {
        public Bet Bet { get; set; }
    }
}
