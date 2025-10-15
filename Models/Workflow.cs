using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowApi.Models;

[Table("workflow")]
public class Workflow
{
    [Key]
    [Column("id_workflow")]
    public string IdWorkflow { get; set; } = string.Empty;

    [Column("descrizione")]
    public string? Descrizione { get; set; }

    [Column("input_system")]
    public string? InputSystem { get; set; }

    [Column("input_format")]
    public string? InputFormat { get; set; }

    public ICollection<WorkflowService> WorkflowServices { get; set; } = new List<WorkflowService>();
    public ICollection<WorkflowSystem> WorkflowSystems { get; set; } = new List<WorkflowSystem>();
}
