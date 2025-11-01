﻿#pragma warning disable IDE0130 // Namespace does not match folder structure

using System;

//// ReSharper disable once CheckNamespace
namespace NCrunch.Framework
{
    /// <summary>
    /// Specifies that the test marked with this attribute will be executed in a process that is spawned specifically for the test alone, as described
    /// <a href="http://www.ncrunch.net/documentation/reference_runtime-framework_isolated-attribute">here</a>.
    /// This class cannot be inherited.
    /// </summary>
    /// <seealso href="http://www.ncrunch.net/documentation/reference_runtime-framework_isolated-attribute"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class IsolatedAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsolatedAttribute"/> class.
        /// </summary>
        public IsolatedAttribute()
        {
        }
    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
