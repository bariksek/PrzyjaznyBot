namespace BetService.Mappers
{
    public static class UserBetMapper
    {
        public static UserBet Map(this Model.UserBet userBet)
        {
            return new UserBet
            {
                BetId = userBet.BetId,
                Id = userBet.Id,
                IsActive = userBet.IsActive,
                IsFinished = userBet.IsFinished,
                UserId = userBet.UserId,
                Condition = userBet.Condition.Map()
            };
        }
    }
}
