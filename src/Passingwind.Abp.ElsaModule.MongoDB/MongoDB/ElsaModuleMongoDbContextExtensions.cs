using Volo.Abp;
using Volo.Abp.MongoDB;

namespace Passingwind.Abp.ElsaModule.MongoDB;

public static class ElsaModuleMongoDbContextExtensions
{
    public static void ConfigureElsaModule(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
