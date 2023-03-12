using System;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.PermissionManagement;

namespace Passingwind.Abp.ElsaModule.Permissions;

public interface IPermissionDefinitionRepository : IRepository<PermissionDefinitionRecord, Guid>
{
}