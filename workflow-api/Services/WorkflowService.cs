using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using WorkflowApi.Contracts.Requests;
using WorkflowApi.Contracts.Responses;
using WorkflowApi.Data;
using Workflow = WorkflowApi.Models.Workflow;
using WorkflowServiceEntity = WorkflowApi.Models.WorkflowService;

namespace WorkflowApi.Services;

public class WorkflowService : IWorkflowService
{
    private readonly WorkflowDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public WorkflowService(WorkflowDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<CreateWorkflowResponse> CreateOrGetWorkflowAsync(CreateWorkflowRequest request)
    {
        var cacheKey = BuildCacheKey(request);
        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            var cachedResponse = JsonSerializer.Deserialize<CreateWorkflowResponse>(cached);
            if (cachedResponse != null)
            {
                return cachedResponse;
            }
        }

        var normalizedServices = request.RequiredServices
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToUpperInvariant())
            .OrderBy(s => s)
            .ToList();

        // Find an existing workflow that matches input system/format and has exactly the same required services
        var candidateWorkflowIds = await _dbContext.Workflows
            .AsNoTracking()
            .Where(w => w.InputSystem == request.InputSystem && w.InputFormat == request.InputFormat)
            .Select(w => w.IdWorkflow)
            .ToListAsync();

        string? matchedExistingWorkflowId = null;
        foreach (var candidateId in candidateWorkflowIds)
        {
            var candidateServices = await _dbContext.WorkflowServices
                .AsNoTracking()
                .Where(ws => ws.IdWorkflow == candidateId)
                .Select(ws => ws.ServiceCode)
                .ToListAsync();

            var normalizedCandidate = candidateServices
                .Select(s => (s ?? string.Empty).Trim().ToUpperInvariant())
                .OrderBy(s => s)
                .ToList();

            if (normalizedCandidate.SequenceEqual(normalizedServices))
            {
                matchedExistingWorkflowId = candidateId;
                break;
            }
        }

        string workflowCode;
        if (matchedExistingWorkflowId != null)
        {
            workflowCode = matchedExistingWorkflowId;
        }
        else
        {
            // Create a new workflow code
            workflowCode = GenerateWorkflowCode(request);

            var workflow = new Workflow
            {
                IdWorkflow = workflowCode,
                Descrizione = $"Workflow for {request.InputSystem}/{request.InputFormat}",
                InputSystem = request.InputSystem,
                InputFormat = request.InputFormat
            };

            _dbContext.Workflows.Add(workflow);

            foreach (var serviceCode in normalizedServices)
            {
                // ensure service exists (optional minimal insert if not present)
                var serviceExists = await _dbContext.Services.AnyAsync(s => s.ServiceCode == serviceCode);
                if (!serviceExists)
                {
                    _dbContext.Services.Add(new Models.Service
                    {
                        ServiceCode = serviceCode,
                        ServiceDesc = serviceCode
                    });
                }

                _dbContext.WorkflowServices.Add(new WorkflowServiceEntity
                {
                    IdWorkflow = workflowCode,
                    ServiceCode = serviceCode
                });
            }

            await _dbContext.SaveChangesAsync();
        }

        var response = new CreateWorkflowResponse
        {
            WorkflowCode = workflowCode,
            Success = true
        };

        // Cache for repeated calls
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
        };
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(response), options);

        return response;
    }

    private static string BuildCacheKey(CreateWorkflowRequest request)
    {
        var normalizedServices = request.RequiredServices
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToUpperInvariant())
            .OrderBy(s => s);
        var input = $"{request.InputSystem}|{request.InputFormat}|{string.Join(',', normalizedServices)}|{request.OutputType}";
        using var sha256 = SHA256.Create();
        var hash = Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(input)));
        return $"workflow:{hash}";
    }

    private static string GenerateWorkflowCode(CreateWorkflowRequest request)
    {
        // Basic code generator; in real scenario use a sequence or specific rule
        return $"WF_{DateTime.UtcNow:yyyyMMddHHmmssfff}";
    }
}
