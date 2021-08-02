/*using DSharpPlus.Entities;
using PrzyjaznyBot.Common;
using PrzyjaznyBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrzyjaznyBot.DAL
{
    public class BetRepository
    {

        private readonly UserRepository UserRepository;

        public BetRepository()
        {
            UserRepository = new UserRepository();
        }

        async public Task<Bet> CreateBet(User user, string message)
        {
            Bet bet = new Bet
            {
                AuthorId = user.Id,
                IsActive = true,
                Message = message
            };

            using var dbContext = new MyDbContext();
            dbContext.Bets.Add(bet);
            return await dbContext.SaveChangesAsync() > 0 ? bet : null;
        }

        async public Task<UserBet> CreateUserBet(DiscordMember member, int betId, string condition, double value)
        {
            var user = UserRepository.GetUser(member);

            if (user == null)
            {
                return null;
            }

            var userBet = new UserBet
            {
                UserId = user.Id,
                BetId = betId,
                Condition = (Condition)Enum.Parse(typeof(Condition), condition),
                Value = value
            };

            using var dbContext = new MyDbContext();
            dbContext.Add(userBet);

            var result = await UserRepository.SubstractPoints(user.Id, value);

            if (!result)
            {
                return null;
            }

            return await dbContext.SaveChangesAsync() > 0 ? userBet : null;
        }

        public Bet GetBet(int betId)
        {
            using var dbContext = new MyDbContext();
            return dbContext.Bets.FirstOrDefault(b => b.Id == betId);
        }

        async public Task<bool> FinishBet(int betId, DiscordMember member, string condition)
        {
            using var dbContext = new MyDbContext();
            var bet = dbContext.Bets.FirstOrDefault(b => b.Id == betId && b.IsActive == true);

            if (bet == null)
            {
                return false;
            }

            User author = UserRepository.GetUser(member);

            if (author == null || author.DiscordUserId != member.Id)
            {
                return false;
            }

            bet.IsActive = false;

            var userBets = dbContext.UserBets.Where(ub => ub.BetId == bet.Id);
            var successUserBets = userBets.Where(v => v.Condition == (Condition)Enum.Parse(typeof(Condition), condition));
            var failUserBets = userBets.Where(v => v.Condition != (Condition)Enum.Parse(typeof(Condition), condition));

            if (successUserBets.Count() == 0)
            {
                return await dbContext.SaveChangesAsync() > 0;
            }

            var prizePool = failUserBets.Sum(f => f.Value);
            var prizePerUser = prizePool / successUserBets.Count();

            var userUpdateTasks = new List<Task<bool>>();
            foreach (var userBet in successUserBets)
            {
                userUpdateTasks.Add(UserRepository.AddPoints(userBet.UserId, prizePerUser));
            }

            await Task.WhenAll(userUpdateTasks);

            return await dbContext.SaveChangesAsync() > 0;
        }
    }
}
*/