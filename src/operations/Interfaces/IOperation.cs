namespace Roblox.Operations;

/// <summary>
/// A synchronous operation interface.
/// </summary>
/// <remarks>
/// Similar to <see cref="IOperation{TInput}"/> but does not have an input.
/// </remarks>
public interface IOperation
{
    /// <summary>
    /// Executes the operation.
    /// </summary>
    /// <returns>The <see cref="OperationError"/> (if the action failed, or <c>null</c>.)</returns>
    OperationError Execute();
}

/// <summary>
/// A synchronous operation interface.
/// </summary>
/// <remarks>
/// Similar to <see cref="IResultOperation{TInput,TOutput}"/> but does not have an output.
/// </remarks>
/// <typeparam name="TInput">The input type for the action.</typeparam>
public interface IOperation<in TInput>
{
    /// <summary>
    /// Executes the operation.
    /// </summary>
    /// <param name="input">The <typeparamref name="TInput"/>.</param>
    /// <returns>The <see cref="OperationError"/> (if the action failed, or <c>null</c>.)</returns>
    OperationError Execute(TInput input);
}
