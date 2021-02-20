using System;

//// ReSharper disable once CheckNamespace
namespace NCrunch.Framework
{
    /// <summary>
    /// Specifies that the test marked with this attribute will be executed by NCrunch using distributed processing.
    /// </summary>
    /// <remarks>
    /// The <see cref="DistributeByCapabilitiesAttribute"/> can be applied to tests, fixtures, or assemblies to simplify the use of NCrunch for multi-platform
    /// testing using distributed processing.
    /// <para />
    /// When NCrunch detects this attribute, it will automatically create multiple clones of the target test(s), one for each parameter passed into the
    /// attribute.
    /// Each clone will be marked with a required capability equal to their source parameter.
    /// <para />
    /// The test clones are effectively treated as separate tests by the entirety of the NCrunch engine.
    /// Throughout the UI, they are distinctively marked with a suffix describing their required capability.
    /// </remarks>
    public class DistributeByCapabilitiesAttribute : Attribute
    {
        public DistributeByCapabilitiesAttribute(params string[] capabilities) => Capabilities = capabilities;

        //// ReSharper disable once MemberCanBePrivate.Global
        //// ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string[] Capabilities { get; }
    }
}