using System.Reflection;

namespace Si.CoreHub.Package.Entitys
{
    public class ModuleInfo
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// 模块路径
        /// </summary>
        public string AssemblyPath { get; set; }
        /// <summary>
        /// 模块程序集
        /// </summary>
        public Assembly Assembly { get; set; }
        /// <summary>
        /// 配置文件路径
        /// </summary>
        public string ConfigFile { get; set; }
    }
}
