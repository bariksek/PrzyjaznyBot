using Microsoft.EntityFrameworkCore;

namespace UserService.DAL
{
    public class PostgreSqlContext : DbContext
    {
        public DbSet<Model.User> Users { get; set; }

        private readonly EncryptionService.EncryptionServiceClient _encryptionServiceClient;
        private readonly int encryptionServiceRequestTimeoutDefault = 10000;

        public PostgreSqlContext(EncryptionService.EncryptionServiceClient encryptionServiceClient)
        {
            _encryptionServiceClient = encryptionServiceClient;
        }

        protected async override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(await GetConnectionString());

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Model.User>()
                .HasIndex(u => u.DiscordUserId)
                .IsUnique();
        }

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
