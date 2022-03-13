using Grpc.Core;
using UserService.Processors;

namespace UserService.Services
{
    public class UserServiceImplementation : UserService.UserServiceBase
    {
        private readonly ILogger<UserServiceImplementation> _logger;
        private readonly IProcessor<CreateUserRequest, CreateUserResponse> _createUserProcessor;
        private readonly IProcessor<GetUserRequest, GetUserResponse> _getUserProcessor;
        private readonly IProcessor<GetUsersRequest, GetUsersResponse> _getUsersProcessor;
        private readonly IProcessor<RemoveUserRequest, RemoveUserResponse> _removeUserProcessor;
        private readonly IProcessor<UpdateUserRequest, UpdateUserResponse> _updateUserProcessor;

        public UserServiceImplementation(ILogger<UserServiceImplementation> logger,
            IProcessor<CreateUserRequest, CreateUserResponse> createUserProcessor,
            IProcessor<GetUserRequest, GetUserResponse> getUserProcessor,
            IProcessor<GetUsersRequest, GetUsersResponse> getUsersProcessor,
            IProcessor<RemoveUserRequest, RemoveUserResponse> removeUserProcessor,
            IProcessor<UpdateUserRequest, UpdateUserResponse> updateUserProcessor)
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
            _logger.LogInformation("CreateUser request handling started");

            return await _createUserProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<RemoveUserResponse> RemoveUser(RemoveUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("RemoveUser request handling started");

            return await _removeUserProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("UpdateUser request handling started");

            return await _updateUserProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetUser request handling started");

            return await _getUserProcessor.Process(request, context.CancellationToken);
        }

        public override async Task<GetUsersResponse> GetUsers(GetUsersRequest request, ServerCallContext context)
        {
            _logger.LogInformation("GetUsers request handling started");

            return await _getUsersProcessor.Process(request, context.CancellationToken);
        }
    }
}