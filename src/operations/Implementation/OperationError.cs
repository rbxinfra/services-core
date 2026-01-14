namespace Roblox.Operations;

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

using Newtonsoft.Json;

/// <summary>
/// The operation error model.
/// </summary>
[DataContract]
public class OperationError
{
    /// <summary>
    /// The error code.
    /// </summary>
    [DataMember(Name = "code")]
    public Enum Code { get; set; }

    /// <summary>
    /// The error message.
    /// </summary>
    [DataMember(Name = "message")]
    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Message { get; set; }

    /// <summary>
    /// Initializes a new <see cref="OperationError"/>.
    /// </summary>
    /// <param name="codeEnum">The <see cref="Message"/> as an enum.</param>
    /// <param name="formatArgs">The arguments to use when formatting the message.</param>
    public OperationError(Enum codeEnum, params object[] formatArgs)
    {
        if (codeEnum == null) throw new ArgumentNullException(nameof(codeEnum));

        var code = EnumToString(codeEnum, out var hasDescription);

        if (hasDescription) 
            Message = formatArgs is { Length: > 0 } ? string.Format(code, formatArgs) : code;
        else 
            Code = codeEnum;
    }

    /// <summary>
    /// Initializes a new <see cref="OperationError"/>.
    /// </summary>
    /// <param name="message">The <see cref="Message"/>.</param>
    /// <param name="formatArgs">The arguments to use when formatting the message.</param>
    public OperationError(string message, params object[] formatArgs)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(message));

        Message = formatArgs is { Length: > 0 } ? string.Format(message, formatArgs) : message;
    }

    private static string EnumToString(Enum codeEnum, out bool hasDescription)
    {
        // Try to use the DescriptionAttribute if it exists.
        var description = codeEnum.GetType().GetField(codeEnum.ToString())?.GetCustomAttributes(typeof(DescriptionAttribute), false);

        hasDescription = description?.Length > 0;

        return hasDescription ? ((DescriptionAttribute)description?[0])?.Description : codeEnum.ToString();
    }

    /// <summary>
    /// Constructor exists to be able to be used with Newtonsoft
    /// </summary>
    internal OperationError()
    {
    }
}
