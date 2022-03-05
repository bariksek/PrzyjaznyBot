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
                Condition = MapCondition(userBet.Condition)
            };
        }

        private static Condition MapCondition(Common.Condition condition)
        {
            return condition switch
            {
                Common.Condition.Yes => Condition.Yes,
                Common.Condition.No => Condition.No,
                _ => Condition.No,
            };
        }
    }
}
