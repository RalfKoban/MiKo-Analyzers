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
    /// <para />
    /// The attribute can be applied to both fixtures and tests, and also supports inheritance.
    /// The attribute can also be applied at assembly level, in which case all tests within the assembly will be considered as making inclusive use of the
    /// specified resources.
    /// <para />
    /// When declaring the attribute, you specify a sequence of string values that are used to identify the resources your test is relying on during its
    /// execution. Any tests that make exclusive use (via <see cref="ExclusivelyUsesAttribute"/>) of one of the specified resources will be considered mutually
    /// exclusive with the decorated test and will not be run at the same time by NCrunch.
    /// <para />
    /// <note>
    /// The resource name declared with <see cref="InclusivelyUsesAttribute"/> is effectively no more than an arbitrary mutex that prevents certain tests from
    /// running concurrently - it does not need to correspond to a physical resource on the system.
    /// Resource naming is done purely for the sake of convention.
    /// </note>
    /// </remarks>
    /// <seealso cref="ExclusivelyUsesAttribute"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InclusivelyUsesAttribute(params string[] resourceName) : ResourceUsageAttribute(resourceName)
    {
    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
