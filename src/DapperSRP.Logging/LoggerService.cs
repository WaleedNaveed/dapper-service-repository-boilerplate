using Serilog;

namespace DapperSRP.Logging
{
    public class LoggerService<T> : ILoggerService<T>
    {
        private readonly ILogger _logger;

        public LoggerService()
        {
            _logger = Log.ForContext<T>();
        }

        public void LogInformation(string message) => _logger.Information(message);
        public void LogWarning(string message) => _logger.Warning(message);
        public void LogError(string message, Exception ex) => _logger.Error(ex, message);
    }
}
