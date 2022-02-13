using PrzyjaznyBot.DTO.UserRepository;
using System.Threading.Tasks;

namespace PrzyjaznyBot.DAL
{
    public interface IUserRepository
    {
        Task<CreateUserResponse> CreateNewUser(CreateUserRequest request);

        GetUserResponse GetUser(GetUserRequest request);

        Task<TransferPointsResponse> TransferPoints(TransferPointsRequest request);

        Task<AddPointsResponse> AddPoints(AddPointsRequest request);

        Task<SubstractPointsResponse> SubstractPoints(SubstractPointsRequest request);

        GetUsersResponse GetUsers(GetUsersRequest request);
    }
}
