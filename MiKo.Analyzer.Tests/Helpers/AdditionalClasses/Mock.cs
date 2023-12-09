#pragma warning disable IDE0130 // Namespace does not match folder structure
#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CA1805 // Do not initialize unnecessarily

using System;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace : Fake for Moq
namespace Moq
{
    public class Mock
    {
        public static T Of<T>() => default;
    }

    public class Mock<T>
    {
        public Mock()
        {
        }

        public void Setup(Expression<Action<T>> expression)
        {
            // nothing to do here
        }

        public void Verify(Expression<Action<T>> expression, Times times)
        {
            // nothing to do here
        }
    }

    public class Times
    {
        public static readonly Times Once = null;
    }
}

#pragma warning restore CA1805 // Do not initialize unnecessarily
#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore IDE0130 // Namespace does not match folder structure
