using System;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.Permissions;

public interface IPermissionGroupDefinitionRepository : IRepository<PermissionGroupDefinitionRecord, Guid>, ITransientDependency
{
}
