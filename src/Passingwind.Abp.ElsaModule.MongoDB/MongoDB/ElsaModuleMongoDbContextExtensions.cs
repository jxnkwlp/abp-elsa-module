using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Groups;
using Passingwind.Abp.ElsaModule.MongoDB.Serializers;
using Passingwind.Abp.ElsaModule.Teams;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB;

public static class ElsaModuleMongoDbContextExtensions
{
    public static void ConfigureElsaModule(this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        BsonSerializer.RegisterSerializer<object>(new MongoObjectSerializer());

        BsonSerializer.RegisterSerializer<System.Text.Json.JsonElement>(new MongoJsonElementSerializer());

        BsonSerializer.RegisterSerializer<Newtonsoft.Json.Linq.JObject>(new MongoJObjectSerializer());
        BsonSerializer.RegisterSerializer<Newtonsoft.Json.Linq.JArray>(new MongoJArraySerializer());

        BsonSerializer.RegisterSerializer<Elsa.Models.Variables>(new MongoVariablesSerializer());
        BsonSerializer.RegisterSerializer<SimpleExceptionModel>(new MongoJsonObjectSerializer<SimpleExceptionModel>());

        BsonSerializer.RegisterSerializer<Dictionary<string, object>>(new MongoJsonObjectSerializer<Dictionary<string, object>>());
        BsonSerializer.RegisterSerializer<IDictionary<string, object>>(new MongoJsonObjectSerializer<IDictionary<string, object>>());

        BsonSerializer.RegisterSerializer<List<Elsa.Models.ScheduledActivity>>(new MongoListSerializer<Elsa.Models.ScheduledActivity>());
        BsonSerializer.RegisterSerializer<List<Elsa.Models.BlockingActivity>>(new MongoListSerializer<Elsa.Models.BlockingActivity>());
        BsonSerializer.RegisterSerializer<List<Elsa.Models.ActivityScope>>(new MongoListSerializer<Elsa.Models.ActivityScope>());

        builder.Entity<Bookmark>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "Bookmarks";
        });

        builder.Entity<Trigger>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "Triggers";
        });

        builder.Entity<WorkflowDefinition>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "WorkflowDefinitions";
        });

        builder.Entity<WorkflowDefinitionVersion>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "WorkflowDefinitionVersions";
        });

        BsonClassMap.RegisterClassMap<Activity>(b =>
        {
            b.AutoMap();
        });

        BsonClassMap.RegisterClassMap<ActivityConnection>(b =>
        {
            b.AutoMap();
        });

        builder.Entity<WorkflowInstance>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "WorkflowInstances";
        });

        BsonClassMap.RegisterClassMap<WorkflowInstanceVariable>(b =>
        {
            b.AutoMap();
        });

        BsonClassMap.RegisterClassMap<WorkflowInstanceActivityData>(b =>
        {
            b.AutoMap();
        });

        BsonClassMap.RegisterClassMap<WorkflowInstanceActivityScope>(b =>
        {
            b.AutoMap();
        });

        BsonClassMap.RegisterClassMap<WorkflowInstanceBlockingActivity>(b =>
        {
            b.AutoMap();
        });

        BsonClassMap.RegisterClassMap<WorkflowInstanceFault>(b =>
        {
            b.AutoMap();
        });

        BsonClassMap.RegisterClassMap<WorkflowInstanceMetadata>(b =>
        {
            b.AutoMap();
        });

        BsonClassMap.RegisterClassMap<WorkflowInstanceScheduledActivity>(b =>
        {
            b.AutoMap();
        });

        builder.Entity<WorkflowExecutionLog>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "WorkflowExecutionLogs";
        });

        builder.Entity<GlobalVariable>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "GlobalVariables";
        });

        builder.Entity<WorkflowTeam>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "WorkflowTeams";
        });

        builder.Entity<WorkflowGroup>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "WorkflowGroups";
        });
    }
}
