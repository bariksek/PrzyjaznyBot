using RiotSharp.Endpoints.LeagueEndpoint;
using System.Collections.Generic;

namespace PrzyjaznyBot.DTO.LolApi
{
    public class GetRankInformationsResponse : ResponseBase
    {
        public IEnumerable<LeagueEntry> LeaguePositions { get; set; }
    }
}
