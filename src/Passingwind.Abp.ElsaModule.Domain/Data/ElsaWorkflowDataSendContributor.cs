using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace Passingwind.Abp.ElsaModule.Data;

public class ElsaWorkflowDataSendContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IWorkflowDataSender _sender;

    public ElsaWorkflowDataSendContributor(IWorkflowDataSender sender)
    {
        _sender = sender;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        await _sender.SendAsync(context.TenantId);
    }
}
