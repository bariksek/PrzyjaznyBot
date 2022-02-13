using PrzyjaznyBot.Model;

namespace PrzyjaznyBot.DTO.UserRepository
{
    public class GetUserResponse : ResponseBase
    {
        public User User { get; set; }
    }
}
