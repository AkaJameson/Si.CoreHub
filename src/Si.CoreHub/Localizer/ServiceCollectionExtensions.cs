using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Si.CoreHub.Package.Core;

namespace Si.CoreHub.Localizer
{
    /// <summary>
    /// 服务集合扩展类
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 添加模块化JSON本地化服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="watchResourceFiles">是否监视资源文件变更</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddModuleJsonLocalization(this IServiceCollection services, bool watchResourceFiles = false)
        {
            // 注册本地化服务
            services.AddSingleton<IStringLocalizerFactory, ModuleStringLocalizerFactory>();
            services.AddSingleton(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));
            services.AddSingleton<IStringLocalizer, ModuleStringLocalizer>();
            
            // 如果启用了资源文件监视，则注册监听器
            if (watchResourceFiles)
            {
                services.AddSingleton<LocalizationPackChangeListener>();
            }
            
            return services;
        }
        
        /// <summary>
        /// 使用模块化JSON本地化
        /// </summary>
        /// <param name="app">应用程序构建器</param>
        /// <param name="watchResourceFiles">是否监视资源文件变更</param>
        /// <returns>应用程序构建器</returns>
        public static IApplicationBuilder UseModuleJsonLocalization(this IApplicationBuilder app, bool watchResourceFiles = false)
        {
            // 加载所有模块的本地化资源
            var packManager = app.ApplicationServices.GetService<PackageManager>();
            if (packManager != null)
            {
                var modules = packManager.GetPacks();
                foreach (var module in modules)
                {
                    LocalizerManager.LoadModuleResources(module);
                    
                    // 如果启用了资源文件监视，则启动监听
                    if (watchResourceFiles)
                    {
                        var changeListener = app.ApplicationServices.GetService<LocalizationPackChangeListener>();
                        changeListener?.StartWatching(module);
                    }
                }
            }
            
            return app;
        }
    }
} 