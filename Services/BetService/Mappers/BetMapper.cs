using Google.Protobuf.WellKnownTypes;

namespace BetService.Mappers
{
    public static class BetMapper
    {
        public static Bet Map(this Model.Bet bet)
        {
            return new Bet
            {
                AuthorId = bet.AuthorId,
                Id = bet.Id,
                IsActive = bet.IsActive,
                IsStopped = bet.IsStopped,
                Message = bet.Message,
                Stake = bet.Stake,
                DateTime = DateTime.SpecifyKind(bet.DateTime, DateTimeKind.Utc).ToTimestamp()
            };
        }
    }
}
