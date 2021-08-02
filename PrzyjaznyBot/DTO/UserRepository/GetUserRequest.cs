namespace PrzyjaznyBot.DTO.UserRepository
{
    public class GetUserRequest
    {
        public ulong DiscordId { get; set; }

        public int UserId { get; set; }
    }
}
