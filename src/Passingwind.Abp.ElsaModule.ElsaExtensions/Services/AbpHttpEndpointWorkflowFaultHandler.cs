using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Elsa.Activities.Http.Contracts;
using Elsa.Activities.Http.Models;
using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Json;

namespace Passingwind.Abp.ElsaModule.Services;

[ExposeServices(typeof(IHttpEndpointWorkflowFaultHandler))]
public class AbpHttpEndpointWorkflowFaultHandler : IHttpEndpointWorkflowFaultHandler, ITransientDependency
{
    private readonly IJsonSerializer _jsonSerializer;

    public AbpHttpEndpointWorkflowFaultHandler(IJsonSerializer jsonSerializer)
    {
        _jsonSerializer = jsonSerializer;
    }

    public async ValueTask HandleAsync(HttpEndpointFaultedWorkflowContext context)
    {
        var httpContext = context.HttpContext;
        var workflowInstance = context.WorkflowInstance;

        httpContext.Response.ContentType = MediaTypeNames.Application.Json;
        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var faultedResponse = _jsonSerializer.Serialize(new
        {
            error = new
            {
                message = $"Workflow with id {workflowInstance.Id} faulted at {workflowInstance.FaultedAt!}",
                data = new
                {
                    id = workflowInstance.Id,
                    name = workflowInstance.Name,
                    version = workflowInstance.Version,
                },
                faults = workflowInstance.Faults?.Select(fault => new
                {
                    activityId = fault.FaultedActivityId,
                    message = fault?.Message,
                    exception = fault?.Exception,
                }),
            }
        });

        await httpContext.Response.WriteAsync(faultedResponse, context.CancellationToken);
    }
}
