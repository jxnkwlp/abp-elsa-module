using System;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Passingwind.Abp.ElsaModule.EntityFrameworkCore.Repositories
{
    public class BookmarkRepository : EfCoreRepository<IElsaModuleDbContext, Bookmark, Guid>, IBookmarkRepository
    {
        public BookmarkRepository(IDbContextProvider<IElsaModuleDbContext> dbContextProvider) : base(dbContextProvider)
        {

        }
    }
}
