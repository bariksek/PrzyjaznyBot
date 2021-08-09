using PrzyjaznyBot.Model;
using System.Collections.Generic;

namespace PrzyjaznyBot.DTO.UserRepository
{
    public class GetUsersResponse : ResponseBase
    {
        public IEnumerable<User> Users { get; set; }
    }
}
