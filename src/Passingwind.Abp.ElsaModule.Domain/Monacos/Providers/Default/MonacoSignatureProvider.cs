using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Default;
public class MonacoSignatureProvider : IMonacoSignatureProvider
{
    public Task<MonacoSignatureResult> HandleAsync(MonacoSignatureRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new MonacoSignatureResult());
    }
}
