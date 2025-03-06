using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Si.CoreHub.Localizer
{
    /// <summary>
    /// 模块字符串本地化器工厂，实现IStringLocalizerFactory接口
    /// </summary>
    public class ModuleStringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            if (resourceSource == null)
            {
                throw new ArgumentNullException(nameof(resourceSource));
            }

            var assembly = resourceSource.Assembly;
            var moduleName = assembly.GetName().Name;
            var resourceName = resourceSource.Name;

            return new ModuleStringLocalizer(moduleName, resourceName);
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (string.IsNullOrEmpty(baseName))
            {
                throw new ArgumentNullException(nameof(baseName));
            }

            if (string.IsNullOrEmpty(location))
            {
                throw new ArgumentNullException(nameof(location));
            }

            return new ModuleStringLocalizer(location, baseName);
        }
    }
} 