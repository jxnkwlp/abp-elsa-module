using System;
using System.Threading.Tasks;
using Demo.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;

namespace Demo;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

        try
        {
            logger.Info("Starting Demo.HttpApi.Host.");

            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                .ConfigureLogging((host, logging) =>
                {
                    logging.AddSimpleConsole(formatOption =>
                    {
                        formatOption.SingleLine = true;
                        formatOption.TimestampFormat = "yyyy-MM-dd HH:mm:ss ";
                    });
                })
                .UseNLog(new NLogAspNetCoreOptions { RemoveLoggerFactoryFilter = false, })
                .UseAutofac()
                ;
            await builder.AddApplicationAsync<DemoHttpApiHostModule>();
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                await sp.GetRequiredService<DemoDbMigrationService>().MigrateAsync();
            }
            await app.InitializeApplicationAsync();
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
