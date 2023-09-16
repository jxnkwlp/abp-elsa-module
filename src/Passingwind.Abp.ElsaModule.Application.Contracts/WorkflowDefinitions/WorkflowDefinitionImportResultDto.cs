using System.Collections.Generic;

namespace Passingwind.Abp.ElsaModule.WorkflowDefinitions;

public class WorkflowDefinitionImportResultDto
{
    public int TotalCount { get; set; }
    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }

    public List<WorkflowImportResult> Results { get; set; }

    public class WorkflowImportResult
    {
        public string FileName { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public WorkflowDefinitionBasicDto Workflow { get; set; }
    }

    public WorkflowDefinitionImportResultDto()
    {
        Results = new List<WorkflowImportResult>();
    }

    public void Add(string fileName, WorkflowDefinitionBasicDto workflow)
    {
        Results.Add(new WorkflowImportResult
        {
            Workflow = workflow,
            HasError = false,
            FileName = fileName,
        });
    }

    public void Add(string fileName, string errorMessage)
    {
        Results.Add(new WorkflowImportResult
        {
            HasError = true,
            ErrorMessage = errorMessage,
            FileName = fileName,
        });
    }
}
