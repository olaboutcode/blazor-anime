namespace BlazorAnime;

internal static class ValidationChecks
{
    internal static void EnsureValidProp(string name, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentNullException.ThrowIfNull(value, nameof(value));
    }

    internal static void EnsureAcceptableCallback(Delegate callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (callback.GetInvocationList().Length > 1)
        {
            throw new ArgumentException(
                "Multicast delegates are not supported as callbacks. Please use a single method.");
        }
    }
}
