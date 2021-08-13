using Microsoft.Extensions.Configuration;
using PrzyjaznyBot.Config;
using PrzyjaznyBot.DTO.LolApi;
using RiotSharp;
using RiotSharp.Misc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace PrzyjaznyBot.API
{
    public class LolApi
    {
        private readonly RiotApi _api;

        private readonly IDictionary<string, Region> _regionMapping = new Dictionary<string, Region>
        {
            {"euw", Region.Euw },
            {"eune", Region.Eune }
        };

        public LolApi()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();

            var section = config.GetSection(nameof(AppConfig));
            var appConfig = section.Get<AppConfig>();

            _api = RiotApi.GetDevelopmentInstance(appConfig.RiotApiKey);
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
                var summoner = await _api.Summoner.GetSummonerByNameAsync(region, request.SummonerName);

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

                var leaguePositions = await _api.League.GetLeagueEntriesBySummonerAsync(region, getSummonerInformationsResponse.Summoner.Id);
                
                
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
