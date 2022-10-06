﻿using System.Linq;
using System.Threading.Tasks;
using Elsa.Options;
using Microsoft.Extensions.Options;
using Volo.Abp.Application.Dtos;

namespace Passingwind.Abp.ElsaModule.Workflow
{
    public class WorkflowChannelAppService : ElsaModuleAppService, IWorkflowChannelAppService
    {
        private readonly ElsaOptions _elsaOptions;

        public WorkflowChannelAppService(IOptions<ElsaOptions> options)
        {
            _elsaOptions = options.Value;
        }

        public Task<ListResultDto<string>> GetListAsync()
        {
            var dto = new ListResultDto<string>(_elsaOptions.WorkflowChannelOptions.Channels.ToArray());

            return Task.FromResult(dto);
        }
    }
}