namespace Roblox.ApiV2;

using System;
using System.Diagnostics;

using Api.ControlPlane;

/// <summary>
/// Implementation for <see cref="IOperation"/>
/// </summary>
public abstract class Operation : IOperation
{
    private readonly IService _Service;
    private readonly IAuthority _Authority;

    /// <inheritdoc cref="IOperation.Name" />
    public abstract string Name { get; }

    /// <inheritdoc cref="IOperation.ExecutionTime" />
    public TimeSpan ExecutionTime { get; private set; }

    /// <summary>
    /// Construct a new instance of <see cref="Operation" />
    /// </summary>
    /// <param name="service">The service used for validation.</param>
    /// <param name="authority">The authority used for authentication and authorization.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="service"/> cannot be null.
    /// - <paramref name="authority"/> cannot be null.
    /// </exception>
    protected Operation(IService service, IAuthority authority)
    {
        _Service = service ?? throw new ArgumentNullException(nameof(service));
        _Authority = authority ?? throw new ArgumentNullException(nameof(authority));
    }

    /// <summary>
    /// Actually execute the operation.
    /// </summary>
    protected abstract void ExecuteOperation();

    /// <inheritdoc cref="IOperation.Execute(string)" />
    public ExecutionStatus Execute(string apiKey)
    {
        var sw = Stopwatch.StartNew();

        try
        {
            if (!Guid.TryParse(apiKey, out var clientKey)) return ExecutionStatus.ClientAuthenticationRejected;

            if (!_Service.IsEnabled()) return ExecutionStatus.ServiceDisabled;
            if (!IsEnabled()) return ExecutionStatus.OperationDisabled;

            if (!_Authority.Authenticate(clientKey)) return ExecutionStatus.ClientAuthenticationRejected;
            if (!IsAuthorized(clientKey)) return ExecutionStatus.ClientOperationRestricted;

            if (_Service.ShouldThrottleSystem()) return ExecutionStatus.ServiceThrottled;
            if (_Service.ShouldThrottleClient(clientKey)) return ExecutionStatus.ClientServiceThrottled;

            if (ShouldThrottleSystem()) return ExecutionStatus.OperationThrottled;
            if (ShouldThrottleClient(clientKey)) return ExecutionStatus.ClientOperationThrottled;

            ExecuteOperation();

            return ExecutionStatus.Success;
        }
        finally
        {
            sw.Stop();

            ExecutionTime = sw.Elapsed;
        }
    }

    /// <inheritdoc cref="IOperation.IsAuthorized(Guid)" />
    public virtual bool IsAuthorized(Guid apiKey)
        => _Authority.IsAuthorized(apiKey, _Service.Name, Name, out _);
    
    /// <inheritdoc cref="IOperation.IsEnabled()" />
    public virtual bool IsEnabled()
        => _Authority.OperationIsEnabled(_Service.Name, Name);

    /// <inheritdoc cref="IOperation.ShouldThrottleClient(Guid)" />
    public virtual bool ShouldThrottleClient(Guid apiKey) => false;

    /// <inheritdoc cref="IOperation.ShouldThrottleSystem()" />
    public virtual bool ShouldThrottleSystem() => false;
}

/// <summary>
/// Implementation of <see cref="IOperation"/> that takes a request object
/// </summary>
/// <typeparam name="TRequest">The type of the request object.</typeparam>
public abstract class Operation<TRequest> : Operation, IOperation<TRequest>
{
    /// <inheritdoc cref="IOperation{TRequest}.Request" />
    public TRequest Request { get; }

    /// <summary>
    /// Construct a new instance of <see cref="Operation" />
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="service">The service used for validation.</param>
    /// <param name="authority">The authority used for authentication and authorization.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="service"/> cannot be null.
    /// - <paramref name="authority"/> cannot be null.
    /// </exception>
    protected Operation(TRequest request, IService service, IAuthority authority)
        : base(service, authority)
    {
        Request = request;
    }
}
