using Serilog.Core;

namespace Si.CoreHub.Logging
{
    public static class LogCenter
    {
        public static void Write2Log(Loglevel logLevel, string message)
        {
            switch (logLevel)
            {
                case Loglevel.Info:
                    {
                        StaticLog.Info(message);
                        break;
                    }
                case Loglevel.Fatal:
                    {
                        StaticLog.Fatal(message);
                        break;
                    }
                case Loglevel.Warning:
                    {
                        StaticLog.Warning(message);
                        break;
                    }
                case Loglevel.Error:
                    {
                        StaticLog.Error(message); 
                        break;
                    }

            }
        }
    }
}
