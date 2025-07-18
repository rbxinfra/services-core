namespace Roblox.ApiV2;

using System;

using rbxops = Roblox.Operations;

using Operations;
using Api.ControlPlane;

/// <summary>
/// Wrapper for an <see cref="rbxops::IOperation" /> for Phase1 migration
/// to services framework (Web Core, see WEBCORE-188276)
/// </summary>
public class OperationWrapper : Operation
{
    private readonly rbxops::IOperation _MigrationOperation;
    private readonly IOperationNameProvider _OperationNameProvider;

    /// <inheritdoc cref="IOperation.Name" />
    public override string Name => _OperationNameProvider.GetOperationName(_MigrationOperation.GetType());

    /// <summary>
    /// Construct a new instance of <see cref="Operation" />
    /// </summary>
    /// <param name="migrationOperation">The <see cref="rbxops::IOperation"/>.</param>
    /// <param name="service">The service used for validation.</param>
    /// <param name="authority">The authority used for authentication and authorization.</param>
    /// <param name="operationNameProvider">The <see cref="IOperationNameProvider"/>, if null specified it will use the default one.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="migrationOperation"/> cannot be null.
    /// - <paramref name="service"/> cannot be null.
    /// - <paramref name="authority"/> cannot be null.
    /// </exception>
    public OperationWrapper(rbxops::IOperation migrationOperation, IService service, IAuthority authority, IOperationNameProvider operationNameProvider = null)
        : base(service, authority)
    {
        _MigrationOperation = migrationOperation ?? throw new ArgumentNullException(nameof(migrationOperation));
        _OperationNameProvider = operationNameProvider ?? new OperationNameProvider();
    }

    /// <inheritdoc cref="Operation.ExecuteOperation()" />
    protected override void ExecuteOperation()
    {
        var operationError = _MigrationOperation.Execute();

        if (operationError != null)
            throw new OperationException(operationError.Message ?? operationError.Code);
    }
}

/// <summary>
/// Wrapper for an <see cref="rbxops::IOperation{TRequest}" /> for Phase1 migration
/// to services framework (Web Core, see WEBCORE-188276)
/// </summary>
public class OperationWrapper<TRequest> : Operation<TRequest>
{
    private readonly rbxops::IOperation<TRequest> _MigrationOperation;
    private readonly IOperationNameProvider _OperationNameProvider;

    /// <inheritdoc cref="IOperation.Name" />
    public override string Name => _OperationNameProvider.GetOperationName(_MigrationOperation.GetType());

    /// <summary>
    /// Construct a new instance of <see cref="Operation" />
    /// </summary>
    /// <param name="migrationOperation">The <see cref="rbxops::IOperation"/>.</param>
    /// <param name="request">The request object.</param>
    /// <param name="service">The service used for validation.</param>
    /// <param name="authority">The authority used for authentication and authorization.</param>
    /// <param name="operationNameProvider">The <see cref="IOperationNameProvider"/>, if null specified it will use the default one.</param>
    /// <exception cref="ArgumentNullException">
    /// - <paramref name="migrationOperation"/> cannot be null.
    /// - <paramref name="service"/> cannot be null.
    /// - <paramref name="authority"/> cannot be null.
    /// </exception>
    public OperationWrapper(rbxops::IOperation<TRequest> migrationOperation, TRequest request, IService service, IAuthority authority, IOperationNameProvider operationNameProvider = null)
        : base(request, service, authority)
    {
        _MigrationOperation = migrationOperation ?? throw new ArgumentNullException(nameof(migrationOperation));
        _OperationNameProvider = operationNameProvider ?? new OperationNameProvider();
    }

    /// <inheritdoc cref="Operation.ExecuteOperation()" />
    protected override void ExecuteOperation()
    {
        var operationError = _MigrationOperation.Execute(Request);

        if (operationError != null)
            throw new OperationException(operationError.Message ?? operationError.Code);
    }
}
