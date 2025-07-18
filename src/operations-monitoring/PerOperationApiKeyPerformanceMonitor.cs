namespace Roblox.Operations.Monitoring;

using System;

using Instrumentation;

/// <summary>
/// Performance monitor per operation monitoring.
/// </summary>
public class PerOperationApiKeyPerformanceMonitor
{
    private const string _Category = "Roblox.Operations.Monitoring";

    /// <summary>
    /// The API key in the request was missing, or unauthorized.
    /// </summary>
    public IRawValueCounter UnauthorizedApiKeys { get; }

    /// <summary>
    /// An API key was included in the request, and it is authorized for the operation it is executing for.
    /// </summary>
    public IRawValueCounter AuthorizedApiKeys { get; }

    /// <summary>
    /// Construct a new instance of <see cref="PerOperationApiKeyPerformanceMonitor"/>
    /// </summary>
    /// <param name="counterRegistry">The <see cref="ICounterRegistry"/></param>
    /// <param name="operationName">The name of the operation.</param>
    /// <exception cref="ArgumentNullException"><paramref name="counterRegistry"/> cannot be null.</exception>
    /// <exception cref="ArgumentException"><paramref name="operationName"/> cannot be null or empty.</exception>
    public PerOperationApiKeyPerformanceMonitor(ICounterRegistry counterRegistry, string operationName)
    {
        if (counterRegistry == null) throw new ArgumentNullException(nameof(counterRegistry));
        if (string.IsNullOrEmpty(operationName)) throw new ArgumentException("operationName cannot be null or empty.", nameof(operationName));

        UnauthorizedApiKeys = counterRegistry.GetRawValueCounter(_Category, nameof(UnauthorizedApiKeys), operationName);
        AuthorizedApiKeys = counterRegistry.GetRawValueCounter(_Category, nameof(AuthorizedApiKeys), operationName);
    }
}
