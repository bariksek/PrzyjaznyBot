namespace BetService.Builders
{
    public interface IStopBetResponseBuilder
    {
        public StopBetResponse Build(bool success, string message);
    }
}
