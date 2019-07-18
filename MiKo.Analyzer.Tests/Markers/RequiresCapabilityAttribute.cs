using System;

//// ReSharper disable once CheckNamespace
namespace NCrunch.Framework
{
    /// <summary>
    /// Specifies that the test marked with this attribute can only be run on a machine configured to support the specified capability.
    /// </summary>
    /// <remarks>
    /// This attribute can be specified at test, fixture, or assembly level.
    /// When specified at assembly level, it will automatically apply to all tests within the assembly.
    /// <note>
    /// It is only useful when using NCrunch's distributed processing features.
    /// </note>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true)]
    public class RequiresCapabilityAttribute : Attribute
    {
        public RequiresCapabilityAttribute(string capabilityName) => CapabilityName = capabilityName;

        public string CapabilityName { get; }
    }
}