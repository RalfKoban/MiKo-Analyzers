using System.Linq;

namespace Microsoft.CodeAnalysis
{
    internal static class IMethodSymbolExtensions
    {
        internal static bool IsEventHandler(this IMethodSymbol method)
        {
            var parameters = method.Parameters;
            return parameters.Length == 2 && parameters[0].Type.ToString() == "object" && parameters[1].Type.InheritsFrom<System.EventArgs>();
        }

        internal static bool IsInterfaceImplementationOf<T>(this IMethodSymbol method)
        {
            var methodSymbols = method.ContainingType.AllInterfaces
                                      .Where(_ => _.Name == typeof(T).FullName)
                                      .SelectMany(_ => _.GetMembers().OfType<IMethodSymbol>())
                                      .ToList();
            return methodSymbols.Any(_ => method.ContainingType.FindImplementationForInterfaceMember(_).Equals(method));
        }

        internal static bool IsTestMethod(this IMethodSymbol method)
        {
            foreach (var name in method.GetAttributes().Select(_ => _.AttributeClass.Name))
            {
                switch (name)
                {
                    case "Test":
                    case "TestAttribute":
                    case "TestCase":
                    case "TestCaseAttribute":
                    case "TestCaseSource":
                    case "TestCaseSourceAttribute":
                    case "Theory":
                    case "TheoryAttribute":
                    case "Fact":
                    case "FactAttribute":
                    case "TestMethod":
                    case "TestMethodAttribute":
                        return true;
                }
            }

            return false;
        }
    }
}