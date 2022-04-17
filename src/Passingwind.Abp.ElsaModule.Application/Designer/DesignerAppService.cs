using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Elsa.Metadata;
using Elsa.Scripting.JavaScript.Services;
using Elsa.Services;
using Passingwind.Abp.ElsaModule.Common;
using Passingwind.Abp.ElsaModule.Stores;
using Volo.Abp.Content;

namespace Passingwind.Abp.ElsaModule.Designer
{
    public class DesignerAppService : ElsaModuleAppService, IDesignerAppService
    {
        private readonly IActivityTypeService _activityTypeService;
        private readonly ITypeScriptDefinitionService _typeScriptDefinitionService;
        private readonly IStoreMapper _storeMapper;
        private readonly IWorkflowDefinitionVersionRepository _workflowDefinitionVersionRepository;

        public DesignerAppService(IActivityTypeService activityTypeService, ITypeScriptDefinitionService typeScriptDefinitionService, IStoreMapper storeMapper, IWorkflowDefinitionVersionRepository workflowDefinitionVersionRepository)
        {
            _activityTypeService = activityTypeService;
            _typeScriptDefinitionService = typeScriptDefinitionService;
            _storeMapper = storeMapper;
            _workflowDefinitionVersionRepository = workflowDefinitionVersionRepository;
        }

        public async Task<ActivityTypeDescriptorListResultDto> GetActivityTypesAsync()
        {
            var activityTypes = await _activityTypeService.GetActivityTypesAsync();

            var tasks = activityTypes.Select(async x => await _activityTypeService.DescribeActivityType(x)).ToArray();

            var descriptors = await Task.WhenAll(tasks);

            var categories = descriptors.Select(x => x.Category).Distinct().OrderBy(x => x).ToArray();

            return new ActivityTypeDescriptorListResultDto()
            {
                Categories = categories,
                Items = ObjectMapper.Map<List<ActivityDescriptor>, List<ActivityTypeDescriptorDto>>(descriptors.ToList())
            };
        }

        public async Task<IRemoteStreamContent> GetScriptTypeDefinitionAsync(Guid id)
        {
            var definition = await _workflowDefinitionVersionRepository.GetLatestAsync(id);

            var workflowDefinition = _storeMapper.MapToModel(definition);

            var typeDefinitions = await _typeScriptDefinitionService.GenerateTypeScriptDefinitionsAsync(workflowDefinition);

            var fileName = $"workflow-definition-{id}.d.ts";

            var data = Encoding.UTF8.GetBytes(typeDefinitions);

            return new RemoteStreamContent(new MemoryStream(data), fileName, "application/x-typescript");
        }
    }
}
