using Microsoft.EntityFrameworkCore;
using WorkflowApi.Models;

namespace WorkflowApi.Data;

public class WorkflowDbContext : DbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
    {
    }

    public DbSet<Workflow> Workflows { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<WorkflowService> WorkflowServices { get; set; }
    public DbSet<WorkflowSystem> WorkflowSystems { get; set; }
    public DbSet<SystemEntity> Systems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure WorkflowService composite key
        modelBuilder.Entity<WorkflowService>()
            .HasKey(ws => new { ws.IdWorkflow, ws.ServiceCode });

        modelBuilder.Entity<WorkflowService>()
            .HasOne(ws => ws.Workflow)
            .WithMany(w => w.WorkflowServices)
            .HasForeignKey(ws => ws.IdWorkflow);

        modelBuilder.Entity<WorkflowService>()
            .HasOne(ws => ws.Service)
            .WithMany(s => s.WorkflowServices)
            .HasForeignKey(ws => ws.ServiceCode);

        // Configure WorkflowSystem composite key
        modelBuilder.Entity<WorkflowSystem>()
            .HasKey(ws => new { ws.IdWorkflow, ws.SystemCode });

        modelBuilder.Entity<WorkflowSystem>()
            .HasOne(ws => ws.Workflow)
            .WithMany(w => w.WorkflowSystems)
            .HasForeignKey(ws => ws.IdWorkflow);

        modelBuilder.Entity<WorkflowSystem>()
            .HasOne(ws => ws.System)
            .WithMany(s => s.WorkflowSystems)
            .HasForeignKey(ws => ws.SystemCode);
    }
}
