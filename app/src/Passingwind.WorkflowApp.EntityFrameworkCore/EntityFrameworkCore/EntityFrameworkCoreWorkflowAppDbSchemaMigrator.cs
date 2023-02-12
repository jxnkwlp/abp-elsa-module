using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Passingwind.WorkflowApp.Data;
using Volo.Abp.DependencyInjection;

namespace Passingwind.WorkflowApp.EntityFrameworkCore;

public class EntityFrameworkCoreWorkflowAppDbSchemaMigrator
    : IWorkflowAppDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreWorkflowAppDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the WorkflowAppDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<WorkflowAppDbContext>()
            .Database
            .MigrateAsync();
    }
}
