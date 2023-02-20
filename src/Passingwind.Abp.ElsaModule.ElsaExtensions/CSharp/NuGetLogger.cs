using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.CSharp;

public class NuGetLogger : LoggerBase, ITransientDependency
{
    private readonly ILogger<NuGetLogger> _logger;

    public NuGetLogger(ILogger<NuGetLogger> logger)
    {
        _logger = logger;
    }

    public override void Log(ILogMessage message)
    {
        switch (message.Level)
        {
            case NuGet.Common.LogLevel.Error:
                _logger.LogError("Code: {code}, {message}", message.Code, message.Message);
                break;
            case NuGet.Common.LogLevel.Verbose:
                _logger.LogTrace("Code: {code}, {message}", message.Code, message.Message);
                break;
            case NuGet.Common.LogLevel.Information:
                _logger.LogInformation("Code: {code}, {message}", message.Code, message.Message);
                break;
            case NuGet.Common.LogLevel.Warning:
                _logger.LogWarning("Code: {code}, {message}", message.Code, message.Message);
                break;
            case NuGet.Common.LogLevel.Minimal:
            case NuGet.Common.LogLevel.Debug:
                _logger.LogDebug("Code: {code}, {message}", message.Code, message.Message);
                break;
        }
    }

    public override Task LogAsync(ILogMessage message)
    {
        Log(message);
        return Task.CompletedTask;
    }
}
