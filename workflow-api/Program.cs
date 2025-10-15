using Microsoft.EntityFrameworkCore;
using WorkflowApi.Data;
using WorkflowApi.Endpoints;
using WorkflowApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core with SQLite
builder.Services.AddDbContext<WorkflowDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Redis distributed cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("Redis").GetValue<string>("Configuration") ?? "localhost:6379";
    options.InstanceName = builder.Configuration.GetSection("Redis").GetValue<string>("InstanceName") ?? "workflow_";
});

// Application services
builder.Services.AddScoped<IWorkflowService, WorkflowService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply minimal API endpoints
app.MapWorkflowEndpoints();

// Ensure database exists and apply simple migration behavior
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
    await db.Database.EnsureCreatedAsync();
}

app.Run();
