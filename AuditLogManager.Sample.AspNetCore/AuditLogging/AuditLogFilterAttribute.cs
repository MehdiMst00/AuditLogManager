using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace AuditLogManager.Sample.AspNetCore.AuditLogging;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class AuditLogFilterAttribute(ILogger<AuditLogFilterAttribute> logger,
    IConfiguration configuration,
    IAuditingManager auditingManager) : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Skip audit log if action is decorated with [DisableAuditLog] attribute
        var disableAuditLog = context.ActionDescriptor.EndpointMetadata.OfType<DisableAuditLogFilterAttribute>().Any();
        if (disableAuditLog)
        {
            _ = await next();
            return;
        }

        var stopwatch = Stopwatch.StartNew();

        var auditLog = new AuditLog()
        {
            // UserId = context.HttpContext.User.GetUserId(), // TODO: Get user from claims if exists
            IpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString(),
            HttpMethod = context.HttpContext.Request.Method,
            Area = context.RouteData.Values["area"]?.ToString(),
            ControllerName = context.RouteData.Values["controller"]?.ToString(),
            ActionName = context.RouteData.Values["action"]?.ToString(),
            BrowserInfo = context.HttpContext.Request.Headers.UserAgent.ToString(),
            Url = $"{context.HttpContext.Request.Path}{context.HttpContext.Request.QueryString}",
            Header = JsonConvert.SerializeObject(context.HttpContext.Request.Headers),
            Arguments = JsonConvert.SerializeObject(context.ActionArguments),
        };

        // Set current audit log
        auditingManager.Current = auditLog;

        try
        {
            // Execute action
            var actionExecutedContext = await next();

            if (actionExecutedContext.Exception != null)
            {
                auditLog.Exceptions = actionExecutedContext.Exception.ToString();
            }

            auditLog.HttpStatusCode = GetStatusCode(actionExecutedContext);
        }
        catch (Exception ex)
        {
            auditLog.Exceptions = ex.ToString();
            throw;
        }
        finally
        {
            stopwatch.Stop();
            auditLog.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
        }

        // Call api to save audit, Or Implement `AuditLogQueue` service and call AuditLogQueue.Enqueue();
        // We create HTTP POST action in another project to call it, You can use gRPC, HTTP or you can call AuditLogQueue without remote scenaro :)
        await SendAsync(context.HttpContext.RequestAborted);
    }

    private static int GetStatusCode(ActionExecutedContext context)
    {
        // Exception detected
        if (context.Exception != null)
        {
            return context.Exception switch
            {
                // TODO: Add your custom exception here for set status code, for example NotFoundException for 404
                ArgumentException or OperationCanceledException => StatusCodes.Status400BadRequest,
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError,
            };
        }

        // Action result response
        if (context.Result is IStatusCodeActionResult statusCodeActionResult &&
            statusCodeActionResult.StatusCode != null)
            return statusCodeActionResult.StatusCode.Value;

        // Task methods without output
        if (context.ActionDescriptor is ControllerActionDescriptor actionDescriptor)
        {
            var returnType = actionDescriptor.MethodInfo.ReturnType;
            if (returnType == typeof(void) || returnType == typeof(Task))
            {
                return StatusCodes.Status204NoContent;
            }
        }

        // Default status code
        return context.HttpContext.Response.StatusCode;
    }

    private async Task SendAsync(CancellationToken cancellationToken)
    {
        using var httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromMilliseconds(configuration.GetValue<int>("AuditLog:Timeout"))
        };

        try
        {
            // Get url from appsetting
            var url = new Uri(configuration.GetValue<string>("AuditLog:Url")!);

            // Sending the request asynchronously without awaiting the response.
            var json = JsonConvert.SerializeObject(auditingManager.Current);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
            await httpClient.PostAsync(url, stringContent, cancellationToken);
        }
        catch (Exception ex)
        {
            // Handle any exceptions if the request fails.
            logger.LogError(ex, "An error occurred while saving audit: {Message}", ex.Message);
        }
    }
}