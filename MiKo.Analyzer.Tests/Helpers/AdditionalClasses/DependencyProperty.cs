#pragma warning disable IDE0060 // Remove unused parameter (IDE0060)
#pragma warning disable IDE0130 // Namespace does not match folder structure

// ReSharper disable once CheckNamespace : Fake for DependencyProperty
namespace System.Windows
{
    public class DependencyProperty
    {
        public static DependencyProperty Register(string name, Type valueType, Type controlType, PropertyMetadata metadata) => null;
    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
#pragma warning restore IDE0060 // Remove unused parameter (IDE0060)
