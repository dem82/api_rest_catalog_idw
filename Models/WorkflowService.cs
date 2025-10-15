using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowApi.Models;

[Table("workflow_service")]
public class WorkflowService
{
    [Column("id_workflow")]
    public string IdWorkflow { get; set; } = string.Empty;

    [Column("service_code")]
    public string ServiceCode { get; set; } = string.Empty;

    public Workflow? Workflow { get; set; }
    public Service? Service { get; set; }
}
