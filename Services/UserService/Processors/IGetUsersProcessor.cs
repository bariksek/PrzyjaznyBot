namespace UserService.Processors
{
    public interface IGetUsersProcessor
    {
        public Task<GetUsersResponse> GetUsers(GetUsersRequest request);
    }
}
