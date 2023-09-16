using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using Passingwind.WorkflowApp.Account;
using Passingwind.WorkflowApp.Data;

namespace Passingwind.WorkflowApp.Web;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var logger = NLog.LogManager.Setup().GetCurrentClassLogger();

        try
        {
            logger.Info("Starting web host.");

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.AddAppSettingsSecretsJson()
                .ConfigureLogging((logging) => logging.ClearProviders())
                .UseNLog(new NLogAspNetCoreOptions { RemoveLoggerFactoryFilter = false })
                .UseAutofac();

            await builder.AddApplicationAsync<WorkflowAppWebModule>();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                await sp.GetRequiredService<WorkflowAppDbMigrationService>().MigrateAsync();
            }

            await app.InitializeApplicationAsync();

            using (var scope = app.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                await sp.GetRequiredService<IExternalLoginProviderManager>().RegisterAllAsync();
            }

            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            NLog.LogManager.Shutdown();
        }
    }
}
