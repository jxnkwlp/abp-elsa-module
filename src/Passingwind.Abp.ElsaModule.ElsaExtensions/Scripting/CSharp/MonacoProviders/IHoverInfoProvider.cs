using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.MonacoProviders;

public interface IHoverInfoProvider : IScopedDependency
{
    Task<HoverInfoResult> HandleAsync(HoverInfoRequest request);
}
