using Passingwind.Abp.ElsaModule.Samples;
using Xunit;

namespace Passingwind.Abp.ElsaModule.MongoDB.Samples;

[Collection(MongoTestCollection.Name)]
public class SampleRepository_Tests : SampleRepository_Tests<ElsaModuleMongoDbTestModule>
{
    /* Don't write custom repository tests here, instead write to
     * the base class.
     * One exception can be some specific tests related to MongoDB.
     */
}
