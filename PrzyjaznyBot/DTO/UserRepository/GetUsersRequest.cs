using System.Collections.Generic;

namespace PrzyjaznyBot.DTO.UserRepository
{
    public class GetUsersRequest
    {
        public IEnumerable<ulong> DiscordIds { get; set; }
    }
}
