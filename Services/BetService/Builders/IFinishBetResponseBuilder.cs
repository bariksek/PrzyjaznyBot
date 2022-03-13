namespace BetService.Builders
{
    public interface IFinishBetResponseBuilder
    {
        public FinishBetResponse Build(bool success, string message);
    }
}
