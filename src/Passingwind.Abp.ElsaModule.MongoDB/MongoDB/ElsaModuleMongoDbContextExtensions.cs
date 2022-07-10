using System.Collections.Generic;
using MongoDB.Bson.Serialization;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.MongoDB.Serializers;
using Volo.Abp;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB;

public static class ElsaModuleMongoDbContextExtensions
{
    public static void ConfigureElsaModule(this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        BsonSerializer.RegisterSerializer<Dictionary<string, object>>(new MongoDictionarySerializer<Dictionary<string, object>>());
        BsonSerializer.RegisterSerializer<IDictionary<string, object>>(new MongoDictionarySerializer<IDictionary<string, object>>());
        BsonSerializer.RegisterSerializer<Elsa.Models.Variables>(new MongoVariablesSerializer());
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

        builder.Entity<WorkflowExecutionLog>(b =>
        {
            b.CollectionName = ElsaModuleDbProperties.DbTablePrefix + "WorkflowExecutionLogs";
        });
    }
}
