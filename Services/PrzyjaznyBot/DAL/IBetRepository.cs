using PrzyjaznyBot.DTO.BetRepository;
using System.Threading.Tasks;

namespace PrzyjaznyBot.DAL
{
    public interface IBetRepository
    {
        Task<CreateBetResponse> CreateBet(CreateBetRequest request);

        Task<CreateUserBetResponse> CreateUserBet(CreateUserBetRequest request);

        GetBetResponse GetBet(GetBetRequest request);

        GetBetInfoResponse GetUserBets(GetBetInfoRequest request);

        Task<FinishBetResponse> FinishBet(FinishBetRequest request);

        Task<StopBetResponse> StopBet(StopBetRequest request);

        Task<GetBetsResponse> GetBets(GetBetsRequest request);
    }
}
