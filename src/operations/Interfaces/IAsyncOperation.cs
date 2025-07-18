namespace Roblox.Operations;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A asynchronous operation interface.
/// </summary>
/// <remarks>
/// Similar to <see cref="IAsyncOperation{TInput}"/> but does not have an input.
/// </remarks>
public interface IAsyncOperation
{
    /// <summary>
    /// Executes the action.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A task returning the <see cref="OperationError"/> (if the action failed, or <c>null</c>.)</returns>
    Task<OperationError> ExecuteAsync(CancellationToken cancellationToken);
}

/// <summary>
/// A asynchronous operation interface.
/// </summary>
/// <remarks>
/// Similar to <see cref="IAsyncResultOperation{TInput,TOutput}"/> but does not have an output.
/// </remarks>
/// <typeparam name="TInput">The input type for the action.</typeparam>
public interface IAsyncOperation<in TInput>
{
    /// <summary>
    /// Executes the operation.
    /// </summary>
    /// <param name="input">The <typeparamref name="TInput"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A task returning the <see cref="OperationError"/> (if the action failed, or <c>null</c>.)</returns>
    Task<OperationError> ExecuteAsync(TInput input, CancellationToken cancellationToken);
}
