namespace Roblox.ApiV2;

using System;

using Api.ControlPlane;

/// <summary>
/// Default implementation for <see cref="IResultOperation{TResult}"/>
/// </summary>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public abstract class ResultOperation<TResult> : Operation, IResultOperation<TResult>
{
    /// <inheritdoc cref="IResultOperation{TResult}.Result" />
    public TResult Result { get; set; }

    /// <summary>
    /// Construct a new instance of <see cref="ResultOperation{TResult}" />
    /// </summary>
    /// <param name="service">The service used for validation.</param>
    /// <param name="authority">The authority used for authentication and authorization.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="service"/> cannot be null.
    /// - <paramref name="authority"/> cannot be null.
    /// </exception>
    protected ResultOperation(IService service, IAuthority authority)
        : base(service, authority)
    {
    }
}

/// <summary>
/// Default implementation for <see cref="IResultOperation{TRequest, TResult}"/>
/// </summary>
/// <typeparam name="TRequest">The type of the request object.</typeparam>
/// <typeparam name="TResult">The type of the result object.</typeparam>
public abstract class ResultOperation<TRequest, TResult> : Operation<TRequest>, IResultOperation<TRequest, TResult>
{
    /// <inheritdoc cref="IResultOperation{TResult}.Result" />
    public TResult Result { get; set; }

    /// <summary>
    /// Construct a new instance of <see cref="ResultOperation{TResult}" />
    /// </summary>
    /// <param name="request">The request object.</param>
    /// <param name="service">The service used for validation.</param>
    /// <param name="authority">The authority used for authentication and authorization.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="service"/> cannot be null.
    /// - <paramref name="authority"/> cannot be null.
    /// </exception>
    protected ResultOperation(TRequest request, IService service, IAuthority authority)
        : base(request, service, authority)
    {
    }
}


