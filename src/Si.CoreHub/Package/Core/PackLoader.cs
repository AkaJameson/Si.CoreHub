using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Si.CoreHub.Package.Entitys;

namespace Si.CoreHub.Package.Core
{
    public static class PackLoader
    {
        internal static Dictionary<string, PackBase> _packs = new Dictionary<string, PackBase>();
        public static void LoadPack(IServiceCollection services, ModuleInfo moduleInfo)
        {
            if (moduleInfo?.Assembly == null)
            {
                return;
            }
            var packType = moduleInfo.Assembly.GetTypes().FirstOrDefault(t => typeof(PackBase).IsAssignableFrom(t));
            if (packType == null)
            {
                return;
            }
            var packInstance = (PackBase)Activator.CreateInstance(packType)!;
            if (packInstance == null)
            {
                return;
            }
            _packs.TryAdd(moduleInfo.AssemblyName, packInstance);
            services.AddControllers().AddApplicationPart(moduleInfo.Assembly);
            if (!File.Exists(moduleInfo.ConfigFile))
            {
                return;
            }
            var builder = new ConfigurationBuilder().AddJsonFile(moduleInfo.ConfigFile, optional: true, reloadOnChange: true);
            PackConfigurationProvider.LoadConfiguration(moduleInfo!.Assembly!.GetName().Name, builder.Build());
            packInstance.ConfigurationServices(services);
        }

        public static void UsePack(IApplicationBuilder app, IServiceProvider serviceProvider, IEndpointRouteBuilder routes)
        {
            _packs.Values.ToList().ForEach(pack => pack.Configuration(app, routes, serviceProvider));
        }
    }
}
