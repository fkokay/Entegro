using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger = Serilog.ILogger;

namespace Entegro.Infrastructure.Logging
{
    public class SerilogLoggingService : ILoggingService
    {
        private readonly ILogger _logger;

        public SerilogLoggingService()
        {
            _logger = Log.ForContext<SerilogLoggingService>();
        }

        public void LogInformation(string message) => _logger.Information(message);
        public void LogWarning(string message) => _logger.Warning(message);
        public void LogError(string message, Exception? exception = null) => _logger.Error(exception, message);
        public void LogDebug(string message) => _logger.Debug(message);
        public void LogCritical(string message, Exception? exception = null) => _logger.Fatal(exception, message);
    }
}
