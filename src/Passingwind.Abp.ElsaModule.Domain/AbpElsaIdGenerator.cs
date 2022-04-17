using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Passingwind.Abp.ElsaModule;

public class AbpElsaIdGenerator : Elsa.Services.IIdGenerator, ISingletonDependency
{
    private readonly IGuidGenerator _guidGenerator;

    public AbpElsaIdGenerator(IGuidGenerator guidGenerator)
    {
        _guidGenerator = guidGenerator;
    }

    public string Generate()
    {
        return _guidGenerator.Create().ToString();
    }
}
