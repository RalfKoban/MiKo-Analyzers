#pragma warning disable IDE0130 // Namespace does not match folder structure

using System;

// ReSharper disable once CheckNamespace
namespace NCrunch.Framework
{
    /// <summary>
    /// Specifies that the test fixture(s) marked with this attribute are considered to be <i>atomic</i> by NCrunch, meaning that their child tests cannot
    /// be run separately from each other.
    /// </summary>
    /// <remarks>
    /// A test being queued for execution under an atomic fixture will result in the entire fixture being queued with its child tests all executed in the
    /// same task/batch.
    /// </remarks>
    /// <seealso href="http://www.ncrunch.net/documentation/reference_runtime-framework_atomic-attribute"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false)]
    public class AtomicAttribute : Attribute
    {
    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
