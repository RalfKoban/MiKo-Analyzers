#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0130 // Namespace does not match folder structure

// ReSharper disable once CheckNamespace : Fake for Xunit Assert
using System;

namespace Xunit
{
    public static class Assert
    {
        public static void True(bool condition)
        {
        }

        public static void True(bool condition, string? userMessage)
        {
        }

        public static void Equal<T>(T expected, T actual)
        {
        }

        public static T Throws<T>(Action testCode) where T : Exception => null;
    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
#pragma warning restore IDE0060 // Remove unused parameter
