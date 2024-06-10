#pragma warning disable IDE0130 // Namespace does not match folder structure

using System;

//// ReSharper disable once CheckNamespace
namespace NCrunch.Framework
{
    /// <summary>
    /// Specifies that the test marked with this attribute makes constrained use of a specific set of resources.
    /// </summary>
    /// <remarks>
    /// The primary reason for this is to prevent concurrent execution of tests that do not support it.
    /// </remarks>
    public abstract class ResourceUsageAttribute(params string[] resourceNames) : Attribute
    {
        //// ReSharper disable once MemberCanBePrivate.Global
        //// ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string[] ResourceNames { get; } = resourceNames;
    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
