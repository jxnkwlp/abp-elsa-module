using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Passingwind.Abp.ElsaModule.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace Passingwind.Abp.ElsaModule;

public class Program
{
    public async static Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Async(c => c.File("Logs/logs.log", rollingInterval: RollingInterval.Day, shared: true))
            .WriteTo.Async(c => c.Console())
            .CreateLogger();

        try
        {
            Log.Information("Starting web host.");
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.AddAppSettingsSecretsJson()
                .UseAutofac()
                .UseSerilog();
            await builder.AddApplicationAsync<ElsaModuleHttpApiHostModule>();

            var app = builder.Build();

            //using (var scope = app.Services.CreateScope())
            //{
            //    var sp = scope.ServiceProvider;
            //    var dbcontext = sp.GetRequiredService<ElsaModuleHttpApiHostMigrationsDbContext>();
            //    await dbcontext.Database.MigrateAsync();
            //}

            await app.InitializeApplicationAsync();
            await app.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Host terminated unexpectedly!");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
