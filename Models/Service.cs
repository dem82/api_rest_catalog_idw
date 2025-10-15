using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkflowApi.Models;

[Table("service")]
public class Service
{
    [Key]
    [Column("service_code")]
    public string ServiceCode { get; set; } = string.Empty;

    [Column("service_desc")]
    public string? ServiceDesc { get; set; }

    public ICollection<WorkflowService> WorkflowServices { get; set; } = new List<WorkflowService>();
}
