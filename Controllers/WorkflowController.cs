using Microsoft.AspNetCore.Mvc;
using WorkflowApi.DTOs;
using WorkflowApi.Services;

namespace WorkflowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowService _workflowService;
    private readonly ILogger<WorkflowController> _logger;

    public WorkflowController(IWorkflowService workflowService, ILogger<WorkflowController> logger)
    {
        _workflowService = workflowService;
        _logger = logger;
    }

    /// <summary>
    /// Finds a workflow based on input criteria
    /// </summary>
    /// <param name="request">The workflow search request</param>
    /// <returns>Workflow response with workflow code and success status</returns>
    [HttpPost("find")]
    [ProducesResponseType(typeof(WorkflowResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WorkflowResponse>> FindWorkflow([FromBody] WorkflowRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.InputFormat))
        {
            return BadRequest(new { error = "InputFormat is required" });
        }

        if (string.IsNullOrWhiteSpace(request.InputSystem))
        {
            return BadRequest(new { error = "InputSystem is required" });
        }

        if (request.RequiredServices == null || !request.RequiredServices.Any())
        {
            return BadRequest(new { error = "RequiredServices must contain at least one service" });
        }

        _logger.LogInformation("Finding workflow for InputFormat: {InputFormat}, InputSystem: {InputSystem}",
            request.InputFormat, request.InputSystem);

        var result = await _workflowService.FindWorkflowAsync(request);

        if (result == null)
        {
            return Ok(new WorkflowResponse { WorkflowCode = string.Empty, Success = false });
        }

        return Ok(result);
    }
}
