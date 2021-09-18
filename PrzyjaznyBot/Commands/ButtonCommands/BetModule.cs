using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using PrzyjaznyBot.DAL;
using PrzyjaznyBot.DTO.BetRepository;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands.ButtonCommands
{
    public class BetModule
    {
        public IBetRepository BetRepository { get; }

        public BetModule(IBetRepository betRepository)
        {
            BetRepository = betRepository;
        }

        public async Task ShowAllBets(ComponentInteractionCreateEventArgs e)
        {
            var getBetsRequest = new GetBetsRequest
            {
                ShowNotActive = false
            };

            var getUsersResponse = await BetRepository.GetBets(getBetsRequest);
            var builder = new DiscordInteractionResponseBuilder();
            builder.AsEphemeral(true);

            if (!getUsersResponse.Success)
            {
                builder.WithContent(getUsersResponse.Message);
                await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
                return;
            }

            StringBuilder betsMessage = new StringBuilder();
            int position = 0;

            foreach (var bet in getUsersResponse.Bets.OrderByDescending(u => u.IsActive).ThenByDescending(u => u.DateTime))
            {
                position++;
                betsMessage.AppendLine($"{position}. BetId: {bet.Id} - {bet.Message} - Stopped: {bet.IsStopped} - Active: {bet.IsActive} - Stake: {bet.Stake:N2}");
            };

            builder.WithContent(betsMessage.ToString());
            await e.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, builder);
        }
    }
}
