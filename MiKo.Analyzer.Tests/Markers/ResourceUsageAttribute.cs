﻿using System;

//// ReSharper disable once CheckNamespace
namespace NCrunch.Framework
{
    /// <summary>
    /// Specifies that the test marked with this attribute makes constrained use of a specific set of resources.
    /// </summary>
    /// <remarks>
    /// The primary reason for this is to prevent concurrent execution of tests that do not support it.
    /// </remarks>
    public abstract class ResourceUsageAttribute : Attribute
    {
        protected ResourceUsageAttribute(params string[] resourceNames) => ResourceNames = resourceNames;

        //// ReSharper disable once MemberCanBePrivate.Global
        //// ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string[] ResourceNames { get; }
    }
}