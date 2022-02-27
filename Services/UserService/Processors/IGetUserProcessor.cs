namespace UserService.Processors
{
    public interface IGetUserProcessor
    {
        public Task<GetUserResponse> GetUser(GetUserRequest request);
    }
}
