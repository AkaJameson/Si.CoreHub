using Microsoft.Extensions.Configuration;

namespace Si.CoreHub.Utility
{
    public static class ServiceLocator
    {
        private static IServiceProvider _serviceProvider;
        private static IConfiguration _configuration;
        public static T GetService<T>()
        {
            return (T)_serviceProvider?.GetService(typeof(T));
        }
        public static IServiceProvider GetServiceProvider
        {
            get => _serviceProvider ?? throw new InvalidOperationException("ServiceProvider is not set.");
            set => _serviceProvider = value;
        }
        public static IConfiguration Configuration
        {
            get => _configuration ?? throw new InvalidOperationException("Configuration is not set.");
            set => _configuration = value;
        }
    }
}
