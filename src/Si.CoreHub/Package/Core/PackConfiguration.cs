using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Si.CoreHub.Package.Abstraction;

namespace Si.CoreHub.Package.Core
{
    public class PackConfiguration<T> : IPackConfiguration<T>
    {
        private readonly IConfiguration _innerConfiguration;

        public PackConfiguration()
        {
            var name = typeof(T).Assembly.GetName().Name;
            _innerConfiguration = PackLoader.GetConfiguration(name!);
        }

        public string this[string key]
        {
            get => _innerConfiguration[key];
            set => _innerConfiguration[key] = value;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return _innerConfiguration.GetChildren();
        }

        public IChangeToken GetReloadToken()
        {
            return _innerConfiguration.GetReloadToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            return _innerConfiguration.GetSection(key);
        }
    }
}
