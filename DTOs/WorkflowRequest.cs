namespace WorkflowApi.DTOs;

public class WorkflowRequest
{
    public string InputFormat { get; set; } = string.Empty;
    public string InputSystem { get; set; } = string.Empty;
    public List<string> RequiredServices { get; set; } = new();
    public int OutputType { get; set; }
}
