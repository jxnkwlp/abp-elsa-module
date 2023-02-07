using System.Collections.Concurrent;

namespace Passingwind.Abp.ElsaModule.Monacos.Providers.Roslyns;

public static class RoslynHostSessionContainer
{
    private static readonly ConcurrentDictionary<string, RoslynHost> _container = new ConcurrentDictionary<string, RoslynHost>();

    public static RoslynHost GetOrCreate(string sessionId, RoslynHostReference hostReference = null)
    {
        return _container.GetOrAdd(sessionId, (_) =>
        {
            var host2 = new RoslynHost(hostReference ?? new RoslynHostReference());
            _container.TryAdd(sessionId, host2);
            return host2;
        });
    }

    public static void Remove(string sessionId)
    {
        if (_container.TryRemove(sessionId, out var host))
        {
            host.Dispose();
        }
    }
}