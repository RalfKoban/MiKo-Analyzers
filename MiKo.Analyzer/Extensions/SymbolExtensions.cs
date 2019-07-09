﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

// ReSharper disable once CheckNamespace
namespace MiKoSolutions.Analyzers
{
    internal static class SymbolExtensions
    {
        private static readonly HashSet<string> TestMethodAttributeNames = new HashSet<string>
                                                                               {
                                                                                   "Test",
                                                                                   "TestAttribute",
                                                                                   "TestCase",
                                                                                   "TestCaseAttribute",
                                                                                   "TestCaseSource",
                                                                                   "TestCaseSourceAttribute",
                                                                                   "Theory",
                                                                                   "TheoryAttribute",
                                                                                   "Fact",
                                                                                   "FactAttribute",
                                                                                   "TestMethod",
                                                                                   "TestMethodAttribute",
                                                                               };

        private static readonly HashSet<string> TestClassAttributeNames = new HashSet<string>
                                                                              {
                                                                                  "TestFixture",
                                                                                  "TestFixtureAttribute",
                                                                                  "TestClass",
                                                                                  "TestClassAttribute",
                                                                              };

        private static readonly HashSet<string> TestSetupAttributeNames = new HashSet<string>
                                                                              {
                                                                                  "SetUp",
                                                                                  "SetUpAttribute",
                                                                                  "TestInitialize",
                                                                                  "TestInitializeAttribute",
                                                                              };

        private static readonly HashSet<string> TestTearDownAttributeNames = new HashSet<string>
                                                                                 {
                                                                                     "TearDown",
                                                                                     "TearDownAttribute",
                                                                                     "TestCleanup",
                                                                                     "TestCleanupAttribute",
                                                                                 };

        private static readonly HashSet<string> ImportAttributeNames = new HashSet<string>
                                                                           {
                                                                               "Import",
                                                                               nameof(ImportAttribute),
                                                                               "ImportMany",
                                                                               nameof(ImportManyAttribute),
                                                                           };

        private static readonly HashSet<string> ImportingConstructorAttributeNames = new HashSet<string>
                                                                                         {
                                                                                             "ImportingConstructor",
                                                                                             nameof(ImportingConstructorAttribute),
                                                                                         };

        private static readonly string[] TypeUnderTestRawFieldNames =
            {
                "ObjectUnderTest",
                "objectUnderTest",
                "SubjectUnderTest",
                "subjectUnderTest",
                "Sut",
                "sut",
                "UnitUnderTest",
                "unitUnderTest",
                "Uut",
                "uut",
                "TestCandidate",
                "TestObject",
                "testCandidate",
                "testObject",
            };

        private static readonly string[] FieldPrefixes =
            {
                string.Empty,
                "_",
                "m_",
                "s_",
            };

        private static readonly HashSet<string> TypeUnderTestFieldNames = FieldPrefixes.SelectMany(_ => TypeUnderTestRawFieldNames, (prefix, name) => prefix + name).ToHashSet();

        private static readonly HashSet<string> TypeUnderTestPropertyNames = new HashSet<string>
                                                                                  {
                                                                                      "ObjectUnderTest",
                                                                                      "Sut",
                                                                                      "SuT",
                                                                                      "SUT",
                                                                                      "SubjectUnderTest",
                                                                                      "UnitUnderTest",
                                                                                      "Uut",
                                                                                      "UuT",
                                                                                      "UUT",
                                                                                      "TestCandidate",
                                                                                      "TestObject",
                                                                                  };

        private static readonly HashSet<string> TypeUnderTestMethodNames = new[] { "Create", "Get" }.SelectMany(_ => TypeUnderTestPropertyNames, (prefix, name) => prefix + name).ToHashSet();

        private static readonly SymbolDisplayFormat FullyQualifiedDisplayFormat = new SymbolDisplayFormat(
                                                                                                          globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
                                                                                                          typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                                                                                                          genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                                                                                                          miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        internal static bool IsEventHandler(this IMethodSymbol method)
        {
            var parameters = method.Parameters;
            return parameters.Length == 2 && parameters[0].Type.IsObject() && parameters[1].Type.IsEventArgs();
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

        internal static bool IsEnhancedByPostSharpAdvice(this ISymbol symbol) => symbol.GetAttributes().Any(_ => _.AttributeClass.InheritsFrom("PostSharp.Aspects.Advices.Advice"));

        internal static bool IsTestClass(this ITypeSymbol symbol) => symbol?.TypeKind == TypeKind.Class && symbol.GetAttributeNames().Any(TestClassAttributeNames.Contains);

        internal static bool IsTestMethod(this IMethodSymbol method) => method.GetAttributeNames().Any(TestMethodAttributeNames.Contains);

        internal static bool IsTestSetupMethod(this IMethodSymbol method) => method.GetAttributeNames().Any(TestSetupAttributeNames.Contains);

        internal static bool IsTestTeardownMethod(this IMethodSymbol method) => method.GetAttributeNames().Any(TestTearDownAttributeNames.Contains);

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

        internal static bool IsConstructor(this ISymbol symbol) => symbol is IMethodSymbol m && m.IsConstructor();

        internal static bool IsConstructor(this IMethodSymbol symbol) => symbol.MethodKind == MethodKind.Constructor;

        internal static bool IsSerializationConstructor(this IMethodSymbol symbol) => symbol.IsConstructor() && symbol.Parameters.Length == 2 && symbol.Parameters[0].IsSerializationInfoParameter() && symbol.Parameters[1].IsStreamingContextParameter();

        internal static bool IsSerializationInfoParameter(this IParameterSymbol parameter) => parameter.Type.Name == nameof(SerializationInfo);

        internal static bool IsStreamingContextParameter(this IParameterSymbol parameter) => parameter.Type.Name == nameof(StreamingContext);

        internal static bool IsImportingConstructor(this ISymbol symbol) => symbol.IsConstructor() && symbol.GetAttributeNames().Any(ImportingConstructorAttributeNames.Contains);

        internal static bool IsImport(this ISymbol symbol) => symbol.GetAttributeNames().Any(ImportAttributeNames.Contains);

        internal static bool InheritsFrom<T>(this ITypeSymbol symbol) => InheritsFrom(symbol, typeof(T).FullName);

        internal static bool InheritsFrom(this ITypeSymbol symbol, string baseClass)
        {
            while (true)
            {
                var fullName = symbol.ToString();
                if (baseClass == fullName)
                {
                    return true;
                }

                var baseType = symbol.BaseType;
                if (baseType is null)
                {
                    return false;
                }

                symbol = baseType;
            }
        }

        internal static bool InheritsFrom(this ITypeSymbol symbol, string baseClassName, string baseClassFullQualifiedName)
        {
            while (true)
            {
                var fullName = symbol.ToString();

                if (baseClassName == fullName)
                {
                    return true;
                }

                if (baseClassFullQualifiedName == fullName)
                {
                    return true;
                }

                var baseType = symbol.BaseType;
                if (baseType is null)
                {
                    return false;
                }

                symbol = baseType;
            }
        }

        internal static bool Implements<T>(this ITypeSymbol symbol) => Implements(symbol, typeof(T).FullName);

        internal static bool Implements(this ITypeSymbol symbol, string interfaceType)
        {
            if (symbol.ToString() == interfaceType)
            {
                return true;
            }

            foreach (var implementedInterface in symbol.AllInterfaces)
            {
                if (implementedInterface.ToString() == interfaceType)
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool Implements(this ITypeSymbol symbol, string interfaceTypeName, string interfaceTypeFullQualifiedName) => symbol.Implements(interfaceTypeName) || symbol.Implements(interfaceTypeFullQualifiedName);

        internal static bool ImplementsPotentialGeneric(this ITypeSymbol symbol, Type interfaceType) => ImplementsPotentialGeneric(symbol, interfaceType.FullName);

        internal static bool ImplementsPotentialGeneric(this ITypeSymbol symbol, string interfaceType)
        {
            var index = interfaceType.IndexOf('`');
            var interfaceTypeWithoutGeneric = index > -1
                                                  ? interfaceType.Substring(0, index)
                                                  : interfaceType;

            if (symbol.ToString().StartsWith(interfaceTypeWithoutGeneric, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            foreach (var implementedInterface in symbol.AllInterfaces)
            {
                if (implementedInterface.ToString().StartsWith(interfaceTypeWithoutGeneric, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsEventArgs(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Class
                                                                  && symbol.SpecialType == SpecialType.None
                                                                  && symbol.InheritsFrom<EventArgs>();

        internal static bool IsException(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Class
                                                                  && symbol.SpecialType == SpecialType.None
                                                                  && symbol.InheritsFrom<Exception>();

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
                    {
                        return true;
                    }

                    if (symbol.TypeKind == TypeKind.Array)
                    {
                        return true;
                    }

                    if (symbol.IsValueType)
                    {
                        return false;
                    }

                    if (symbol is INamedTypeSymbol s && IsEnumerable(s.ConstructedFrom.SpecialType))
                    {
                        return true;
                    }

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
            var baseTypes = new Queue<ITypeSymbol>(1);
            baseTypes.Enqueue(symbol);

            while (true)
            {
                var baseType = symbol.BaseType;
                if (baseType is null)
                {
                    break;
                }

                symbol = baseType;

                baseTypes.Enqueue(baseType);
            }

            return baseTypes;
        }

        internal static IEnumerable<ITypeSymbol> IncludingAllNestedTypes(this ITypeSymbol symbol)
        {
            var types = new Queue<ITypeSymbol>(1);
            types.Enqueue(symbol);

            CollectAllNestedTypes(symbol, types);

            return types;
        }

        internal static IEnumerable<TSymbol> GetMembersIncludingInherited<TSymbol>(this ITypeSymbol symbol) where TSymbol : ISymbol => symbol.IncludingAllBaseTypes().SelectMany(_ => _.GetMembers().OfType<TSymbol>());

        internal static bool IsEnum(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Enum;

        internal static bool IsTask(this ITypeSymbol symbol) => symbol?.Name == nameof(System.Threading.Tasks.Task);

        internal static INamedTypeSymbol FindContainingType(this SyntaxNodeAnalysisContext context) => FindContainingType(context.ContainingSymbol);

        internal static INamedTypeSymbol FindContainingType(this ISymbol symbol)
        {
            while (symbol != null)
            {
                if (symbol is INamedTypeSymbol s)
                {
                    return s;
                }

                symbol = symbol.ContainingType;
            }

            return null;
        }

        internal static bool IsFactory(this ITypeSymbol symbol) => symbol.Name.EndsWith("Factory", StringComparison.Ordinal) && symbol.Name.EndsWith("TaskFactory", StringComparison.Ordinal) is false; // ignore special situation for task factory

        internal static bool IsCancellationToken(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Struct && symbol.ToString() == TypeNames.CancellationToken;

        internal static bool IsNullable(this ITypeSymbol symbol) => symbol.IsValueType && symbol.Name == nameof(Nullable);

        internal static bool IsCommand(this ITypeSymbol symbol) => symbol.Implements<ICommand>();

        internal static bool IsAsyncTaskBased(this IMethodSymbol method) => method.IsAsync || method.ReturnType.IsTask();

        internal static bool IsObject(this ITypeSymbol symbol) => symbol.SpecialType == SpecialType.System_Object;

        internal static bool IsDependencyObject(this ITypeSymbol symbol) => symbol.InheritsFrom("DependencyObject", "System.Windows.DependencyObject");

        internal static bool IsDependencyProperty(this ITypeSymbol symbol) => symbol.Name == "DependencyProperty" || symbol.Name == "System.Windows.DependencyProperty";

        internal static bool IsDependencyPropertyKey(this ITypeSymbol symbol) => symbol.Name == "DependencyPropertyKey" || symbol.Name == "System.Windows.DependencyPropertyKey";

        internal static bool IsDependencyPropertyChangedEventArgs(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Struct
                                                                                              && symbol.SpecialType == SpecialType.None
                                                                                              && symbol.InheritsFrom("DependencyPropertyChangedEventArgs", "System.Windows.DependencyPropertyChangedEventArgs");

        internal static bool IsDependencyPropertyEventHandler(this IMethodSymbol method)
        {
            var parameters = method.Parameters;
            return parameters.Length == 2 && parameters[0].Type.IsDependencyObject() && parameters[1].Type.IsDependencyPropertyChangedEventArgs();
        }

        internal static bool HasDependencyObjectParameter(this IMethodSymbol method) => method.Parameters.Any(_ => _.Type.IsDependencyObject());

        internal static bool IsCoerceValueCallback(this IMethodSymbol method)
        {
            if (method.ReturnType.IsObject())
            {
                var parameters = method.Parameters;
                return parameters.Length == 2 && parameters[0].Type.IsDependencyObject() && parameters[1].Type.IsObject();
            }

            return false;
        }

        internal static bool IsValidateValueCallback(this IMethodSymbol method)
        {
            if (method.ReturnType.SpecialType == SpecialType.System_Boolean)
            {
                var parameters = method.Parameters;
                return parameters.Length == 1 && parameters[0].Type.IsObject();
            }

            return false;
        }

        internal static bool IsValueConverter(this ITypeSymbol symbol) => symbol.Implements("IValueConverter", "System.Windows.Data.IValueConverter") || symbol.InheritsFrom("IValueConverter", "System.Windows.Data.IValueConverter");

        internal static bool IsMultiValueConverter(this ITypeSymbol symbol) => symbol.Implements("IMultiValueConverter", "System.Windows.Data.IMultiValueConverter") || symbol.InheritsFrom("IMultiValueConverter", "System.Windows.Data.IMultiValueConverter");

        internal static bool ContainsExtensionMethods(this ITypeSymbol symbol) => symbol.TypeKind == TypeKind.Class && symbol.IsStatic && symbol.GetMembers().OfType<IMethodSymbol>().Any(_ => _.IsExtensionMethod);

        internal static string FullyQualifiedName(this ISymbol symbol) => symbol.ToDisplayString(FullyQualifiedDisplayFormat);

        internal static ITypeSymbol GetReturnType(this IPropertySymbol symbol) => symbol.GetMethod?.ReturnType ?? symbol.SetMethod?.Parameters[0].Type;

        internal static IEnumerable<ITypeSymbol> GetTypeUnderTestTypes(this ITypeSymbol symbol)
        {
            // TODO: RKN what about base types?
            var members = symbol.GetMembersIncludingInherited<ISymbol>().ToList();
            var methodTypes = members.OfType<IMethodSymbol>().Where(_ => _.ReturnsVoid is false).Where(_ => TypeUnderTestMethodNames.Contains(_.Name)).Select(_ => _.ReturnType);
            var propertyTypes = members.OfType<IPropertySymbol>().Where(_ => TypeUnderTestPropertyNames.Contains(_.Name)).Select(_ => _.GetReturnType());
            var fieldTypes = members.OfType<IFieldSymbol>().Where(_ => TypeUnderTestFieldNames.Contains(_.Name)).Select(_ => _.Type);

            return propertyTypes.Concat(fieldTypes).Concat(methodTypes);
        }

        internal static ITypeSymbol GetTypeUnderTestType(this ITypeSymbol symbol) => symbol.GetTypeUnderTestTypes().FirstOrDefault(_ => _ != null);

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

        internal static bool IsPartial(this INamedTypeSymbol symbol) => symbol.Locations.Length > 1;

        internal static bool TryGetGenericArgumentType(this ITypeSymbol symbol, out ITypeSymbol result, int index = 0)
        {
            result = null;

            if (symbol is INamedTypeSymbol namedType && namedType.TypeArguments.Length >= index + 1)
            {
                result = namedType.TypeArguments[index];
            }

            return result != null;
        }

        internal static bool TryGetGenericArgumentCount(this ITypeSymbol symbol, out int result)
        {
            result = 0;
            if (symbol is INamedTypeSymbol namedType)
            {
                result = namedType.TypeArguments.Length;
            }

            return result > 0;
        }

        internal static string GetGenericArgumentsAsTs(this ITypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol namedType)
            {
                var count = namedType.TypeArguments.Length;
                switch (count)
                {
                    case 0: return string.Empty;
                    case 1: return "T";
                    case 2: return "T1,T2";
                    case 3: return "T1,T2,T3";
                    case 4: return "T1,T2,T3,T4";
                    case 5: return "T1,T2,T3,T4,T5";
                    default: return Enumerable.Range(1, count).Select(_ => "T" + _).ConcatenatedWith(",");
                }
            }

            return string.Empty;
        }

        internal static string MinimalTypeName(this ITypeSymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        /// <summary>
        /// Determines if a <see cref="IPropertySymbol"/> of the containing type has the same name as the given <see cref="IParameterSymbol"/>.
        /// </summary>
        /// <param name="symbol">The symbol to inspect.</param>
        /// <returns>
        /// <see langword="true"/> if the containing <see cref="INamedTypeSymbol"/> contains a <see cref="IPropertySymbol"/> that matches the name of <paramref name="symbol"/>; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool MatchesProperty(this IParameterSymbol symbol) => symbol.ContainingType.GetMembersIncludingInherited<IPropertySymbol>().Any(_ => string.Equals(symbol.Name, _.Name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Determines if a <see cref="IFieldSymbol"/> of the containing type has the same name as the given <see cref="IParameterSymbol"/>.
        /// </summary>
        /// <param name="symbol">The symbol to inspect.</param>
        /// <returns>
        /// <see langword="true"/> if the containing <see cref="INamedTypeSymbol"/> contains a <see cref="IFieldSymbol"/> that matches the name of <paramref name="symbol"/>; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool MatchesField(this IParameterSymbol symbol)
        {
            var name = symbol.Name;
            var matchesField = symbol.ContainingType.GetMembersIncludingInherited<IFieldSymbol>()
                                     .Select(_ => _.Name)
                                     .Any(_ => FieldPrefixes.Select(__ => __ + name).Any(__ => string.Equals(_, __, StringComparison.OrdinalIgnoreCase)));
            return matchesField;
        }

        internal static int GetStartingLine(this IMethodSymbol method) => method.Locations.First(__ => __.IsInSource).GetLineSpan().StartLinePosition.Line;

        internal static string GetMethodSignature(this IMethodSymbol method)
        {
            var parameters = "(" + method.Parameters.Select(GetParameterSignature).ConcatenatedWith(",") + ")";
            var staticPrefix = method.IsStatic ? "static " : string.Empty;
            var asyncPrefix = method.IsAsync ? "async " : string.Empty;

            var methodName = GetMethodNameForKind(method);

            return string.Concat(staticPrefix, asyncPrefix, methodName, parameters);
        }

        private static void CollectAllNestedTypes(this ITypeSymbol symbol, Queue<ITypeSymbol> types)
        {
            types.Enqueue(symbol);

            foreach (var nestedType in symbol.GetTypeMembers())
            {
                CollectAllNestedTypes(nestedType, types);
            }
        }

        private static string GetMethodNameForKind(IMethodSymbol method)
        {
            switch (method.MethodKind)
            {
                case MethodKind.Constructor:
                case MethodKind.StaticConstructor:
                    return method.ContainingType.Name;

                default:
                {
                    var returnType = method.ReturnType.MinimalTypeName();

                    var suffix = method.IsGenericMethod ?
                                 string.Concat("<", method.TypeParameters.Select(MinimalTypeName).ConcatenatedWith(","), ">")
                                 : string.Empty;

                    return string.Concat(returnType, " ", method.Name, suffix);
                }
            }
        }

        private static string GetParameterSignature(IParameterSymbol parameter)
        {
            var modifier = GetModifierSignature(parameter);
            var parameterType = parameter.Type.MinimalTypeName();
            return modifier + parameterType;
        }

        private static string GetModifierSignature(IParameterSymbol parameter)
        {
            if (parameter.IsParams)
            {
                return "params ";
            }

            switch (parameter.RefKind)
            {
                case RefKind.Ref: return "ref ";
                case RefKind.Out: return "out ";
                default: return string.Empty;
            }
        }
    }
}