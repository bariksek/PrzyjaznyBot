using PrzyjaznyBot.Model;
using System.Collections.Generic;

namespace PrzyjaznyBot.DTO.BetRepository
{
    public class GetBetInfoResponse : ResponseBase
    {
        public IEnumerable<UserBet> UserBets { get; set; }
    }
}
