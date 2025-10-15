using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WorkflowApi.Contracts.Requests;

public class CreateWorkflowRequest
{
    [Required]
    public string InputFormat { get; set; } = string.Empty;

    [Required]
    public string InputSystem { get; set; } = string.Empty;

    [Required]
    public List<string> RequiredServices { get; set; } = new();

    [Required]
    public int OutputType { get; set; }
}
