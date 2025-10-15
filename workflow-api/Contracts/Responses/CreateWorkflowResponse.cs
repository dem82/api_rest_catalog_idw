namespace WorkflowApi.Contracts.Responses;

public class CreateWorkflowResponse
{
    public string WorkflowCode { get; set; } = string.Empty;
    public bool Success { get; set; }
}
