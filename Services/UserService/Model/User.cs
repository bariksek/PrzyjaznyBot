namespace UserService.Model
{
    public class User
    {
        public int Id { get; set; }

        public ulong DiscordUserId { get; set; }

        public string? Username { get; set; }

        public double Points { get; set; }

        public DateTime LastDailyRewardClaimDateTime { get; set; }
    }
}
