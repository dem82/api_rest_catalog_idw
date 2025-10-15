using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowApi.Models;

[Table("workflow_systems")]
public class WorkflowSystem
{
    [Column("id_workflow")]
    public string IdWorkflow { get; set; } = string.Empty;

    [Column("system_code")]
    public string SystemCode { get; set; } = string.Empty;

    [Column("system_ord")]
    public int SystemOrd { get; set; }

    public Workflow? Workflow { get; set; }
    public SystemEntity? System { get; set; }
}
