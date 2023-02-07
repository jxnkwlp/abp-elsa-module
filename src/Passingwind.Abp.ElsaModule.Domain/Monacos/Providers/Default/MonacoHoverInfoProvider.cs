using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Default;
public class MonacoHoverInfoProvider : IMonacoHoverInfoProvider
{
    public Task<MonacoHoverInfoResult> HandleAsync(MonacoHoverInfoRequest request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new MonacoHoverInfoResult());
    }
}
