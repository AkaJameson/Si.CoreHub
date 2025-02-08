using Microsoft.Extensions.Configuration;
using System.Collections.Concurrent;

namespace Si.CoreHub.Package.Core
{
    public class PackConfigurationProvider
    {
        private static readonly ConcurrentDictionary<string, IConfiguration> _packConfigurations = new ConcurrentDictionary<string, IConfiguration>();
        public static IConfiguration GetConfiguration(string packName)
        {
            _packConfigurations.TryGetValue(packName, out IConfiguration configuration);
            return configuration;
        }

        public static void RegisterConfiguration(string packName, IConfiguration configuration)
        {
            _packConfigurations.TryAdd(packName, configuration);
        }
    }

}
