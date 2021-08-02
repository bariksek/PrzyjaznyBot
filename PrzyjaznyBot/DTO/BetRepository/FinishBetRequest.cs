namespace PrzyjaznyBot.DTO.BetRepository
{
    public class FinishBetRequest
    {
        public int BetId { get; set; }

        public ulong DiscordId { get; set; }

        public string Condition { get; set; }
    }
}
