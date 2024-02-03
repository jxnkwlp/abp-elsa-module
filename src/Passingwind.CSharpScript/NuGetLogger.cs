using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Common;

namespace Passingwind.CSharpScriptEngine;

public class NuGetLogger : LoggerBase
{
    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    public NuGetLogger(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger("NuGet");
    }

    public override void Log(ILogMessage message)
    {
        _logger.LogDebug("Code: {code}, {message}", message.Code, message.Message);
    }

    public override Task LogAsync(ILogMessage message)
    {
        Log(message);
        return Task.CompletedTask;
    }
}
