using RiotSharp.Endpoints.SummonerEndpoint;

namespace PrzyjaznyBot.DTO.LolApi
{
    public class GetSummonerInformationsResponse : ResponseBase
    {
        public Summoner Summoner { get; set; }
    }
}
