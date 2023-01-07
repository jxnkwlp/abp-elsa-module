using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Scripting.CSharp.MonacoProviders;
public interface ICodeTestProvider
{
    Task<CodeCheckResult> HandlerAsync(CodeCheckRequest request);
}
