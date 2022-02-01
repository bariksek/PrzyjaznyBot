using PrzyjaznyBot.Model;

namespace PrzyjaznyBot.DTO.UserRepository
{
    public class CreateUserResponse : ResponseBase
    {
        public User CreatedUser { get; set; }
    }
}
