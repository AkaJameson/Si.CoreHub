using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Si.CoreHub.Package.Entitys;

namespace Si.CoreHub.Package.Core
{
    public static class PackLoader
    {
        internal static Dictionary<string, PackBase> _packs = new Dictionary<string, PackBase>();
        public static void LoadPack(WebApplicationBuilder builder, ModuleInfo moduleInfo)
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
            var partManager = builder.Services.AddMvcCore();
            partManager.AddApplicationPart(moduleInfo.Assembly); // 添加插件程序集
            if (!File.Exists(moduleInfo.ConfigFile))
            {
                return;
            }
            var Configurationbuilder = new ConfigurationBuilder().AddJsonFile(moduleInfo.ConfigFile, optional: true, reloadOnChange: true);
            PackConfigurationProvider.RegisterConfiguration(moduleInfo!.Assembly!.GetName().Name, Configurationbuilder.Build());
            packInstance.ConfigurationServices(builder,builder.Services);
        }

        public static void UsePack(IApplicationBuilder app, IServiceProvider serviceProvider, IEndpointRouteBuilder routes)
        {
            _packs.Values.ToList().ForEach(pack => pack.Configuration(app, routes, serviceProvider));
        }
    }
}
