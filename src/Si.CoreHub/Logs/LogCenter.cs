using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Si.CoreHub.Logs
{
    public static class LogCenter
    {
        internal static Dictionary<Loglevel, Logger> _loggers = new Dictionary<Loglevel, Logger>();
        internal static int _lock = 0;
        /// 初始化 Serilog 配置
        /// </summary>
        public static void Init(string LogFilePath)
        {
            if (Interlocked.Exchange(ref _lock, 1) == 0)
            {
                if (string.IsNullOrEmpty(LogFilePath))
                {
                    throw new ArgumentException("日志文件路径不能为空！");
                }
                LogFilePath = Path.Combine(LogFilePath, DateTime.Now.ToString("yyyy-MM-dd"));
                var Infomation = new LoggerConfiguration().
                    WriteTo.File(
                     Path.Combine(LogFilePath, "Information-.log"),
                        restrictedToMinimumLevel: LogEventLevel.Information,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 10,
                        fileSizeLimitBytes: 10485760,
                        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
                    ).CreateLogger();
                var warning = new LoggerConfiguration().
                  WriteTo.File(
                   Path.Combine(LogFilePath, "Warning-.log"),
                      restrictedToMinimumLevel: LogEventLevel.Warning,
                      rollingInterval: RollingInterval.Day,
                      retainedFileCountLimit: 10,
                      fileSizeLimitBytes: 10485760,
                      outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
                  ).CreateLogger();
                var Error = new LoggerConfiguration().
                   WriteTo.File(
                    Path.Combine(LogFilePath, "Error-.log"),
                       restrictedToMinimumLevel: LogEventLevel.Error,
                       rollingInterval: RollingInterval.Day,
                       retainedFileCountLimit: 10,
                       fileSizeLimitBytes: 10485760,
                       outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
                   ).CreateLogger();
                var fatal = new LoggerConfiguration().
                   WriteTo.File(
                    Path.Combine(LogFilePath, "Fatal-.log"),
                       restrictedToMinimumLevel: LogEventLevel.Fatal,
                       rollingInterval: RollingInterval.Day,
                       retainedFileCountLimit: 10,
                       fileSizeLimitBytes: 10485760,
                       outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
                   ).CreateLogger();
                _loggers.Add(Loglevel.Info, Infomation);
                _loggers.Add(Loglevel.Error, Error);
                _loggers.Add(Loglevel.Fatal, fatal);
                _loggers.Add(Loglevel.Warning, warning);
                Log.Logger = Infomation;
            }
        }
        /// <summary>
        /// 输出日志
        /// </summary>
        /// <param name="level">日志级别</param>
        /// <param name="message">日志信息</param>
        internal static void Write2Log(Loglevel level, string message)
        {
            //只有初始化完成后才能输出日志
            if(_lock == 1)
            {
                switch (level)
                {
                    case Loglevel.Info:
                        _loggers[Loglevel.Info].Information(message);
                        break;
                    case Loglevel.Error:
                        _loggers[Loglevel.Error].Error(message);
                        break;
                    case Loglevel.Fatal:
                        _loggers[Loglevel.Fatal].Fatal(message);
                        break;
                    case Loglevel.Warning:
                        _loggers[Loglevel.Warning].Warning(message);
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// 将System日志注册为Serilog日志
        /// </summary>
        /// <param name="Host"></param>
        /// <param name="filePath"></param>
        public static void ConfiguraSystemHub(this IHostBuilder Host, string filePath)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(filePath, "System-.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
            Host.UseSerilog();
        }
        /// <summary>
        /// 注册日志服务
        /// </summary>
        /// <param name="services"></param>
        public static void UseLogHub(this IServiceCollection services)
        {
            services.AddScoped<ILogHub, LogHub>();
        }
    }
}
