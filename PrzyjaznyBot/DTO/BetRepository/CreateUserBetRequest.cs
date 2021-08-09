namespace PrzyjaznyBot.DTO.BetRepository
{
    public class CreateUserBetRequest
    {
        public ulong DiscordId { get; set; }

        public int BetId { get; set; }

        public string Condition { get; set; }
    }
}
