using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Si.CoreHub.Localizer
{
    /// <summary>
    /// 模块字符串本地化器，实现IStringLocalizer接口
    /// </summary>
    public class ModuleStringLocalizer : IStringLocalizer
    {
        private readonly string _moduleName;
        private readonly string _resourceName;
        
        public ModuleStringLocalizer(string moduleName, string resourceName)
        {
            _moduleName = moduleName;
            _resourceName = resourceName;
        }
        
        public LocalizedString this[string name]
        {
            get
            {
                string value = GetString(name, CultureInfo.CurrentUICulture);
                bool notFound = value == null;
                return new LocalizedString(name, notFound ? name : value, notFound);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                string format = GetString(name, CultureInfo.CurrentUICulture);
                bool notFound = format == null;
                string value = notFound ? name : string.Format(format, arguments);
                return new LocalizedString(name, value, notFound);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            var resource = LocalizerManager.GetModuleResource(_moduleName, CultureInfo.CurrentUICulture.Name, _resourceName);
            return resource.Select(r => new LocalizedString(r.Key, r.Value, false));
        }
        
        private string GetString(string name, CultureInfo culture)
        {
            try
            {
                var resource = LocalizerManager.GetModuleResource(_moduleName, culture.Name, _resourceName);
                if (resource.TryGetValue(name, out string value))
                {
                    return value;
                }

                // 如果当前文化找不到，尝试回退到父文化
                if (culture.Parent != null && culture.Parent != CultureInfo.InvariantCulture)
                {
                    return GetString(name, culture.Parent);
                }
                
                // 找不到翻译时记录日志
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
} 