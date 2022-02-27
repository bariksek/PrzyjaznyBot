namespace UserService.Processors
{
    public interface IUpdateUserProcessor
    {
        public Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request);
    }
}
