namespace UserService.Processors
{
    public interface ICreateUserProcessor
    {
        public Task<CreateUserResponse> CreateUser(CreateUserRequest request);
    }
}
