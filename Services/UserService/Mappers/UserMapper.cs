using Google.Protobuf.WellKnownTypes;

namespace UserService.Mappers
{
    public static class UserMapper
    {
        public static Model.User Map(this User user)
        {
            return new Model.User
            {
                DiscordUserId = user.DiscordUserId,
                Username = user.Username,
                Id = user.Id,
                Points = user.Points,
                LastDailyRewardClaimDateTime = user.LastDailyRewardClaimDateTime.ToDateTime()
            };
        }

        public static User Map(this Model.User userDto)
        {
            return new User
            {
                DiscordUserId = userDto.DiscordUserId,
                Id = userDto.Id,
                Username = userDto.Username,
                Points = userDto.Points,
                LastDailyRewardClaimDateTime = userDto.LastDailyRewardClaimDateTime.ToTimestamp()
            };
        }
    }
}
