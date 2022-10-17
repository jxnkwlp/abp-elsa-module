using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Demo.Data;

internal class WorkflowInstanceDataSender : ITransientDependency, IDataSeedContributor
{
    private readonly ILogger<WorkflowInstanceDataSender> _logger;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;

    public WorkflowInstanceDataSender(ILogger<WorkflowInstanceDataSender> logger, IWorkflowInstanceRepository workflowInstanceRepository)
    {
        _logger = logger;
        _workflowInstanceRepository = workflowInstanceRepository;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var db = await _workflowInstanceRepository.GetDbContextAsync();

        var checkSql = " SELECT 1 FROM sys.columns WHERE Name = N'ActivityData' AND Object_ID = Object_ID(N'ElsaWorkflowInstances') ";

        await db.Database.CommitTransactionAsync();

        var connection = db.Database.GetDbConnection();

        if (connection.ExecuteScalar<int>(checkSql) != 1)
        {
            return;
        }

        _logger.LogInformation("Starting migration workflow instance data...");

        var variables = db.Set<WorkflowInstanceVariable>();
        var metadata = db.Set<WorkflowInstanceMetadata>();
        var scheduledActivity = db.Set<WorkflowInstanceScheduledActivity>();
        var blockingActivity = db.Set<WorkflowInstanceBlockingActivity>();
        var activityScope = db.Set<WorkflowInstanceActivityScope>();
        var activityData = db.Set<WorkflowInstanceActivityData>();

        var querySql = "select Id, Variables, Metadata, ScheduledActivities, BlockingActivities, Scopes, ActivityData from ElsaWorkflowInstances where ActivityData <> ''";

        var list = connection.Query<WorkflowInstance>(querySql).ToArray();

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
                        variables.Add(new WorkflowInstanceVariable() { WorkflowInstanceId = instance.Id, Key = item.Key, Value = item.Value.ToString(Newtonsoft.Json.Formatting.None) });
                    }
                }

                if (!string.IsNullOrWhiteSpace(instance.Metadata))
                {
                    foreach (var item in JObject.Parse(instance.Metadata))
                    {
                        if (item.Key == "$id")
                            continue;
                        metadata.Add(new WorkflowInstanceMetadata() { WorkflowInstanceId = instance.Id, Key = item.Key, Value = item.Value.ToString(Newtonsoft.Json.Formatting.None) });
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
                _logger.LogError(ex, "Migration workflow instance error.");
            }
        }

        await db.SaveChangesAsync();

        // clear workflowinstance table 
        await connection.ExecuteAsync("ALTER TABLE [dbo].[ElsaWorkflowInstances] DROP COLUMN [Variables], COLUMN [Metadata], COLUMN [ScheduledActivities], COLUMN [BlockingActivities], COLUMN [Scopes], COLUMN [ActivityData]");

        _logger.LogInformation("Migration workflow instance data success.");

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