namespace BetService.Model
{
    public class Bet
    {
        public int Id { get; set; }

        public virtual int AuthorId { get; set; }

        public bool IsActive { get; set; }

        public bool IsStopped { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        public double Stake { get; set; }
    }
}
