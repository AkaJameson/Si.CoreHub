using Si.CoreHub.Package.Entitys;
using System.IO;

namespace Si.CoreHub.Localizer
{
    /// <summary>
    /// 本地化包变更监听器，监控资源文件变更自动重新加载
    /// </summary>
    public class LocalizationPackChangeListener
    {
        private readonly Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();
        
        /// <summary>
        /// 开始监听指定模块的资源文件变更
        /// </summary>
        /// <param name="moduleInfo">模块信息</param>
        public void StartWatching(ModuleInfo moduleInfo)
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
            
            // 如果已经在监听，先停止
            StopWatching(moduleName);
            
            var watcher = new FileSystemWatcher(resourcesPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            watcher.Filter = "*.json";
            watcher.IncludeSubdirectories = false;
            
            // 当资源文件变更时重新加载
            watcher.Changed += (sender, e) => LocalizerManager.LoadModuleResources(moduleInfo);
            watcher.Created += (sender, e) => LocalizerManager.LoadModuleResources(moduleInfo);
            watcher.Deleted += (sender, e) => LocalizerManager.LoadModuleResources(moduleInfo);
            watcher.Renamed += (sender, e) => LocalizerManager.LoadModuleResources(moduleInfo);
            
            watcher.EnableRaisingEvents = true;
            _watchers[moduleName] = watcher;
        }
        
        /// <summary>
        /// 停止监听指定模块的资源文件变更
        /// </summary>
        /// <param name="moduleName">模块名称</param>
        public void StopWatching(string moduleName)
        {
            if (_watchers.TryGetValue(moduleName, out var watcher))
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
                _watchers.Remove(moduleName);
            }
        }
        
        /// <summary>
        /// 停止所有监听
        /// </summary>
        public void StopAllWatching()
        {
            foreach (var watcher in _watchers.Values)
            {
                watcher.EnableRaisingEvents = false;
                watcher.Dispose();
            }
            
            _watchers.Clear();
        }
    }
} 