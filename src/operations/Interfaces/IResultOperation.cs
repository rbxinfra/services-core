namespace Roblox.Operations;

/// <summary>
/// A synchronous operation interface without input.
/// </summary>
/// <typeparam name="TOutput">The output type for the operation.</typeparam>
public interface IResultOperation<TOutput>
{
    /// <summary>
    /// Executes the operation.
    /// </summary>
    /// <returns>The <typeparamref name="TOutput"/>/<see cref="OperationError"/> tuple.</returns>
    (TOutput Output, OperationError Error) Execute();
}

/// <summary>
/// A synchronous operation interface.
/// </summary>
/// <typeparam name="TInput">The input type for the operation.</typeparam>
/// <typeparam name="TOutput">The output type for the operation.</typeparam>
public interface IResultOperation<in TInput, TOutput>
{
    /// <summary>
    /// Executes the operation.
    /// </summary>
    /// <param name="input">The <typeparamref name="TInput"/>.</param>
    /// <returns>The <typeparamref name="TOutput"/>/<see cref="OperationError"/> tuple.</returns>
    (TOutput Output, OperationError Error) Execute(TInput input);
}
