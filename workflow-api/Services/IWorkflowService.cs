using System.Threading.Tasks;
using WorkflowApi.Contracts.Requests;
using WorkflowApi.Contracts.Responses;

namespace WorkflowApi.Services;

public interface IWorkflowService
{
    Task<CreateWorkflowResponse> CreateOrGetWorkflowAsync(CreateWorkflowRequest request);
}
