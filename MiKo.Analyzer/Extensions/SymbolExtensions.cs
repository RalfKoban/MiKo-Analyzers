using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Microsoft.CodeAnalysis
{
    internal static class SymbolExtensions
    {
        internal static bool IsEventHandler(this IMethodSymbol method)
        {
            var parameters = method.Parameters;
            return parameters.Length == 2 && parameters[0].Type.ToString() == "object" && parameters[1].Type.InheritsFrom<EventArgs>();
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

        internal static bool IsTestSetupMethod(this IMethodSymbol method)
        {
            foreach (var name in method.GetAttributes().Select(_ => _.AttributeClass.Name))
            {
                switch (name)
                {
                    case "SetUp":
                    case "SetUpAttribute":
                    case "TestInitialize":
                    case "TestInitializeAttribute":
                        return true;
                }
            }

            return false;
        }

        internal static bool IsTestTeardownMethod(this IMethodSymbol method)
        {
            foreach (var name in method.GetAttributes().Select(_ => _.AttributeClass.Name))
            {
                switch (name)
                {
                    case "TearDown":
                    case "TearDownAttribute":
                    case "TestCleanup":
                    case "TestCleanupAttribute":
                        return true;
                }
            }

            return false;
        }

        internal static bool IsSpecialAccessor(this IMethodSymbol method)
        {
            switch (method.MethodKind)
            {
                case MethodKind.EventAdd:
                case MethodKind.EventRemove:
                case MethodKind.PropertyGet:
                case MethodKind.PropertySet:
                case MethodKind.ExplicitInterfaceImplementation:
                    return true;
                default:
                    return false;
            }
        }

        internal static bool IsConstructor(this ISymbol symbol) => symbol?.Name == ".ctor";

        internal static bool IsClassConstructor(this ISymbol symbol) => symbol?.Name == ".cctor";

        internal static bool IsImportingConstructor(this ISymbol symbol)
        {
            foreach (var name in symbol.GetAttributes().Select(_ => _.AttributeClass.Name))
            {
                switch (name)
                {
                    case "ImportingConstructor":
                    case nameof(ImportingConstructorAttribute):
                        return true;
                }
            }

            return false;
        }

        internal static bool IsImport(this ISymbol symbol)
        {
            foreach (var name in symbol.GetAttributes().Select(_ => _.AttributeClass.Name))
            {
                switch (name)
                {
                    case "Import":
                    case nameof(ImportAttribute):
                    case "ImportMany":
                    case nameof(ImportManyAttribute):
                        return true;
                }
            }

            return false;
        }

        internal static bool InheritsFrom<T>(this ITypeSymbol symbol)
        {
            var baseClass = typeof(T).FullName;

            while (true)
            {
                if (symbol.ToString() == baseClass) return true;

                var baseType = symbol.BaseType;
                if (baseType == null) return false;

                symbol = baseType;
            }
        }

        internal static IEnumerable<ITypeSymbol> AllBaseTypes(this ITypeSymbol symbol)
        {
            var baseTypes = new List<ITypeSymbol>();
            while (true)
            {
                var baseType = symbol.BaseType;
                if (baseType == null) break;

                baseTypes.Add(baseType);
                symbol = baseType;
            }

            return baseTypes;
        }

        internal static bool IsTestClass(this ITypeSymbol symbol)
        {
            foreach (var name in symbol.GetAttributes().Select(_ => _.AttributeClass.Name))
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

        internal static bool IsEnum(this INamedTypeSymbol symbol) => symbol.EnumUnderlyingType != null;
    }
}