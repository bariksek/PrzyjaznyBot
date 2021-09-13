using Microsoft.Extensions.Configuration;
using PrzyjaznyBot.Config;
using System;

namespace PrzyjaznyBot.Common
{
    public interface IConfigFetcher
    {
        AppConfig GetConfig();
    }

    public class ConfigFetcher : IConfigFetcher
    {
        public AppConfig GetConfig()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json").Build();

            var section = config.GetSection(nameof(AppConfig));
            var appConfig = section.Get<AppConfig>();

            return appConfig;
        }
    }
}
