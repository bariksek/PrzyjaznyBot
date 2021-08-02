namespace PrzyjaznyBot.DTO.BetRepository
{
    public class CreateBetRequest
    {
        public ulong DiscordId { get; set; }

        public string Message { get; set; }
    }
}
