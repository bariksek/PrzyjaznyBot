namespace PrzyjaznyBot.DTO.UserRepository
{
    public class CreateUserRequest
    {
        public ulong DiscordId { get; set; }

        public string Username { get; set; }

        public double Points { get; set; }
    }
}
