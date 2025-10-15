using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowApi.Models;

[Table("systems")]
public class SystemEntity
{
    [Key]
    [Column("system_code")]
    public string SystemCode { get; set; } = string.Empty;

    [Column("system_desc")]
    public string? SystemDesc { get; set; }

    public ICollection<WorkflowSystem> WorkflowSystems { get; set; } = new List<WorkflowSystem>();
}
