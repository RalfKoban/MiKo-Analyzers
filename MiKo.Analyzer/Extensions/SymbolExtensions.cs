using System;
using System.Collections;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SymbolExtensions
    {
        internal static bool IsEventHandler(this IMethodSymbol method)
        {
            var parameters = method.Parameters;
            return parameters.Length == 2 && parameters[0].Type.SpecialType == SpecialType.System_Object && parameters[1].Type.IsEventArgs();
        }

        internal static bool IsInterfaceImplementationOf<T>(this IMethodSymbol method)
        {
            var fullName = typeof(T).FullName;

            var typeSymbol = method.ContainingType;
            if (typeSymbol.Implements(fullName))
            {
                var methodSymbols = typeSymbol.AllInterfaces
                                          .Where(_ => _.Name == fullName)
                                          .SelectMany(_ => _.GetMembers().OfType<IMethodSymbol>());
                return methodSymbols.Any(_ => method.Equals(typeSymbol.FindImplementationForInterfaceMember(_)));
            }

            return false;
        }

        internal static bool IsInterfaceImplementation<TSymbol>(this TSymbol symbol) where TSymbol : ISymbol
        {
            var symbols = symbol.ContainingType.AllInterfaces
                                .SelectMany(_ => _.GetMembers().OfType<TSymbol>());
            return symbols.Any(_ => symbol.Equals(symbol.ContainingType.FindImplementationForInterfaceMember(_)));
        }

        internal static IEnumerable<string> GetAttributeNames(this ISymbol symbol) => symbol.GetAttributes().Select(_ => _.AttributeClass.Name);

        internal static bool IsTestClass(this ITypeSymbol symbol)
        {
            if (symbol?.TypeKind == TypeKind.Class)
            {
                foreach (var name in symbol.GetAttributeNames())
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
            }

            return false;
        }

        internal static bool IsTestMethod(this IMethodSymbol method)
        {
            foreach (var name in method.GetAttributeNames())
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
            foreach (var name in method.GetAttributeNames())
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
            foreach (var name in method.GetAttributeNames())
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

        internal static bool IsConstructor(this ISymbol symbol) => symbol is IMethodSymbol m && m.MethodKind == MethodKind.Constructor;

        internal static bool IsImportingConstructor(this ISymbol symbol)
        {
            if (!symbol.IsConstructor()) return false;

            foreach (var name in symbol.GetAttributeNames())
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
            foreach (var name in symbol.GetAttributeNames())
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

        internal static bool InheritsFrom<T>(this ITypeSymbol symbol) => InheritsFrom(symbol, typeof(T).FullName);

        internal static bool InheritsFrom(this ITypeSymbol symbol, params string[] baseClasses)
        {
            while (true)
            {
                var fullName = symbol.ToString();

                foreach (var baseClass in baseClasses)
                {
                    if (baseClass == fullName) return true;
                }

                var baseType = symbol.BaseType;
                if (baseType == null) return false;

                symbol = baseType;
            }
        }

        internal static bool Implements<T>(this ITypeSymbol symbol) => Implements(symbol, typeof(T).FullName);

        internal static bool Implements(this ITypeSymbol symbol, string interfaceType)
        {
            if (symbol.ToString() == interfaceType) return true;

            foreach (var implementedInterface in symbol.AllInterfaces)
            {
                if (implementedInterface.ToString() == interfaceType) return true;
            }

            return false;
        }

        internal static bool ImplementsPotentialGeneric(this ITypeSymbol symbol, Type interfaceType) => ImplementsPotentialGeneric(symbol, interfaceType.FullName);

        internal static bool ImplementsPotentialGeneric(this ITypeSymbol symbol, string interfaceType)
        {
            var index = interfaceType.IndexOf('`');
            var interfaceTypeWithoutGeneric = index > -1
                                                  ? interfaceType.Substring(0, index)
                                                  : interfaceType;

            if (symbol.ToString().StartsWith(interfaceTypeWithoutGeneric, StringComparison.OrdinalIgnoreCase))
                return true;

            foreach (var implementedInterface in symbol.AllInterfaces)
            {
                if (implementedInterface.ToString().StartsWith(interfaceTypeWithoutGeneric, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        internal static bool IsEventArgs(this ITypeSymbol symbol)
        {
            if (symbol.TypeKind != TypeKind.Class) return false;
            if (symbol.SpecialType != SpecialType.None) return false;

            return symbol.InheritsFrom<EventArgs>();
        }

        internal static bool IsException(this ITypeSymbol symbol)
        {
            if (symbol.TypeKind != TypeKind.Class) return false;
            if (symbol.SpecialType != SpecialType.None) return false;

            return symbol.InheritsFrom<Exception>();
        }

        internal static bool IsEnumerable(this ITypeSymbol symbol)
        {
            switch (symbol.SpecialType)
            {
                case SpecialType.System_Void:
                case SpecialType.System_Boolean:
                case SpecialType.System_Char:
                case SpecialType.System_SByte:
                case SpecialType.System_Byte:
                case SpecialType.System_Int16:
                case SpecialType.System_UInt16:
                case SpecialType.System_Int32:
                case SpecialType.System_UInt32:
                case SpecialType.System_Int64:
                case SpecialType.System_UInt64:
                case SpecialType.System_Decimal:
                case SpecialType.System_Single:
                case SpecialType.System_Double:
                case SpecialType.System_String:
                case SpecialType.System_IntPtr:
                case SpecialType.System_UIntPtr:
                case SpecialType.System_DateTime:
                    return false;

                default:
                    if (IsEnumerable(symbol.SpecialType))
                        return true;

                    if (symbol is INamedTypeSymbol s && IsEnumerable(s.ConstructedFrom.SpecialType))
                        return true;

                    return symbol.Implements<IEnumerable>();
            }
        }

        internal static bool IsEnumerable(this SpecialType specialType)
        {
            switch (specialType)
            {
                case SpecialType.System_Array:
                case SpecialType.System_Collections_IEnumerable:
                case SpecialType.System_Collections_Generic_IEnumerable_T:
                case SpecialType.System_Collections_Generic_IList_T:
                case SpecialType.System_Collections_Generic_ICollection_T:
                case SpecialType.System_Collections_Generic_IReadOnlyList_T:
                case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    return true;

                default:
                    return false;
            }
        }

        internal static IEnumerable<ITypeSymbol> IncludingAllBaseTypes(this ITypeSymbol symbol)
        {
            var baseTypes = new List<ITypeSymbol> { symbol };
            while (true)
            {
                var baseType = symbol.BaseType;
                if (baseType == null) break;

                baseTypes.Add(baseType);
                symbol = baseType;
            }

            return baseTypes;
        }

        internal static IEnumerable<ITypeSymbol> IncludingAllNestedTypes(this ITypeSymbol symbol)
        {
            var types = new List<ITypeSymbol> { symbol };
            CollectAllNestedTypes(symbol, types);
            return types;
        }

        internal static bool IsEnum(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Enum;

        internal static bool IsTask(this ITypeSymbol symbol) => symbol?.Name == nameof(System.Threading.Tasks.Task);

        internal static INamedTypeSymbol FindContainingType(this SyntaxNodeAnalysisContext context) => FindContainingType(context.ContainingSymbol);

        internal static INamedTypeSymbol FindContainingType(this ISymbol symbol)
        {
            while (symbol != null)
            {
                if (symbol is INamedTypeSymbol s) return s;

                symbol = symbol.ContainingType;
            }

            return null;
        }

        internal static bool IsFactory(this ITypeSymbol symbol) => symbol.Name.EndsWith("Factory", StringComparison.Ordinal) && symbol.Name.EndsWith("TaskFactory", StringComparison.Ordinal) == false; // ignore special situation for task factory

        internal static bool IsCancellationToken(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Struct && symbol.ToString() == TypeNames.CancellationToken;

        internal static bool IsNullable(this ITypeSymbol symbol) => symbol.IsValueType && symbol.Name == nameof(Nullable);

        internal static ISymbol GetSymbol(this SyntaxToken token, SemanticModel semanticModel)
        {
            var position = token.GetLocation().SourceSpan.Start;
            var name = token.ValueText;

            if (token.Parent is ParameterSyntax node)
            {
                // we might have a ctor here and no nethod
                var methodName = node.GetEnclosing<MethodDeclarationSyntax>()?.Identifier.ValueText ?? node.GetEnclosing<ConstructorDeclarationSyntax>()?.Identifier.ValueText;
                var methodSymbols = semanticModel.LookupSymbols(position, name: methodName).OfType<IMethodSymbol>();
                var parameterSymbol = methodSymbols.SelectMany(_ => _.Parameters).FirstOrDefault(_ => _.Name == name);
                return parameterSymbol;

                // if it's no method parameter, then it is a local one (but Roslyn cannot handle that currently)
                //var symbol = semanticModel.LookupSymbols(position).First(_ => _.Kind == SymbolKind.Local);
            }

            return semanticModel.LookupSymbols(position, name: name).First();
        }

        internal static ISymbol GetEnclosingSymbol(this SyntaxNode node, SemanticModel semanticModel)
        {
            switch (node)
            {
                case MethodDeclarationSyntax s:
                    return semanticModel.GetDeclaredSymbol(s);
                case PropertyDeclarationSyntax p:
                    return semanticModel.GetDeclaredSymbol(p);
                case ConstructorDeclarationSyntax c:
                    return semanticModel.GetDeclaredSymbol(c);
                case FieldDeclarationSyntax f:
                    return semanticModel.GetDeclaredSymbol(f);
                case EventDeclarationSyntax e:
                    return semanticModel.GetDeclaredSymbol(e);
                default:
                    return semanticModel.GetEnclosingSymbol(node.GetLocation().SourceSpan.Start);
            }
        }

        internal static IMethodSymbol GetEnclosingMethod(this SyntaxNode node, SemanticModel semanticModel) => node.GetEnclosingSymbol(semanticModel) as IMethodSymbol;

        internal static T GetEnclosing<T>(this SyntaxNode node) where T : SyntaxNode
        {
            while (true)
            {
                switch (node)
                {
                    case null: return null;
                    case T t: return t;
                }

                node = node.Parent;
            }
        }

        internal static SyntaxNode GetEnclosing(this SyntaxNode node, params SyntaxKind[] syntaxKinds)
        {
            while (true)
            {
                if (node == null)
                    return null;

                foreach (var syntaxKind in syntaxKinds)
                {
                    if (node.IsKind(syntaxKind))
                        return node;
                }

                node = node.Parent;
            }
        }

        internal static bool IsCommand(this IErrorTypeSymbol symbol) => symbol.Name == nameof(ICommand);

        internal static bool IsCommand(this ITypeSymbol symbol)
        {
            if (symbol.Implements<ICommand>())
                return true;

            // TODO: refactor this as we do this for tests
            return symbol.IncludingAllBaseTypes().Concat(symbol.AllInterfaces).OfType<IErrorTypeSymbol>().Any(IsCommand);
        }

        internal static bool IsCommand(this TypeSyntax syntax, SemanticModel semanticModel)
        {
            var name = syntax.ToString();

            return name.Contains("Command")
                && semanticModel.LookupSymbols(syntax.GetLocation().SourceSpan.Start, name: name).FirstOrDefault() is ITypeSymbol symbol
                && symbol.IsCommand();
        }

        internal static bool IsAsyncTaskBased(this IMethodSymbol method) => method.IsAsync || method.ReturnType.IsTask();

        internal static bool IsString(this ExpressionSyntax syntax, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(syntax);
            return typeInfo.Type?.SpecialType == SpecialType.System_String;
        }

        internal static bool IsStruct(this ExpressionSyntax syntax, SemanticModel semanticModel)
        {
            var typeInfo = semanticModel.GetTypeInfo(syntax);
            switch (typeInfo.Type?.TypeKind)
            {
                case TypeKind.Struct:
                case TypeKind.Enum:
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsDependencyObject(this ITypeSymbol symbol) => symbol.InheritsFrom("DependencyObject", "System.Windows.DependencyObject");

        internal static bool HasDependencyObjectParameter(this IMethodSymbol method) => method.Parameters.Any(_ => _.Type.IsDependencyObject());

        internal static bool IsDependencyProperty(this ITypeSymbol symbol) => symbol.Name == "DependencyProperty" || symbol.Name == "System.Windows.DependencyProperty";

        internal static bool IsDependencyPropertyKey(this ITypeSymbol symbol) => symbol.Name == "DependencyPropertyKey" || symbol.Name == "System.Windows.DependencyPropertyKey";

        internal static bool IsValueConverter(this ITypeSymbol symbol) => symbol.InheritsFrom("IValueConverter", "System.Windows.Data.IValueConverter");

        internal static bool IsMultiValueConverter(this ITypeSymbol symbol) => symbol.InheritsFrom("IMultiValueConverter", "System.Windows.Data.IMultiValueConverter");

        internal static bool ContainsExtensionMethods(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Class && symbol.IsStatic && symbol.GetMembers().OfType<IMethodSymbol>().Any(_ => _.IsExtensionMethod);

        internal static ITypeSymbol GetReturnType(this IPropertySymbol symbol) => symbol.GetMethod?.ReturnType ?? symbol.SetMethod?.Parameters[0].Type;

        internal static IEnumerable<MemberAccessExpressionSyntax> GetAssignmentsVia(this IFieldSymbol symbol, string invocation)
        {
            return symbol.DeclaringSyntaxReferences
                         .Select(_ => _.GetSyntax())
                         .Select(_ => _.GetEnclosing<FieldDeclarationSyntax>())
                         .SelectMany(_ => _.DescendantNodes()
                                           .OfType<MemberAccessExpressionSyntax>()
                                           .Where(__ => __.ToCleanedUpString() == invocation));
        }

        internal static SeparatedSyntaxList<ArgumentSyntax> GetInvocationArgumentsFrom(this IFieldSymbol symbol, string invocation) => symbol.GetAssignmentsVia(invocation)
                                                                                                                                             .Select(_ => _.GetEnclosing<InvocationExpressionSyntax>())
                                                                                                                                             .Select(_ => _.ArgumentList.Arguments)
                                                                                                                                             .FirstOrDefault(_ => _.Count > 0);

        internal static string ToCleanedUpString(this ExpressionSyntax s) => s?.ToString().RemoveAll(Constants.WhiteSpaces);

        internal static bool IsPartial(this INamedTypeSymbol symbol) => symbol.Locations.Length > 1;

        internal static bool IsInsideIfStatementWithCallTo(this SyntaxNode node, string methodName)
        {
            var ifStatement = GetEnclosingIfStatement(node);
            var ifExpression = ifStatement?.ChildNodes().OfType<MemberAccessExpressionSyntax>().FirstOrDefault();

            var inside = ifExpression?.Name.ToString() == methodName;
            return inside;
        }

        private static IfStatementSyntax GetEnclosingIfStatement(SyntaxNode node)
        {
            // consider brackets:
            //                    if (true)
            //                    {
            //                      xyz();
            //                    }
            //
            //  and no brackets:
            //                    if (true)
            //                      xyz();
            //
            var enclosingNode = node.GetEnclosing(SyntaxKind.Block, SyntaxKind.IfStatement);
            if (enclosingNode is BlockSyntax)
                enclosingNode = enclosingNode.Parent;

            return enclosingNode as IfStatementSyntax;
        }

        private static void CollectAllNestedTypes(this ITypeSymbol symbol, ICollection<ITypeSymbol> types)
        {
            types.Add(symbol);

            foreach (var nestedType in symbol.GetTypeMembers())
            {
                CollectAllNestedTypes(nestedType, types);
            }
        }
    }
}