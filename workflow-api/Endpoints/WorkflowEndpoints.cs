using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WorkflowApi.Contracts.Requests;
using WorkflowApi.Contracts.Responses;
using WorkflowApi.Services;

namespace WorkflowApi.Endpoints;

public static class WorkflowEndpoints
{
    public static IEndpointRouteBuilder MapWorkflowEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/workflow", async (CreateWorkflowRequest request, IWorkflowService workflowService) =>
        {
            if (string.IsNullOrWhiteSpace(request.InputFormat) || string.IsNullOrWhiteSpace(request.InputSystem) || request.RequiredServices == null)
            {
                return Results.BadRequest("Invalid request body");
            }

            var response = await workflowService.CreateOrGetWorkflowAsync(request);

            // Ensure property names match exact casing required in response
            return Results.Json(new CreateWorkflowResponse
            {
                WorkflowCode = response.WorkflowCode,
                Success = response.Success
            });
        })
        .WithOpenApi();

        return endpoints;
    }
}
