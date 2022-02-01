using PrzyjaznyBot.Model;
using System.Collections.Generic;

namespace PrzyjaznyBot.DTO.BetRepository
{
    public class GetBetsResponse : ResponseBase
    {
        public IEnumerable<Bet> Bets { get; set; }
    }
}
