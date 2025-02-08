using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Si.CoreHub.EventBus;
using Si.CoreHub.EventBus.Entitys;
using Si.CoreHub.Logs;
using Si.CoreHub.MemoryCache;
using Si.CoreHub.Package.Abstraction;
using Si.CoreHub.Package.Core;
using Si.CoreHub.Package.Entitys;
using System.Net;

namespace Si.CoreHub.Extension
{
    public static class WebApplicationExtensions
    {
        /// <summary>
        /// Kestrel配置
        /// </summary>
        /// <param name="builder"></param>
        public static void UseKestrel(this WebApplicationBuilder builder)
        {
            builder.WebHost.ConfigureKestrel(options =>
            {
                var kestrelConfig = builder.Configuration.GetSection("Kestrel");
                var ipAddr = IPAddress.Parse(kestrelConfig.GetValue<string>("Url") ?? "0.0.0.0");
                //https配置
                var httpPort = kestrelConfig.GetValue<int>("Configuration:Http:Port");
                options.Listen(ipAddr, httpPort);
                var httpsPort = kestrelConfig.GetValue<int>("Configuration:Https:Port");
                var certificatePath = kestrelConfig.GetValue<string>("Configuration:Https:Certificate:Path");
                var certificatePassword = kestrelConfig.GetValue<string>("Configuration:Https:Certificate:Password");
                if (!string.IsNullOrEmpty(certificatePath) && File.Exists(certificatePath))
                {
                    try
                    {
                        options.Listen(ipAddr, httpsPort, listenOptions =>
                        {
                            listenOptions.UseHttps(certificatePath, certificatePassword, httpsOptions =>
                            {
                                httpsOptions.AllowAnyClientCertificate();
                            });
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Https配置失败");
                    }
                }
            });
        }
        /// <summary>
        /// 注册缓存服务
        /// </summary>
        /// <param name="services"></param>
        public static void UseMemoryCache(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton(typeof(ICacheService), typeof(MemoryCacheService));
        }
        /// <summary>
        /// 添加包
        /// </summary>
        /// <param name="services"></param>
        /// <param name="options"></param>
        public static void AddPackages(this WebApplicationBuilder builder, Action<PackOptions> options)
        {
            var packOptions = new PackOptions();
            options(packOptions);
            PackageManager finder = new PackageManager(packOptions);
            builder.Services.AddScoped(typeof(IPackConfiguration<>), typeof(PackConfiguration<>));
            foreach (var package in finder.GetPacks())
            {
                PackLoader.LoadPack(builder, package);
            }
            LogCenter.Write2Log(Loglevel.Info, $"加载包完成,{finder.GetPacks().Select(x => new
            {
                x.AssemblyName,
                x.AssemblyPath,
                x.ConfigFile
            }).ToList().ToJson()}");
        }
        /// <summary>
        /// 注册包
        /// </summary>
        /// <param name="app"></param>
        /// <param name="routes"></param>
        /// <param name="provider"></param>
        public static void UsePackages(this IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider provider)
        {
            PackLoader.UsePack(app, provider, routes);
        }
        /// <summary>
        /// 注册内存事件总线
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services)
        {
            services.AddSingleton<EventBus.Abstraction.IEventBus, InMemoryEventBus>();
            return services;
        }
        /// <summary>
        /// 注册RabbitMQ事件总线
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services, Action<RabbitMqOptions> configureOptions)
        {
            var options = new RabbitMqOptions();
            configureOptions(options);
            services.AddSingleton(options);
            services.AddSingleton<EventBus.Abstraction.IEventBus, RabbitMqEventBus>();
            return services;

        }

    }
}
