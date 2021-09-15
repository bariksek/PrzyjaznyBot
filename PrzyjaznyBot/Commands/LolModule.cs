using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using PrzyjaznyBot.API;
using PrzyjaznyBot.DTO.LolApi;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrzyjaznyBot.Commands
{
    public class LolModule : BaseCommandModule
    {
        private readonly ILolApi _lolApi;

        public LolModule(ILolApi lolApi)
        {
            _lolApi = lolApi;
        }

        [Command("summoner")]
        [Description("Command for getting summoner informations.")]
        public async Task SummonerCommand(CommandContext ctx, [Description("Summoner name")] string summonerName, [Description("Summoner region")] string summonerRegion)
        {
            var getSummonerInformationsRequest = new GetSummonerInformationsRequest
            {
                SummonerName = summonerName,
                SummonerRegion = summonerRegion
            };

            var getSummonerInformationsResponse = await _lolApi.GetSummonerInformations(getSummonerInformationsRequest);

            if (!getSummonerInformationsResponse.Success)
            {
                await ctx.RespondAsync(getSummonerInformationsResponse.Message);
                return;
            }

            var summonerMessage = new StringBuilder();

            summonerMessage.AppendLine($"Summoner with name: {summonerName} found!");
            summonerMessage.AppendLine($"Summoner level is: {getSummonerInformationsResponse.Summoner.Level}.");
            summonerMessage.AppendLine($"Summoner id is: {getSummonerInformationsResponse.Summoner.Id}.");
            summonerMessage.AppendLine($"Summoner account id is: {getSummonerInformationsResponse.Summoner.AccountId}.");

            await ctx.RespondAsync(summonerMessage.ToString());
        }

        [Command("rank")]
        [Description("Command for getting summoner rank informations.")]
        public async Task SummonerRank(CommandContext ctx, [Description("Summoner name")] string summonerName, [Description("Summoner region")] string summonerRegion)
        {
            var getRankInformationsRequest = new GetRankInformationsRequest
            {
                SummonerName = summonerName,
                SummonerRegion = summonerRegion
            };

            var getRankInformationsResponse = await _lolApi.GetRankInformations(getRankInformationsRequest);

            if (!getRankInformationsResponse.Success)
            {
                await ctx.RespondAsync(getRankInformationsResponse.Message);
                return;
            }

            var soloRankedEntry = getRankInformationsResponse.LeaguePositions.FirstOrDefault(entry => entry.QueueType == "RANKED_SOLO_5x5");

            var rankMessage = new StringBuilder();

            rankMessage.AppendLine($"Current rank for {soloRankedEntry.SummonerName}:");
            rankMessage.AppendLine($"{soloRankedEntry.Rank} {soloRankedEntry.Tier} {soloRankedEntry.LeaguePoints} LP");
            rankMessage.AppendLine($"Wins: {soloRankedEntry.Wins}");
            rankMessage.AppendLine($"Losses: {soloRankedEntry.Losses}");

            await ctx.RespondAsync(rankMessage.ToString());
        }
    }
}
