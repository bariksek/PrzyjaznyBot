using Microsoft.EntityFrameworkCore;

namespace BetService.DAL
{
    public class PostgreSqlContext : DbContext
    {
        public DbSet<Model.Bet> Bets { get; set; }
        public DbSet<Model.UserBet> UserBets { get; set; }

        private readonly EncryptionService.EncryptionServiceClient _encryptionServiceClient;
        private readonly int encryptionServiceRequestTimeoutDefault = 10000;

        public PostgreSqlContext(EncryptionService.EncryptionServiceClient encryptionServiceClient)
        {
            _encryptionServiceClient = encryptionServiceClient;
        }

        protected async override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(await GetConnectionString());

        private async Task<string> GetConnectionString()
        {
            var host = Environment.GetEnvironmentVariable("DbHost");
            var database = Environment.GetEnvironmentVariable("DbDatabase");
            var username = Environment.GetEnvironmentVariable("DbUsername");
            var encryptedPassword = Environment.GetEnvironmentVariable("DbPassword");

            var decryptRequest = new DecryptRequest
            {
                Cipher = encryptedPassword
            };

            var cancellationTokenSource = new CancellationTokenSource();
            var timeoutParseResult = int.TryParse(Environment.GetEnvironmentVariable("EncryptionServiceRequestTimeout"), out var encryptionServiceRequestTimeout);
            cancellationTokenSource.CancelAfter(timeoutParseResult ? encryptionServiceRequestTimeout : encryptionServiceRequestTimeoutDefault);

            var decryptResponse = await _encryptionServiceClient.DecryptAsync(decryptRequest, cancellationToken: cancellationTokenSource.Token);

            return $"Host={host};Database={database};Username={username};Password={decryptResponse?.Message}";
        }
    }
}
