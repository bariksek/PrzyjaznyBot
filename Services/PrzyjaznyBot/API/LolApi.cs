using PrzyjaznyBot.DTO.LolApi;
using RiotSharp;
using RiotSharp.Misc;
using System.Net;

namespace PrzyjaznyBot.API
{
    public class LolApi : ILolApi
    {
        private RiotApi? _api = null;

        private RiotApi Api
        {
            get
            {
                if (_api == null)
                {
                    _api = RiotApi.GetDevelopmentInstance(Decrypt(Environment.GetEnvironmentVariable("RiotApiKey")).Result);
                }
                return _api;
            }
        }
        private readonly EncryptionService.EncryptionServiceClient _encryptionServiceClient;

        private readonly IDictionary<string, Region> _regionMapping = new Dictionary<string, Region>
        {
            {"euw", Region.Euw },
            {"eune", Region.Eune }
        };

        public LolApi(EncryptionService.EncryptionServiceClient encryptionServiceClient)
        {
            _encryptionServiceClient = encryptionServiceClient;
        }

        private async Task<string> Decrypt(string? cypher)
        {
            if (cypher is null)
            {
                return string.Empty;
            }

            var decryptRequest = new DecryptRequest
            {
                Cipher = cypher
            };

            var decryptResponse = await _encryptionServiceClient.DecryptAsync(decryptRequest);

            return decryptResponse.Message;
        }

        public async Task<GetSummonerInformationsResponse> GetSummonerInformations(GetSummonerInformationsRequest request)
        {
            if (!_regionMapping.TryGetValue(request.SummonerRegion, out var region))
            {
                return new GetSummonerInformationsResponse
                {
                    Success = false,
                    Message = $"Region with value: {request.SummonerRegion} not supported."
                };
            }
            try
            {
                var summoner = await Api.Summoner.GetSummonerByNameAsync(region, request.SummonerName);

                if (summoner == null)
                {
                    return new GetSummonerInformationsResponse
                    {
                        Success = false,
                        Message = $"Summoner with name: {request.SummonerName} connot be found"
                    };
                }
                
                return new GetSummonerInformationsResponse
                {
                    Success = true,
                    Summoner = summoner
                };
            }
            catch(RiotSharpException ex)
            {
                return new GetSummonerInformationsResponse
                {
                    Success = false,
                    Message = ex.HttpStatusCode == HttpStatusCode.NotFound ? $"Summoner with name: {request.SummonerName} connot be found" : $"Unexpected error with code: {ex.HResult}"
                };
            }
        }

        public async Task<GetRankInformationsResponse> GetRankInformations(GetRankInformationsRequest request)
        {
            if (!_regionMapping.TryGetValue(request.SummonerRegion, out var region))
            {
                return new GetRankInformationsResponse
                {
                    Success = false,
                    Message = $"Region with value: {request.SummonerRegion} not supported."
                };
            }
            try
            {
                var getSummonerInformationsRequest = new GetSummonerInformationsRequest
                {
                    SummonerName = request.SummonerName,
                    SummonerRegion = request.SummonerRegion
                };

                var getSummonerInformationsResponse = await GetSummonerInformations(getSummonerInformationsRequest);

                if (!getSummonerInformationsResponse.Success)
                {
                    return new GetRankInformationsResponse
                    {
                        Success = false,
                        Message = getSummonerInformationsResponse.Message
                    };
                }

                var leaguePositions = await Api.League.GetLeagueEntriesBySummonerAsync(region, getSummonerInformationsResponse.Summoner.Id);
                
                
                if(leaguePositions == null)
                {
                    return new GetRankInformationsResponse
                    {
                        Success = false,
                        Message = $"Cannot find league positions for summoner: {request.SummonerName}"
                    };
                }

                return new GetRankInformationsResponse
                {
                    Success = true,
                    LeaguePositions = leaguePositions
                };
            }
            catch(RiotSharpException ex)
            {
                return new GetRankInformationsResponse
                {
                    Success = false,
                    Message = ex.HttpStatusCode == HttpStatusCode.NotFound ? 
                        $"Rank informations for summoner: {request.SummonerName} connot be found" : 
                        $"Unexpected error with code: {ex.HResult}"
                };
            }
        }
    }
}
