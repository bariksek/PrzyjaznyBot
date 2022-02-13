using PrzyjaznyBot.Model;

namespace PrzyjaznyBot.DTO.BetRepository
{
    public class GetBetResponse : ResponseBase
    {
        public Bet Bet { get; set; }
    }
}
