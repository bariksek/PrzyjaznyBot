namespace PrzyjaznyBot.DTO.UserRepository
{
    public class TransferPointsRequest
    {
        public ulong SenderDiscordId { get; set; }

        public int SenderUserId { get; set; }

        public ulong ReceiverDiscordId { get; set; }

        public int ReceiverUserId { get; set; }

        public double Value { get; set; }
    }
}
