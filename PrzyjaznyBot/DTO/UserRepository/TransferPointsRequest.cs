namespace PrzyjaznyBot.DTO.UserRepository
{
    public class TransferPointsRequest
    {
        public ulong SenderDiscordId { get; set; }

        public ulong ReceiverDiscordId { get; set; }

        public double Value { get; set; }
    }
}
