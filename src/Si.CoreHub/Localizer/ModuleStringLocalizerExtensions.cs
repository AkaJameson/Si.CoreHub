using Microsoft.Extensions.Localization;

namespace Si.CoreHub.Localizer
{
    /// <summary>
    /// 模块字符串本地化器扩展类
    /// </summary>
    public static class ModuleStringLocalizerExtensions
    {
        /// <summary>
        /// 获取特定模块和资源的本地化器
        /// </summary>
        /// <param name="factory">字符串本地化器工厂</param>
        /// <param name="moduleName">模块名称</param>
        /// <param name="resourceName">资源名称</param>
        /// <returns>字符串本地化器</returns>
        public static IStringLocalizer ForModule(this IStringLocalizerFactory factory, string moduleName, string resourceName)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (string.IsNullOrEmpty(moduleName))
            {
                throw new ArgumentNullException(nameof(moduleName));
            }

            if (string.IsNullOrEmpty(resourceName))
            {
                throw new ArgumentNullException(nameof(resourceName));
            }

            return new ModuleStringLocalizer(moduleName, resourceName);
        }

        /// <summary>
        /// 获取当前模块的本地化器
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="factory">字符串本地化器工厂</param>
        /// <returns>字符串本地化器</returns>
        public static IStringLocalizer<T> ForCurrentModule<T>(this IStringLocalizerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            return (IStringLocalizer<T>)factory.Create(typeof(T));
        }
    }
} 