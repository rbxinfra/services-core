namespace Roblox.ApiV2;

/// <summary>
/// Represents the execution status of an operation.
/// </summary>
public enum ExecutionStatus
{
    /// <summary>
    /// Status returned if <see cref="IService.IsEnabled()"/> returned [false]
    /// </summary>
    ServiceDisabled,

    /// <summary>
    /// Status returned if <see cref="IOperation.IsEnabled()"/> returned [false].
    /// </summary>
    OperationDisabled,

    /// <summary>
    /// Status returned if the client cannot be determined, e.g. no/invalid api key or non existent client.
    /// </summary>
    ClientAuthenticationRejected,

    /// <summary>
    /// Result returned if the client that was found has no authorization for the specified service/operation.
    /// </summary>
    ClientOperationRestricted,

    /// <summary>
    /// Result returned if the executing <see cref="IService"/> returns [true] throttling the system.
    /// </summary>
    ServiceThrottled,

    /// <summary>
    /// Result returned if the executing <see cref="IOperation"/> returns [true] throttling the system.
    /// </summary>
    OperationThrottled,

    /// <summary>
    /// Result returned if the executing <see cref="IService"/> returns [true] throttling the client.
    /// </summary>
    ClientServiceThrottled,

    /// <summary>
    /// Result returned if the executing <see cref="IService"/> returns [true] throttling the client.
    /// </summary>
    ClientOperationThrottled,

    /// <summary>
    /// Result returned if pre-checks were successful.
    /// </summary>
    Success
}
