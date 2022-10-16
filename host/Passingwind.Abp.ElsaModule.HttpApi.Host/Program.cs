using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace Passingwind.Abp.ElsaModule;

public class Program
{
    public static async Task<int> Main(string[] args)
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

            using (var scope = app.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var dbcontext = sp.GetRequiredService<ElsaModuleHttpApiHostMigrationsDbContext>();
                await dbcontext.Database.MigrateAsync();

                await WorkflowDataMigrationsAsync(dbcontext);
            }

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

    private static async Task WorkflowDataMigrationsAsync(ElsaModuleHttpApiHostMigrationsDbContext db)
    {
        // 
        // For SqlServer 
        //

        var checkSql = " SELECT 1 FROM sys.columns WHERE Name = N'ActivityData' AND Object_ID = Object_ID(N'ElsaWorkflowInstances') ";

        var connect = db.Database.GetDbConnection();

        if (connect.ExecuteScalar<int>(checkSql) != 1)
        {
            return;
        }

        Log.Information("Starting migration workflow instance data...");

        var variables = db.Set<WorkflowInstanceVariable>();
        var metadata = db.Set<WorkflowInstanceMetadata>();
        var scheduledActivity = db.Set<WorkflowInstanceScheduledActivity>();
        var blockingActivity = db.Set<WorkflowInstanceBlockingActivity>();
        var activityScope = db.Set<WorkflowInstanceActivityScope>();
        var activityData = db.Set<WorkflowInstanceActivityData>();

        var querySql = "select Id, Variables, Metadata, ScheduledActivities, BlockingActivities, Scopes, ActivityData from ElsaWorkflowInstances where ActivityData <> ''";

        var list = connect.Query<WorkflowInstance>(querySql).ToArray();

        foreach (var instance in list)
        {
            try
            {

                if (!string.IsNullOrWhiteSpace(instance.Variables))
                {
                    foreach (var item in JObject.Parse(instance.Variables))
                    {
                        if (item.Key == "$id")
                            continue;
                        variables.Add(new WorkflowInstanceVariable() { WorkflowInstanceId = instance.Id, Key = item.Key, Value = item.Value.ToString() });
                    }
                }

                if (!string.IsNullOrWhiteSpace(instance.Metadata))
                {
                    foreach (var item in JObject.Parse(instance.Metadata))
                    {
                        if (item.Key == "$id")
                            continue;
                        metadata.Add(new WorkflowInstanceMetadata() { WorkflowInstanceId = instance.Id, Key = item.Key, Value = item.Value.ToString() });
                    }
                }

                if (!string.IsNullOrWhiteSpace(instance.ScheduledActivities))
                {
                    foreach (JToken item in JArray.Parse(instance.ScheduledActivities))
                    {
                        var tmp = item.ToObject<Elsa.Models.ScheduledActivity>();
                        scheduledActivity.Add(new WorkflowInstanceScheduledActivity() { WorkflowInstanceId = instance.Id, ActivityId = Guid.Parse(tmp.ActivityId), Input = tmp.Input });
                    }
                }

                if (!string.IsNullOrWhiteSpace(instance.BlockingActivities))
                {
                    foreach (JToken item in JArray.Parse(instance.BlockingActivities))
                    {
                        var tmp = item.ToObject<Elsa.Models.BlockingActivity>();
                        blockingActivity.Add(new WorkflowInstanceBlockingActivity() { WorkflowInstanceId = instance.Id, ActivityId = Guid.Parse(tmp.ActivityId), ActivityType = tmp.ActivityType, Tag = tmp.Tag, });
                    }
                }

                if (!string.IsNullOrWhiteSpace(instance.Scopes))
                {
                    foreach (JToken item in JArray.Parse(instance.Scopes))
                    {
                        var tmp = item.ToObject<Elsa.Models.ActivityScope>();
                        activityScope.Add(new WorkflowInstanceActivityScope() { WorkflowInstanceId = instance.Id, ActivityId = Guid.Parse(tmp.ActivityId), Variables = (Dictionary<string, object>)tmp.Variables.Data, });
                    }
                }
                if (!string.IsNullOrWhiteSpace(instance.ActivityData))
                {
                    foreach (var item in JObject.Parse(instance.ActivityData))
                    {
                        if (item.Key == "$id")
                            continue;
                        activityData.Add(new WorkflowInstanceActivityData() { WorkflowInstanceId = instance.Id, ActivityId = Guid.Parse(item.Key), Data = item.Value.ToObject<Dictionary<string, object>>(), });
                    }
                }
            }
            catch (Exception ex)
            {
                // ignore error 
                Log.Error(ex, "Migration workflow instance error.");
            }
        }

        await db.SaveChangesAsync();

        // clear workflowinstance table 
        await connect.ExecuteAsync("ALTER TABLE [dbo].[ElsaWorkflowInstances] DROP COLUMN [Variables], COLUMN [Metadata], COLUMN [ScheduledActivities], COLUMN [BlockingActivities], COLUMN [Scopes], COLUMN [ActivityData]");

        Log.Information("Migration workflow instance data success.");
    }

    private class WorkflowInstance
    {
        public Guid Id { get; set; }
        public string Variables { get; set; }
        public string Metadata { get; set; }
        public string ScheduledActivities { get; set; }
        public string BlockingActivities { get; set; }
        public string Scopes { get; set; }
        public string ActivityData { get; set; }
    }
}
