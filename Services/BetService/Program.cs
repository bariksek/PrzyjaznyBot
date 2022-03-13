using BetService;
using BetService.Builders;
using BetService.DAL;
using BetService.Processors;
using BetService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddGrpc();
builder.Services.AddGrpcClient<UserService.UserService.UserServiceClient>(options =>
{
    options.Address = new Uri(Environment.GetEnvironmentVariable("UserServiceAddress") ?? "");
});
builder.Services.AddGrpcClient<EncryptionService.EncryptionServiceClient>(options =>
{
    options.Address = new Uri(Environment.GetEnvironmentVariable("EncryptionServiceAddress") ?? "");
});
builder.Services.AddDbContextFactory<PostgreSqlContext>();
builder.Services.AddTransient<ICreateBetResponseBuilder, CreateBetResponseBuilder>();
builder.Services.AddTransient<IProcessor<CreateBetRequest, CreateBetResponse>, CreateBetProcessor>();
builder.Services.AddTransient<ICreateUserBetResponseBuilder, CreateUserBetResponseBuilder>();
builder.Services.AddTransient<IProcessor<CreateUserBetRequest, CreateUserBetResponse> , CreateUserBetProcessor>();
builder.Services.AddTransient<IGetBetResponseBuilder, GetBetResponseBuilder>();
builder.Services.AddTransient<IProcessor<GetBetRequest, GetBetResponse> , GetBetProcessor>();
builder.Services.AddTransient<IGetBetsResponseBuilder, GetBetsResponseBuilder>();
builder.Services.AddTransient<IProcessor<GetBetsRequest, GetBetsResponse> , GetBetsProcessor>();
builder.Services.AddTransient<IGetUserBetsResponseBuilder, GetUserBetsResponseBuilder>();
builder.Services.AddTransient<IProcessor<GetUserBetsRequest, GetUserBetsResponse> , GetUserBetsProcessor>();
builder.Services.AddTransient<IStopBetResponseBuilder, StopBetResponseBuilder>();
builder.Services.AddTransient<IProcessor<StopBetRequest, StopBetResponse> , StopBetProcessor>();
builder.Services.AddTransient<IFinishBetResponseBuilder, FinishBetResponseBuilder>();
builder.Services.AddTransient<IProcessor<FinishBetRequest, FinishBetResponse> , FinishBetProcessor>();

var app = builder.Build();

var dbContextFactory = app.Services.GetService<IDbContextFactory<PostgreSqlContext>>();
using (var dbContext = dbContextFactory?.CreateDbContext())
{
    dbContext?.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.MapGrpcService<BetServiceImplementation>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
