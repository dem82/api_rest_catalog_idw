using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Text.Json;
using WorkflowApi.Data;
using WorkflowApi.DTOs;

namespace WorkflowApi.Services;

public class WorkflowService : IWorkflowService
{
    private readonly WorkflowDbContext _dbContext;
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<WorkflowService> _logger;

    public WorkflowService(
        WorkflowDbContext dbContext,
        IConnectionMultiplexer redis,
        ILogger<WorkflowService> logger)
    {
        _dbContext = dbContext;
        _redis = redis;
        _logger = logger;
    }

    public async Task<WorkflowResponse?> FindWorkflowAsync(WorkflowRequest request)
    {
        // Generate cache key based on request parameters
        var cacheKey = GenerateCacheKey(request);
        var db = _redis.GetDatabase();

        try
        {
            // Try to get from Redis cache
            var cachedResult = await db.StringGetAsync(cacheKey);
            if (!cachedResult.IsNullOrEmpty)
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}", cacheKey);
                return JsonSerializer.Deserialize<WorkflowResponse>(cachedResult!);
            }

            _logger.LogInformation("Cache miss for key: {CacheKey}. Querying database...", cacheKey);

            // Query database to find matching workflow
            var workflow = await _dbContext.Workflows
                .Include(w => w.WorkflowServices)
                .Where(w => w.InputFormat == request.InputFormat && w.InputSystem == request.InputSystem)
                .FirstOrDefaultAsync(w => w.WorkflowServices
                    .Select(ws => ws.ServiceCode)
                    .All(sc => request.RequiredServices.Contains(sc)) &&
                    w.WorkflowServices.Count == request.RequiredServices.Count);

            if (workflow == null)
            {
                _logger.LogWarning("No workflow found for the given criteria");
                var failureResponse = new WorkflowResponse
                {
                    WorkflowCode = string.Empty,
                    Success = false
                };

                // Cache the failure result for a shorter time (30 seconds)
                await db.StringSetAsync(
                    cacheKey,
                    JsonSerializer.Serialize(failureResponse),
                    TimeSpan.FromSeconds(30));

                return failureResponse;
            }

            var response = new WorkflowResponse
            {
                WorkflowCode = workflow.IdWorkflow,
                Success = true
            };

            // Cache the successful result for 5 minutes
            await db.StringSetAsync(
                cacheKey,
                JsonSerializer.Serialize(response),
                TimeSpan.FromMinutes(5));

            _logger.LogInformation("Workflow {WorkflowCode} found and cached", workflow.IdWorkflow);

            return response;
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(ex, "Redis connection error. Falling back to database only.");
            
            // Fallback to database only if Redis is unavailable
            var workflow = await _dbContext.Workflows
                .Include(w => w.WorkflowServices)
                .Where(w => w.InputFormat == request.InputFormat && w.InputSystem == request.InputSystem)
                .FirstOrDefaultAsync(w => w.WorkflowServices
                    .Select(ws => ws.ServiceCode)
                    .All(sc => request.RequiredServices.Contains(sc)) &&
                    w.WorkflowServices.Count == request.RequiredServices.Count);

            if (workflow == null)
            {
                return new WorkflowResponse { WorkflowCode = string.Empty, Success = false };
            }

            return new WorkflowResponse { WorkflowCode = workflow.IdWorkflow, Success = true };
        }
    }

    private static string GenerateCacheKey(WorkflowRequest request)
    {
        // Sort services to ensure consistent cache key
        var sortedServices = string.Join(",", request.RequiredServices.OrderBy(s => s));
        return $"workflow:{request.InputFormat}:{request.InputSystem}:{sortedServices}:{request.OutputType}";
    }
}
