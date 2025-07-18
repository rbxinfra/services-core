namespace Roblox.ApiV2;

using System;

/// <summary>
/// Contract for a service
/// </summary>
public interface IService
{
    /// <summary>
    /// Gets the name of the service.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Determines if the service is enabled or not (IsValid)
    /// </summary>
    /// <returns>[true] if the service is enabled, [false] otherwise.</returns>
    bool IsEnabled();

    /// <summary>
    /// Determines if the system should be throttled (global ratelimit)
    /// </summary>
    /// <returns>[true] if the system should be throttled, [false] otherwise.</returns>
    bool ShouldThrottleSystem();

    /// <summary>
    /// Determines if the specified client should be throttled (per client ratelimit)
    /// </summary>
    /// <param name="apiKey">The api key of the client</param>
    /// <returns>[true] if the system should be throttled, [false] otherwise.</returns>
    bool ShouldThrottleClient(Guid apiKey);
}
