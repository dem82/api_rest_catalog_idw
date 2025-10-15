using Microsoft.EntityFrameworkCore;
using WorkflowApi.Models;

namespace WorkflowApi.Data;

public class WorkflowDbContext : DbContext
{
    public WorkflowDbContext(DbContextOptions<WorkflowDbContext> options) : base(options)
    {
    }

    public DbSet<Workflow> Workflows => Set<Workflow>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<WorkflowService> WorkflowServices => Set<WorkflowService>();
    public DbSet<SystemEntity> Systems => Set<SystemEntity>();
    public DbSet<WorkflowSystem> WorkflowSystems => Set<WorkflowSystem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkflowService>().HasKey(ws => new { ws.IdWorkflow, ws.ServiceCode });
        modelBuilder.Entity<WorkflowService>()
            .HasOne(ws => ws.Workflow)
            .WithMany(w => w.WorkflowServices)
            .HasForeignKey(ws => ws.IdWorkflow);
        modelBuilder.Entity<WorkflowService>()
            .HasOne(ws => ws.Service)
            .WithMany(s => s.WorkflowServices)
            .HasForeignKey(ws => ws.ServiceCode);

        modelBuilder.Entity<WorkflowSystem>().HasKey(ws => new { ws.IdWorkflow, ws.SystemCode });
        modelBuilder.Entity<WorkflowSystem>()
            .HasOne(ws => ws.Workflow)
            .WithMany(w => w.WorkflowSystems)
            .HasForeignKey(ws => ws.IdWorkflow);
        modelBuilder.Entity<WorkflowSystem>()
            .HasOne(ws => ws.System)
            .WithMany(s => s.WorkflowSystems)
            .HasForeignKey(ws => ws.SystemCode);

        // Ensure table names in lower-case match the schema
        modelBuilder.Entity<Workflow>().ToTable("workflow");
        modelBuilder.Entity<Service>().ToTable("service");
        modelBuilder.Entity<WorkflowService>().ToTable("workflow_service");
        modelBuilder.Entity<SystemEntity>().ToTable("systems");
        modelBuilder.Entity<WorkflowSystem>().ToTable("workflow_systems");
    }
}
