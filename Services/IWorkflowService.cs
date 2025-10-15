using WorkflowApi.DTOs;

namespace WorkflowApi.Services;

public interface IWorkflowService
{
    Task<WorkflowResponse?> FindWorkflowAsync(WorkflowRequest request);
}
