using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;

namespace UserService.DAL
{
    public class PostgreSqlContext : DbContext
    {
        public DbSet<Model.User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseNpgsql(GetConnectionString());

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Model.User>()
                .HasIndex(u => u.DiscordUserId)
                .IsUnique();
        }

        private static string GetConnectionString()
        {
            var host = Environment.GetEnvironmentVariable("DbHost");
            var database = Environment.GetEnvironmentVariable("DbDatabase");
            var username = Environment.GetEnvironmentVariable("DbUsername");
            var encryptedPassword = Environment.GetEnvironmentVariable("DbPassword");
            var encryptionServiceAddress = Environment.GetEnvironmentVariable("EncryptionServiceAddress");
            if (encryptionServiceAddress is null)
            {
                throw new ArgumentNullException("Cannot find EncryptionService address");
            }
            using var channel = GrpcChannel.ForAddress(encryptionServiceAddress);
            var encryptionServiceClient = new EncryptionService.EncryptionServiceClient(channel);
            var decryptRequest = new DecryptRequest
            {
                Cipher = encryptedPassword
            };
            var decryptResponse = encryptionServiceClient.Decrypt(decryptRequest);

            return $"Host={host};Database={database};Username={username};Password={decryptResponse?.Message}";
        }
    }
}
