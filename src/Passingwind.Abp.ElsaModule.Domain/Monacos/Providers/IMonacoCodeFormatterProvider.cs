using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers;

public interface IMonacoCodeFormatterProvider : IScopedDependency
{
    Task<MonacoCodeFormatterResult> HandleAsync(MonacoCodeFormatterRequest request, CancellationToken cancellationToken = default);
}
