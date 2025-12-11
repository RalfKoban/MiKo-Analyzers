#pragma warning disable IDE0130
#pragma warning disable CA1813
#pragma warning disable SA1402
#pragma warning disable SA1403

using System;
using System.Diagnostics.CodeAnalysis;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class Throw
    {
        [DoesNotReturn]
        internal static void ArgumentOutOfRange(string paramName, in int paramValue, string message) => throw new ArgumentOutOfRangeException(paramName, paramValue, message);

        [DoesNotReturn]
        internal static void InvalidOperation(string message) => throw new InvalidOperationException(message);
    }
}

#if !NETCOREAPP3_0_OR_GREATER
namespace System.Diagnostics.CodeAnalysis
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DoesNotReturnAttribute : Attribute
    {
    }
}
#endif
