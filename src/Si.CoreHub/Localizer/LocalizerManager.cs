using System.Collections.Concurrent;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Si.CoreHub.Package.Core;
using Si.CoreHub.Package.Entitys;

namespace Si.CoreHub.Localizer
{
    public class LocalizerManager
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, string>>> _resourcesCache = 
            new ConcurrentDictionary<string, ConcurrentDictionary<string, Dictionary<string, string>>>();
        
        /// <summary>
        /// 加载模块的本地化资源
        /// </summary>
        /// <param name="moduleInfo">模块信息</param>
        public static void LoadModuleResources(ModuleInfo moduleInfo)
        {
            if (moduleInfo?.Assembly == null)
            {
                return;
            }

            string moduleName = moduleInfo.Assembly.GetName().Name;
            string resourcesPath = Path.Combine(Path.GetDirectoryName(moduleInfo.AssemblyPath), "Resources");
            
            if (!Directory.Exists(resourcesPath))
            {
                return;
            }

            var moduleDictionary = _resourcesCache.GetOrAdd(moduleName, _ => new ConcurrentDictionary<string, Dictionary<string, string>>());
            
            // 寻找所有JSON资源文件，约定文件名格式为：资源名.{cultureName}.json，如 Messages.zh-CN.json
            var resourceFiles = Directory.GetFiles(resourcesPath, "*.json");
            foreach (var resourceFile in resourceFiles)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(resourceFile);
                    string[] parts = fileName.Split('.');
                    
                    if (parts.Length < 2)
                    {
                        continue; // 忽略不符合命名约定的文件
                    }
                    
                    string resourceName = parts[0];
                    string cultureName = parts[1];
                    
                    string json = File.ReadAllText(resourceFile);
                    var resourceDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                    
                    if (resourceDictionary != null)
                    {
                        moduleDictionary[cultureName + "." + resourceName] = resourceDictionary;
                    }
                }
                catch (Exception ex)
                {
                    // 记录日志，但不中断处理
                    Console.WriteLine($"加载本地化资源失败: {resourceFile}, 错误: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// 获取指定模块的资源
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        /// <param name="cultureName">文化名称</param>
        /// <param name="resourceName">资源名称</param>
        /// <returns>资源字典</returns>
        public static Dictionary<string, string> GetModuleResource(string moduleName, string cultureName, string resourceName)
        {
            if (_resourcesCache.TryGetValue(moduleName, out var moduleResources))
            {
                string key = cultureName + "." + resourceName;
                if (moduleResources.TryGetValue(key, out var resource))
                {
                    return resource;
                }
                
                // 如果找不到特定文化的资源，尝试返回默认文化的资源
                string defaultKey = "." + resourceName; // 默认文化的资源没有文化前缀
                if (moduleResources.TryGetValue(defaultKey, out var defaultResource))
                {
                    return defaultResource;
                }
            }
            
            return new Dictionary<string, string>();
        }
        
        /// <summary>
        /// 清除特定模块的资源缓存
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        public static void ClearModuleCache(string moduleName)
        {
            _resourcesCache.TryRemove(moduleName, out _);
        }
        
        /// <summary>
        /// 清除所有资源缓存
        /// </summary>
        public static void ClearAllCache()
        {
            _resourcesCache.Clear();
        }
    }
} 