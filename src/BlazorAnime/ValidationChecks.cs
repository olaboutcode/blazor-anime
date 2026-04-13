using System.Runtime.CompilerServices;

namespace BlazorAnime;

public static class ValidationChecks
{
    public static void EnsureValidProp(string name, object value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentNullException.ThrowIfNull(value, nameof(value));
    }


    public static void EnsureAcceptableCallback(Delegate callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        bool isLambda = callback.Method.IsDefined(typeof(CompilerGeneratedAttribute), false);
        if (isLambda)
        {
            throw new ArgumentException(
                "Lambda expressions are not supported as callbacks. Please use a named method.");
        }

        if (callback.GetInvocationList().Length > 1)
        {
            throw new ArgumentException(
                "Multicast delegates are not supported as callbacks. Please use a single method.");
        }

        if (callback.Target == null)
        {
            throw new ArgumentException(
                "Static methods are not supported as callbacks. Please use an instance method.");
        }
    }
}
