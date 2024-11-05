#pragma warning disable IDE0060 // Remove unused parameter
#pragma warning disable IDE0130 // Namespace does not match folder structure

// ReSharper disable once CheckNamespace : Fake for Xunit Assert
namespace Xunit
{
    public class Assert
    {
        public void True(bool condition)
        {
        }

        public void True(bool condition, string? userMessage)
        {
        }
    }
}

#pragma warning restore IDE0130 // Namespace does not match folder structure
#pragma warning restore IDE0060 // Remove unused parameter
