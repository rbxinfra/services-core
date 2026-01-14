namespace Roblox.ApiV2;

using System;

using rbxops = Roblox.Operations;

using Operations;
using Api.ControlPlane;

/// <summary>
/// Wrapper for an <see cref="rbxops::IResultOperation{TResult}" /> for Phase1 migration
/// to services framework (Web Core, see WEBCORE-188276)
/// </summary>
public class ResultOperationWrapper<TResult> : ResultOperation<TResult>
{
    private readonly rbxops::IResultOperation<TResult> _MigrationOperation;
    private readonly IOperationNameProvider _OperationNameProvider;

    /// <inheritdoc cref="IOperation.Name" />
    public override string Name => _OperationNameProvider.GetOperationName(_MigrationOperation.GetType());

    /// <summary>
    /// Construct a new instance of <see cref="Operation" />
    /// </summary>
    /// <param name="migrationOperation">The <see cref="rbxops::IResultOperation{TResult}"/>.</param>
    /// <param name="service">The service used for validation.</param>
    /// <param name="authority">The authority used for authentication and authorization.</param>
    /// <param name="operationNameProvider">The <see cref="IOperationNameProvider"/>, if null specified it will use the default one.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="migrationOperation"/> cannot be null.
    /// - <paramref name="service"/> cannot be null.
    /// - <paramref name="authority"/> cannot be null.
    /// </exception>
    public ResultOperationWrapper(rbxops::IResultOperation<TResult> migrationOperation, IService service, IAuthority authority, IOperationNameProvider operationNameProvider = null)
        : base(service, authority)
    {
        _MigrationOperation = migrationOperation ?? throw new ArgumentNullException(nameof(migrationOperation));
        _OperationNameProvider = operationNameProvider ?? new OperationNameProvider();
    }

    /// <inheritdoc cref="Operation.ExecuteOperation()" />
    protected override void ExecuteOperation()
    {
        var (result, operationError) = _MigrationOperation.Execute();

        if (operationError != null)
            throw new OperationException(operationError.Message ?? operationError.Code.ToString());

        Result = result;
    }
}

/// <summary>
/// Wrapper for an <see cref="rbxops::IResultOperation{TRequest, TResult}" /> for Phase1 migration
/// to services framework (Web Core, see WEBCORE-188276)
/// </summary>
public class ResultOperationWrapper<TRequest, TResult> : ResultOperation<TRequest, TResult>
{
    private readonly rbxops::IResultOperation<TRequest, TResult> _MigrationOperation;
    private readonly IOperationNameProvider _OperationNameProvider;

    /// <inheritdoc cref="IOperation.Name" />
    public override string Name => _OperationNameProvider.GetOperationName(_MigrationOperation.GetType());

    /// <summary>
    /// Construct a new instance of <see cref="Operation" />
    /// </summary>
    /// <param name="migrationOperation">The <see cref="rbxops::IResultOperation{TInput, TOutput}"/>.</param>
    /// <param name="request">The request object.</param>
    /// <param name="service">The service used for validation.</param>
    /// <param name="authority">The authority used for authentication and authorization.</param>
    /// <param name="operationNameProvider">The <see cref="IOperationNameProvider"/>, if null specified it will use the default one.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="migrationOperation"/> cannot be null.
    /// - <paramref name="service"/> cannot be null.
    /// - <paramref name="authority"/> cannot be null.
    /// </exception>
    public ResultOperationWrapper(rbxops::IResultOperation<TRequest, TResult> migrationOperation, TRequest request, IService service, IAuthority authority, IOperationNameProvider operationNameProvider = null)
        : base(request, service, authority)
    {
        _MigrationOperation = migrationOperation ?? throw new ArgumentNullException(nameof(migrationOperation));
        _OperationNameProvider = operationNameProvider ?? new OperationNameProvider();
    }

    /// <inheritdoc cref="Operation.ExecuteOperation()" />
    protected override void ExecuteOperation()
    {
        var (result, operationError) = _MigrationOperation.Execute(Request);

        if (operationError != null)
            throw new OperationException(operationError.Message ?? operationError.Code.ToString());

        Result = result;
    }
}
