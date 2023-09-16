using System;
using Volo.Abp.Domain.Repositories;

namespace Passingwind.Abp.ElsaModule.Common;

public interface IBookmarkRepository : IRepository<Bookmark, Guid>
{
}
