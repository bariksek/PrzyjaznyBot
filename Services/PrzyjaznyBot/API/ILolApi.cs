using PrzyjaznyBot.DTO.LolApi;
using System.Threading.Tasks;

namespace PrzyjaznyBot.API
{
    public interface ILolApi
    {
        Task<GetSummonerInformationsResponse> GetSummonerInformations(GetSummonerInformationsRequest request);

        Task<GetRankInformationsResponse> GetRankInformations(GetRankInformationsRequest request);
    }
}
