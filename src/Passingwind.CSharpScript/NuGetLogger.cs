using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Common;

namespace Passingwind.CSharpScriptEngine;

public class NuGetLogger : LoggerBase
{
    private readonly ILogger<NuGetLogger> _logger;

    public NuGetLogger(ILogger<NuGetLogger> logger)
    {
        _logger = logger;
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
