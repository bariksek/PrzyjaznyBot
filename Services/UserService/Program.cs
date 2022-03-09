using Microsoft.EntityFrameworkCore;
using UserService;
using UserService.Builders;
using UserService.DAL;
using UserService.Processors;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<EncryptionService.EncryptionServiceClient>(options =>
{
    options.Address = new Uri(Environment.GetEnvironmentVariable("EncryptionServiceAddress") ?? "");
});
builder.Services.AddDbContextFactory<PostgreSqlContext>();
builder.Services.AddTransient<IProcessor<CreateUserRequest, CreateUserResponse>, CreateUserProcessor>();
builder.Services.AddTransient<IProcessor<GetUserRequest, GetUserResponse>, GetUserProcessor>();
builder.Services.AddTransient<IProcessor<GetUsersRequest, GetUsersResponse>, GetUsersProcessor>();
builder.Services.AddTransient<IProcessor<RemoveUserRequest, RemoveUserResponse>, RemoveUserProcessor>();
builder.Services.AddTransient<IProcessor<UpdateUserRequest, UpdateUserResponse>, UpdateUserProcessor>();
builder.Services.AddTransient<ICreateUserResponseBuilder, CreateUserResponseBuilder>();
builder.Services.AddTransient<IUpdateUserResponseBuilder, UpdateUserResponseBuilder>();
builder.Services.AddTransient<IRemoveUserResponseBuilder, RemoveUserResponseBuilder>();
builder.Services.AddTransient<IGetUserResponseBuilder, GetUserResponseBuilder>();
builder.Services.AddTransient<IGetUsersResponseBuilder, GetUsersResponseBuilder>();

var app = builder.Build();

var dbContextFactory = app.Services.GetService<IDbContextFactory<PostgreSqlContext>>();
using (var dbContext = dbContextFactory?.CreateDbContext())
{
    dbContext?.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<UserServiceImplementation>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
