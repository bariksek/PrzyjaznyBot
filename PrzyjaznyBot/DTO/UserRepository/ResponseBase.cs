namespace PrzyjaznyBot.DTO.UserRepository
{
    public class ResponseBase
    {
        public bool Success { get; set; }

        public string Message { get; set; }

        public int ResourceId { get; set; }
    }
}
