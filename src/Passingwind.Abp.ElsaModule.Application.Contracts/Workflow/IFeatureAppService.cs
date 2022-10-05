using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public interface IFeatureAppService : IApplicationService
    {
        Task<WorkflowFeatureResultDto> GetAsync();
    }
}
