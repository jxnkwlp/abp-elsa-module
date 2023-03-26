using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Passingwind.WorkflowApp.Data;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MongoDB;

namespace Passingwind.WorkflowApp.MongoDB;

public class MongoDbWorkflowAppDbSchemaMigrator : IWorkflowAppDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public MongoDbWorkflowAppDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        var dbContexts = _serviceProvider.GetServices<IAbpMongoDbContext>();
        var connectionStringResolver = _serviceProvider.GetRequiredService<IConnectionStringResolver>();

        foreach (var dbContext in dbContexts)
        {
            var connectionString = await connectionStringResolver.ResolveAsync(ConnectionStringNameAttribute.GetConnStringName(dbContext.GetType()));
            var mongoUrl = new MongoUrl(connectionString);
            var databaseName = mongoUrl.DatabaseName;
            var client = new MongoClient(mongoUrl);

            if (databaseName.IsNullOrWhiteSpace())
            {
                databaseName = ConnectionStringNameAttribute.GetConnStringName(dbContext.GetType());
            }

            (dbContext as AbpMongoDbContext)?.InitializeCollections(client.GetDatabase(databaseName));
        }
    }
}
