namespace Si.CoreHub.Logs
{
    public interface ILogHub
    {
        void Error(string message);
        void Fatal(string message);
        void Info(string message);
        void Warn(string message);
    }
}