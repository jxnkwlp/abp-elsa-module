using System;
using Passingwind.Abp.ElsaModule.Common;
using Volo.Abp.Domain.Repositories.MongoDB;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB.Repositories
{
    public class BookmarkRepository : MongoDbRepository<IElsaModuleMongoDbContext, Bookmark, Guid>, IBookmarkRepository
    {
        public BookmarkRepository(IMongoDbContextProvider<IElsaModuleMongoDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
