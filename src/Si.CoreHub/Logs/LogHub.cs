namespace Si.CoreHub.Logs
{

    public class LogHub : ILogHub
    {
        public void Info(string message)
        {
            LogSetting.Write2Log(Loglevel.Info, message);
        }

        public void Error(string message)
        {
            LogSetting.Write2Log(Loglevel.Error, message);
        }

        public void Warn(string message)
        {
            LogSetting.Write2Log(Loglevel.Warning, message);
        }
        public void Fatal(string message)
        {
            LogSetting.Write2Log(Loglevel.Fatal, message);
        }
    }
}
