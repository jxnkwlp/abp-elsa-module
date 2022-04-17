using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule
{
    public interface IIdGenerator : ISingletonDependency
    {
        long Generate();
    }
}
