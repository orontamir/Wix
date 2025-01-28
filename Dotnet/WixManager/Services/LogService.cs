using Serilog;

namespace WixManager.Services
{
    public class LogService
    {
        private readonly ILogger _logger;

        public static void LogInformation(string message)
        {
            Log.Information(message);
        }

        public static void LogWarning(string message) { 
            Log.Warning(message);
        }

        public static void LogError(string message) {
            Log.Error(message); 
        }
    }
}
