using Google.Protobuf.WellKnownTypes;

namespace UserService.Mappers
{
    public static class UserMapper
    {
        public static User Map(this Model.User user)
        {
            return new User
            {
                DiscordUserId = user.DiscordUserId,
                Id = user.Id,
                Username = user.Username,
                Points = user.Points,
                LastDailyRewardClaimDateTime = DateTime.SpecifyKind(user.LastDailyRewardClaimDateTime, DateTimeKind.Utc).ToTimestamp()
            };
        }
    }
}
