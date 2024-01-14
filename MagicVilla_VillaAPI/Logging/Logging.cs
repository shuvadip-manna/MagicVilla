namespace MagicVilla_VillaAPI.Logging
{
    public class Logging : ILogging
    {
        public void LogDebug(string MethodName, string Message)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.WriteLine("DEBUG - ["+MethodName+"] - "+Message);
            Console.BackgroundColor = ConsoleColor.Blue;
        }

        public void LogError(string MethodName, string Message)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR - [" + MethodName + "] - " + Message);
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public void LogInformation(string MethodName, string Message)
        {
            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("INFO - [" + MethodName + "] - " + Message);
            Console.BackgroundColor = ConsoleColor.Black;
        }

        public void LogWarn(string MethodName, string Message)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WARN - [" + MethodName + "] - " + Message);
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}
