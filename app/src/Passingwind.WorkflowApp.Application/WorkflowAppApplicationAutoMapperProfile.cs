using AutoMapper;
using Passingwind.WorkflowApp.ApiKeys;

namespace Passingwind.WorkflowApp;

public class WorkflowAppApplicationAutoMapperProfile : Profile
{
    public WorkflowAppApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<ApiKey, ApiKeyDto>();
    }
}
