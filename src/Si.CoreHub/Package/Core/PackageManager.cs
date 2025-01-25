using Si.CoreHub.Logs;
using Si.CoreHub.Package.Entitys;
using System.Reflection;

namespace Si.CoreHub.Package.Core
{
    public class PackageManager
    {
        private static PackOptions _packOptions;
        private static List<ModuleInfo> _modules = new List<ModuleInfo>();
        public PackageManager(PackOptions packOptions)
        {
            _packOptions = packOptions;
            AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
        }
        /// <summary>
        /// 获取所有模块信息
        /// </summary>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public List<ModuleInfo> GetPacks()
        {
            if (_modules.Count > 0)
            {
                return _modules;
            }
            if (string.IsNullOrEmpty(_packOptions.FilePath) || !Directory.Exists(_packOptions.FilePath))
            {
                throw new DirectoryNotFoundException($"指定的模块路径 {_packOptions.FilePath} 不存在");
            }
            var moduleDirs = Directory.GetDirectories(_packOptions.FilePath);
            foreach (var moduleDir in moduleDirs)
            {
                try
                {
                    string fileName = Path.GetFileName(moduleDir);
                    var moduleInfo = new ModuleInfo();
                    moduleInfo.AssemblyName = fileName + ".dll";
                    moduleInfo.AssemblyPath = Path.Combine(_packOptions.FilePath, fileName, $"{fileName}.dll");
                    var assembly = Assembly.LoadFrom(moduleInfo.AssemblyPath);
                    if (assembly == null)
                        continue;
                    moduleInfo.Assembly = assembly;
                    moduleInfo.ConfigFile = Path.Combine(_packOptions.FilePath, fileName, $"{fileName}.json");
                    _modules.Add(moduleInfo);
                }
                catch (Exception ex)
                {
                    LogCenter.Write2Log(Loglevel.Error, ex.Message +ex.StackTrace);
                    continue;
                }
            }
            return _modules;
        }
        /// <summary>
        /// 处理程序集解析事件，查找子文件夹路径
        /// </summary>
        private static Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {

            foreach (var moduleInfo in _modules)
            {
                var assemblyPath = Path.Combine(Path.GetDirectoryName(moduleInfo.AssemblyPath)!, args.Name + ".dll");

                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
            }
            return null;
        }



    }
}
