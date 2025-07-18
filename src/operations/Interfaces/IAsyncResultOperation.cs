namespace Roblox.Operations;

using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// A asynchronous operation interface without input.
/// </summary>
/// <typeparam name="TOutput">The output type for the operation.</typeparam>
public interface IAsyncResultOperation<TOutput>
{
    /// <summary>
    /// Executes the operation.
    /// </summary>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A task returning the <typeparamref name="TOutput"/>/<see cref="OperationError"/> tuple.</returns>
    /// <exception cref="TaskCanceledException"><paramref name="cancellationToken"/> is cancelled.</exception>
    Task<(TOutput Output, OperationError Error)> ExecuteAsync(CancellationToken cancellationToken);
}

/// <summary>
/// A asynchronous operation interface.
/// </summary>
/// <typeparam name="TInput">The input type for the operation.</typeparam>
/// <typeparam name="TOutput">The output type for the operation.</typeparam>
public interface IAsyncResultOperation<in TInput, TOutput>
{
    /// <summary>
    /// Executes the operation.
    /// </summary>
    /// <param name="input">The <typeparamref name="TInput"/>.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/>.</param>
    /// <returns>A task returning the <typeparamref name="TOutput"/>/<see cref="OperationError"/> tuple.</returns>
    /// <exception cref="TaskCanceledException"><paramref name="cancellationToken"/> is cancelled.</exception>
    Task<(TOutput Output, OperationError Error)> ExecuteAsync(TInput input, CancellationToken cancellationToken);
}
