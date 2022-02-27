namespace UserService.Processors
{
    public interface IRemoveUserProcessor
    {
        public Task<RemoveUserResponse> RemoveUser(RemoveUserRequest request);
    }
}
