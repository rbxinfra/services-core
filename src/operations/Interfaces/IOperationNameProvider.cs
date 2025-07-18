namespace Roblox.Operations;

using System;
using System.ComponentModel;

/// <summary>
/// A provider of names for operations.
/// </summary>
public interface IOperationNameProvider
{
    /// <summary>
    /// The operation type.
    /// </summary>
    /// <remarks>
    /// The <see cref="DisplayNameAttribute"/> is looked up on the <paramref name="operationType"/>,
    /// and if it is found the value is returned.
    ///
    /// After that, if the class name is suffixed with "Action" or "Operation", it is stripped
    /// and the preceding string of the class name is returned.
    /// 
    /// The type is expected to be the class inheriting one of:
    /// - <see cref="IResultOperation{TOutput}"/>
    /// - <see cref="IResultOperation{TInput,TOutput}"/>
    /// - <see cref="IAsyncResultOperation{TOutput}"/>
    /// - <see cref="IAsyncResultOperation{TInput,TOutput}"/>
    /// - <see cref="IOperation"/>
    /// - <see cref="IOperation{TInput}"/>
    /// - <see cref="IOperation"/>
    /// - <see cref="IOperation{TInput}"/>
    /// </remarks>
    /// <param name="operationType">The operation type.</param>
    /// <returns>The name of the operation.</returns>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="operationType"/>
    /// </exception>
    string GetOperationName(Type operationType);
}
