namespace Roblox.Operations;

using System;
using System.Reflection;
using System.ComponentModel;

/// <inheritdoc cref="IOperationNameProvider"/>
public class OperationNameProvider : IOperationNameProvider
{
    private const string _OperationSuffix = "Operation";
    private const string _ActionSuffix = "Action";

    /// <inheritdoc cref="IOperationNameProvider.GetOperationName"/>
    public string GetOperationName(Type operationType)
    {
        if (operationType == null) throw new ArgumentNullException(nameof(operationType));

        var name = operationType.Name;
        var displayName = operationType.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

        if (!string.IsNullOrWhiteSpace(displayName?.DisplayName))
            name = displayName.DisplayName;
        else if (name.EndsWith(_OperationSuffix))
            name = name.Substring(0, name.Length - _OperationSuffix.Length);
        else if (name.EndsWith(_ActionSuffix))
            name = name.Substring(0, name.Length - _ActionSuffix.Length);

        return name;
    }
}
