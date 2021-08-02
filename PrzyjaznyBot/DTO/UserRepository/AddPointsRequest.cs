namespace PrzyjaznyBot.DTO.UserRepository
{
    public class AddPointsRequest
    {
        public ulong DiscordId { get; set; }

        public double Value { get; set; }
    }
}
