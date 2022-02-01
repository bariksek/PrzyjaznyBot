using PrzyjaznyBot.Common;

namespace PrzyjaznyBot.Model
{
    public class UserBet
    {
        public int Id { get; set; }

        public virtual int UserId { get; set; }

        public virtual int BetId { get; set; }

        public Condition Condition { get; set; }

        public bool IsFinished { get; set; }

        public bool IsActive { get; set; }
    }
}
