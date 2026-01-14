namespace Roblox.ApiV2;

using System;

/// <summary>
/// Exception thrown when an error occurs in an operation.
/// Use this for more details errors outside generic exceptions.
/// </summary>
public class OperationException : Exception
{
    /// <summary>
    /// Construct a new instance of <see cref="OperationException"/>
    /// </summary>
    /// <param name="message">The message to be given back to the client.</param>
    public OperationException(string message)
        : base(message)
    {}
}
