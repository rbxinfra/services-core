namespace Roblox.ApiV2;

/// <summary>
/// Represents an operation that can take a result object.
/// </summary>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public interface IResultOperation<TResult> : IOperation
{
    /// <summary>
    /// Gets or sets the result of the operation.
    /// </summary>
    TResult Result { get; set; }
}

/// <summary>
/// Represents an operation that can take a result object as well as a request.
/// </summary>
/// <typeparam name="TRequest">The type of the request object.</typeparam>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public interface IResultOperation<TRequest, TResult> : IOperation<TRequest>, IResultOperation<TResult>
{
}
