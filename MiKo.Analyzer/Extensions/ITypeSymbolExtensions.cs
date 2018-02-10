using System.Linq;

namespace Microsoft.CodeAnalysis
{
    internal static class ITypeSymbolExtensions
    {
        internal static bool InheritsFrom<T>(this ITypeSymbol symbol)
        {
            var baseClass = typeof(T).FullName;

            while (true)
            {
                if (symbol.ToString() == baseClass) return true;
                if (symbol.BaseType == null) return false;

                symbol = symbol.BaseType;
            }
        }

        internal static bool IsTestClass(this ITypeSymbol method)
        {
            foreach (var name in method.GetAttributes().Select(_ => _.AttributeClass.Name))
            {
                switch (name)
                {
                    case "TestFixture":
                    case "TestFixtureAttribute":
                    case "TestClass":
                    case "TestClassAttribute":
                        return true;
                }
            }

            return false;
        }
    }
}