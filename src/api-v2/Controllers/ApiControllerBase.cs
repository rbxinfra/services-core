namespace Roblox.ApiV2;

using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Web.Framework.Common;

/// <summary>
/// Base class for an API controller that consumes <see cref="IOperation"/>s
/// </summary>
[ApiController]
public abstract class ApiControllerBase : Controller 
{
    private const string _ApiKeyQueryKey = "apiKey";
    private const string _ApiKeyHeaderKey = "Roblox-Api-Key";

    private static IActionResult HandleExecutionStatus(ExecutionStatus status)
        => status switch
        {
            ExecutionStatus.ServiceDisabled => new HttpStatusCodeResult(503, "Service is disabled."),
            ExecutionStatus.OperationDisabled => new HttpStatusCodeResult(503, "Operation is disabled."),
            ExecutionStatus.ClientAuthenticationRejected => new HttpStatusCodeResult(401, "Invalid client credentials."),
            ExecutionStatus.ClientOperationRestricted => new HttpStatusCodeResult(503, "The client is not authorized to perform this operation"),
            ExecutionStatus.ClientServiceThrottled or ExecutionStatus.ClientOperationThrottled or
            ExecutionStatus.ServiceThrottled or ExecutionStatus.OperationThrottled => new StatusCodeResult(429),
            _ => null
        };

    private static IActionResult HandleOperationException(OperationException ex)
        => new ObjectResult(new { Message = ex.Message }) { StatusCode = 409 };

    private string GetApiKey()
    {
        if (Request.Headers.TryGetValue(_ApiKeyHeaderKey, out var apiKey) || Request.Query.TryGetValue(_ApiKeyQueryKey, out apiKey)) 
            return apiKey;

        return string.Empty;
    }

    /// <summary>
    /// Execute the specified operation with the given key.
    /// </summary>
    /// <param name="apiKey">The API key given from the request.</param>
    /// <param name="operation">The <see cref="IOperation"/> to execute.</param>
    /// <returns>An <see cref="IActionResult"/> object containing an error or result.</returns>
    protected IActionResult ExecuteOperation(string apiKey, IOperation operation)
    {
        ExecutionStatus status;

        try
        {
            status = operation.Execute(apiKey);
        }
        catch (OperationException ex) { return HandleOperationException(ex); }

        return status != ExecutionStatus.Success ? HandleExecutionStatus(status) : Ok();
    }

    /// <summary>
    /// Execute the specified operation with the given key.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <param name="apiKey">The API key given from the request.</param>
    /// <param name="operation">The <see cref="IResultOperation{TResult}"/> to execute.</param>
    /// <returns>An <see cref="IActionResult"/> object containing an error or result.</returns>
    protected IActionResult ExecuteOperation<T>(string apiKey, IResultOperation<T> operation)
    {
        ExecutionStatus status;

        try
        {
            status = operation.Execute(apiKey);
        }
        catch (OperationException ex) { return HandleOperationException(ex); }

        return status != ExecutionStatus.Success ? HandleExecutionStatus(status) : Json(new { data = operation.Result });
    }

    /// <summary>
    /// Execute the specified operation with the given key async.
    /// </summary>
    /// <remarks>This method is here for compatibility with async based ASP.NET Core methods, as I actually have no clue if ApiV2 got an async rework.</remarks>
    /// <param name="apiKey">The API key given from the request.</param>
    /// <param name="operation">The <see cref="IOperation"/> to execute.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="IActionResult"/> object containing an error or result.</returns>
    protected Task<IActionResult> ExecuteOperationAsync(string apiKey, IOperation operation, CancellationToken cancellationToken = default)
    {
        ExecutionStatus status;

        try
        {
            status = operation.Execute(apiKey);
        }
        catch (OperationException ex) { return Task.FromResult(HandleOperationException(ex)); }

        return status != ExecutionStatus.Success ? Task.FromResult(HandleExecutionStatus(status)) : Task.FromResult<IActionResult>(Ok());
    }

    /// <summary>
    /// Execute the specified operation with the given key async.
    /// </summary>
    /// <remarks>This method is here for compatibility with async based ASP.NET Core methods, as I actually have no clue if ApiV2 got an async rework.</remarks>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="apiKey">The API key given from the request.</param>
    /// <param name="operation">The <see cref="IResultOperation{TResult}"/> to execute.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="IActionResult"/> object containing an error or result.</returns>
    protected Task<IActionResult> ExecuteOperationAsync<T>(string apiKey, IResultOperation<T> operation, CancellationToken cancellationToken = default)
    {
        ExecutionStatus status;

        try
        {
            status = operation.Execute(apiKey);
        }
        catch (OperationException ex) { return Task.FromResult(HandleOperationException(ex)); }

        return status != ExecutionStatus.Success ? Task.FromResult(HandleExecutionStatus(status)) : Task.FromResult<IActionResult>(Json(new { data = operation.Result }));
    }

    /// <summary>
    /// Execute the specified operation with the given key.
    /// </summary>
    /// <param name="operation">The <see cref="IOperation"/> to execute.</param>
    /// <returns>An <see cref="IActionResult"/> object containing an error or result.</returns>
    protected IActionResult ExecuteOperation(IOperation operation)
        => ExecuteOperation(GetApiKey(), operation);

    /// <summary>
    /// Execute the specified operation with the given key.
    /// </summary>
    /// <typeparam name="T">The type of result.</typeparam>
    /// <param name="operation">The <see cref="IResultOperation{TResult}"/> to execute.</param>
    /// <returns>An <see cref="IActionResult"/> object containing an error or result.</returns>
    protected IActionResult ExecuteOperation<T>(IResultOperation<T> operation)
        => ExecuteOperation(GetApiKey(), operation);

    /// <summary>
    /// Execute the specified operation with the given key async.
    /// </summary>
    /// <remarks>This method is here for compatibility with async based ASP.NET Core methods, as I actually have no clue if ApiV2 got an async rework.</remarks>
    /// <param name="operation">The <see cref="IOperation"/> to execute.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="IActionResult"/> object containing an error or result.</returns>
    protected async Task<IActionResult> ExecuteOperationAsync(IOperation operation, CancellationToken cancellationToken = default)
        => await ExecuteOperationAsync(GetApiKey(), operation, cancellationToken).ConfigureAwait(false);

    /// <summary>
    /// Execute the specified operation with the given key async.
    /// </summary>
    /// <remarks>This method is here for compatibility with async based ASP.NET Core methods, as I actually have no clue if ApiV2 got an async rework.</remarks>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="operation">The <see cref="IResultOperation{TResult}"/> to execute.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
    /// <returns>An <see cref="IActionResult"/> object containing an error or result.</returns>
    protected async Task<IActionResult> ExecuteOperationAsync<T>(IResultOperation<T> operation, CancellationToken cancellationToken = default)
        => await ExecuteOperationAsync(GetApiKey(), operation, cancellationToken).ConfigureAwait(false);
}
