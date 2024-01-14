namespace MagicVilla_VillaAPI.Logging
{
    public interface ILogging
    {
        public void LogError(string methodName = "", string message="");
        public void LogWarn(string methodName = "", string message="");
        public void LogInformation(string methodName = "", string message = "");
        public void LogDebug(string methodName = "", string message="");
    }
}
