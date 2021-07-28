using DSharpPlus.Entities;
using PrzyjaznyBot.Model;
using System.Linq;
using System.Threading.Tasks;

namespace PrzyjaznyBot.DAL
{
    public class UserDA
    {
        public Task<User> CreateNewUser(MyDbContext dbContext, DiscordMember member)
        {
            User user = new User
            {
                DiscordUserId = member.Id,
                Username = member.Username,
                Value = 100
            };

            dbContext.Users.AddAsync(user);
            var result = dbContext.SaveChangesAsync();

            return Task.FromResult(user);
        }

        public Task<User> GetUser(MyDbContext dbContext, DiscordMember member)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.DiscordUserId == member.Id);

            return Task.FromResult(user);
        }
    }
}
