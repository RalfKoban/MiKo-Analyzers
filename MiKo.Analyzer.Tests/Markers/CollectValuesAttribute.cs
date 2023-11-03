#pragma warning disable IDE0130 // Namespace does not match folder structure

using System;

// ReSharper disable once CheckNamespace
namespace NCrunch.Framework
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class CollectValuesAttribute : Attribute
    {
        public CollectValuesAttribute(bool collectValues) => Value = collectValues;

        public bool Value { get; private set; }
    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
