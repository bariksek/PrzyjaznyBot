namespace BetService.Mappers
{
    public static class ConditionMapper
    {
        public static Condition Map(this Common.Condition condition)
        {
            return condition switch
            {
                Common.Condition.Yes => Condition.Yes,
                Common.Condition.No => Condition.No,
                _ => Condition.No,
            };
        }

        public static Common.Condition Map(this Condition condition)
        {
            return condition switch
            {
                Condition.Yes => Common.Condition.Yes,
                Condition.No => Common.Condition.No,
                _ => Common.Condition.No,
            };
        }
    }
}
