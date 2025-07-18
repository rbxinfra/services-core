namespace Roblox.ApiV2;

using System;

/// <summary>
/// Base contract for an operation consumed by the Api V2 stack.
/// </summary>
public interface IOperation
{
    /// <summary>
    /// Gets the Name of the operation used for authorization.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the execution time used for performance monitoring.
    /// </summary>
    TimeSpan ExecutionTime { get; }

    /// <summary>
    /// Execute the specified operation.
    /// </summary>
    /// <param name="apiKey">The API key to use for authentication and authorization.</param>
    /// <returns>Returns one of <see cref="ExecutionStatus"/></returns>
    ExecutionStatus Execute(string apiKey);

    /// <summary>
    /// Determines if the specified API key is authorized for this operation.
    /// </summary>
    /// <param name="apiKey">The API key to use for authorization.</param>
    /// <returns>Returns [true] if the API key is authorized, [false] otherwise.</returns>
    bool IsAuthorized(Guid apiKey);

    /// <summary>
    /// Determines if the operation is enabled or not (IsValid)
    /// </summary>
    /// <returns>[true] if the service is enabled, [false] otherwise.</returns>
    bool IsEnabled();

    /// <summary>
    /// Determines if the system should be throttled (global ratelimit)
    /// </summary>
    /// <returns>[true] if the system should be throttled, [false] otherwise.</returns>
    bool ShouldThrottleSystem();

    /// <summary>
    /// Determines if the specified client should be throttled (per client ratelimit)
    /// </summary>
    /// <param name="apiKey">The api key of the client</param>
    /// <returns>[true] if the system should be throttled, [false] otherwise.</returns>
    bool ShouldThrottleClient(Guid apiKey);
}

/// <summary>
/// Version of <see cref="IOperation"/> that takes a request
/// </summary>
/// <typeparam name="TRequest">The type of the request object.</typeparam>
public interface IOperation<TRequest>
{
    /// <summary>
    /// Gets the request for the operation.
    /// </summary>
    TRequest Request { get; }
}
