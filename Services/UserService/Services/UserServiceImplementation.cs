using Grpc.Core;
using UserService.Processors;

namespace UserService.Services
{
    public class UserServiceImplementation : UserService.UserServiceBase
    {
        private readonly ILogger<UserServiceImplementation> _logger;
        private readonly ICreateUserProcessor _createUserProcessor;
        private readonly IGetUserProcessor _getUserProcessor;
        private readonly IGetUsersProcessor _getUsersProcessor;
        private readonly IRemoveUserProcessor _removeUserProcessor;
        private readonly IUpdateUserProcessor _updateUserProcessor;

        public UserServiceImplementation(ILogger<UserServiceImplementation> logger,
            ICreateUserProcessor createUserProcessor,
            IGetUserProcessor getUserProcessor,
            IGetUsersProcessor getUsersProcessor,
            IRemoveUserProcessor removeUserProcessor,
            IUpdateUserProcessor updateUserProcessor)
        {
            _logger = logger;
            _createUserProcessor = createUserProcessor;
            _getUserProcessor = getUserProcessor;
            _getUsersProcessor = getUsersProcessor;
            _removeUserProcessor = removeUserProcessor;
            _updateUserProcessor = updateUserProcessor;
        }

        public override async Task<CreateUserResponse> CreateUser(CreateUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Create user request handling started");

            return await _createUserProcessor.CreateUser(request);
        }

        public override async Task<RemoveUserResponse> RemoveUser(RemoveUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Remove user request handling started");

            return await _removeUserProcessor.RemoveUser(request);
        }

        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Update user request handling started");

            return await _updateUserProcessor.UpdateUser(request);
        }

        public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Get user request handling started");

            return await _getUserProcessor.GetUser(request);
        }

        public override async Task<GetUsersResponse> GetUsers(GetUsersRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Get users request handling started");

            return await _getUsersProcessor.GetUsers(request);
        }
    }
}