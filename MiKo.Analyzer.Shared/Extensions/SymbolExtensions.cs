﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

// ncrunch: rdi off
// ReSharper disable once CheckNamespace
#pragma warning disable IDE0130
#pragma warning disable CA1506
namespace MiKoSolutions.Analyzers
{
    internal static class SymbolExtensions
    {
        private static readonly SymbolDisplayFormat FullyQualifiedDisplayFormat = new SymbolDisplayFormat(
                                                                                                      SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
                                                                                                      SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                                                                                                      SymbolDisplayGenericsOptions.IncludeTypeParameters,
                                                                                                      miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

        private static readonly SymbolDisplayFormat FullyQualifiedDisplayFormatWithoutAlias = new SymbolDisplayFormat(
                                                                                                                  SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
                                                                                                                  SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                                                                                                                  SymbolDisplayGenericsOptions.IncludeTypeParameters,
                                                                                                                  miscellaneousOptions: SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers);

        private static readonly Func<SyntaxNode, bool> IsLocalFunctionContainer = IsLocalFunctionContainerCore;

        internal static IEnumerable<IMethodSymbol> GetExtensionMethods(this ITypeSymbol value) => value.GetMethods().Where(_ => _.IsExtensionMethod);

        internal static IReadOnlyList<IMethodSymbol> GetMethods(this ITypeSymbol value) => value.GetMembers<IMethodSymbol>();

        internal static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol value, MethodKind kind)
        {
            // note that methods with MethodKind.Constructor cannot be referenced by name
            var methods = kind is MethodKind.Ordinary
                          ? value.GetNamedMethods()
                          : value.GetMethods();

            var count = methods.Count;

            if (count > 0)
            {
                for (var index = 0; index < count; index++)
                {
                    var method = methods[index];

                    if (method.MethodKind == kind)
                    {
                        yield return method;
                    }
                }
            }
        }

        internal static IReadOnlyList<IMethodSymbol> GetMethods(this ITypeSymbol value, string methodName)
        {
            var members = value.GetMembers(methodName);

            var length = members.Length;

            if (length > 0)
            {
                var results = new List<IMethodSymbol>(length);
                results.AddRange(members.OfType<IMethodSymbol>());

                return results;
            }

            return Array.Empty<IMethodSymbol>();
        }

        /// <summary>
        /// Gets all methods that can be referenced by name.
        /// </summary>
        /// <param name="value">
        /// The type whose methods are wanted.
        /// </param>
        /// <returns>
        /// A collection of methods (that can be referenced by name).
        /// </returns>
        /// <remarks>
        /// <note type="important">
        /// Methods with <see cref="MethodKind.Constructor"/> or <see cref="MethodKind.StaticConstructor"/> cannot be referenced by name and therefore are not part of the result.
        /// </note>
        /// </remarks>
        internal static IReadOnlyList<IMethodSymbol> GetNamedMethods(this ITypeSymbol value)
        {
            var methods = value.GetMethods();

            if (methods.Count > 0)
            {
                return GetNamedSymbols(methods);
            }

            return Array.Empty<IMethodSymbol>();
        }

        internal static IReadOnlyList<IPropertySymbol> GetProperties(this ITypeSymbol value)
        {
            var properties = value.GetMembers<IPropertySymbol>();

            if (properties.Count > 0)
            {
                return GetNamedSymbols(properties);
            }

            return Array.Empty<IPropertySymbol>();
        }

        internal static IReadOnlyList<IFieldSymbol> GetFields(this ITypeSymbol value)
        {
            var fields = value.GetMembers<IFieldSymbol>();

            if (fields.Count > 0)
            {
                return GetNamedSymbols(fields);
            }

            return Array.Empty<IFieldSymbol>();
        }

        internal static IEnumerable<IFieldSymbol> GetFields(this ITypeSymbol value, string fieldName)
        {
            var members = value.GetMembers(fieldName);

            if (members.Length > 0)
            {
                return members.OfType<IFieldSymbol>().ToList();
            }

            return Array.Empty<IFieldSymbol>();
        }

        internal static IReadOnlyList<LocalFunctionStatementSyntax> GetLocalFunctions(this IMethodSymbol value)
        {
            var node = value.GetSyntaxNodeInSource();

            if (node is null)
            {
                return Array.Empty<LocalFunctionStatementSyntax>();
            }

            List<LocalFunctionStatementSyntax> functions = null;

            foreach (var descendantNode in node.DescendantNodes(IsLocalFunctionContainer))
            {
                if (descendantNode is LocalFunctionStatementSyntax function)
                {
                    if (functions is null)
                    {
                        functions = new List<LocalFunctionStatementSyntax>(1);
                    }

                    functions.Add(function);
                }
            }

            return functions ?? (IReadOnlyList<LocalFunctionStatementSyntax>)Array.Empty<LocalFunctionStatementSyntax>();
        }

        internal static bool ContainsExtensionMethods(this INamedTypeSymbol value) => value.TypeKind is TypeKind.Class && value.IsStatic && value.MightContainExtensionMethods && value.GetExtensionMethods().Any();

        internal static INamedTypeSymbol FindContainingType(this in SyntaxNodeAnalysisContext value) => FindContainingType(value.ContainingSymbol);

        internal static INamedTypeSymbol FindContainingType(this ISymbol value) => value as INamedTypeSymbol ?? value?.ContainingType;

        internal static string FullyQualifiedName(this ISymbol value, in bool useAlias = true)
        {
            switch (value)
            {
                case null: // currently I do not know how to reproduce the situation here, but I've seen AD0001 reported for that
                    return string.Empty;

                case IMethodSymbol m:
                    return m.ContainingType.FullyQualifiedName(useAlias) + "." + m.Name;

                case IPropertySymbol p:
                    return p.ContainingType.FullyQualifiedName(useAlias) + "." + p.Name;

                case INamedTypeSymbol t when useAlias is false:
                    return t.ToDisplayString(FullyQualifiedDisplayFormatWithoutAlias);

                case IAssemblySymbol a:
                    return a.Identity.GetDisplayName(); // use short display name

                default:
                    return value.ToDisplayString(FullyQualifiedDisplayFormat); // makes use of aliases for language such as 'int' instead of 'System.Int32'
            }
        }

        internal static IEnumerable<MemberAccessExpressionSyntax> GetAssignmentsVia(this IFieldSymbol value, string invocation)
        {
            var field = value.GetSyntax<FieldDeclarationSyntax>();

            if (field is null)
            {
                return Array.Empty<MemberAccessExpressionSyntax>();
            }

            return field.DescendantNodes<MemberAccessExpressionSyntax>(_ => _.ToCleanedUpString() == invocation);
        }

        internal static string[] GetAttributeNames(this ISymbol value) => value.GetAttributes().ToArray(_ => _.AttributeClass?.Name);

        internal static IEnumerable<ObjectCreationExpressionSyntax> GetCreatedObjectSyntaxReturnedByMethod(this IMethodSymbol value)
        {
            var method = (MethodDeclarationSyntax)value.GetSyntax();

            foreach (var createdObject in method.DescendantNodes<ObjectCreationExpressionSyntax>())
            {
                var parent = createdObject.Parent;

                switch (parent?.Kind())
                {
                    case SyntaxKind.ArrowExpressionClause:
                    case SyntaxKind.ReturnStatement:
                    {
                        if (parent.Parent == method)
                        {
                            yield return createdObject;
                        }

                        break;
                    }
                }
            }
        }

        // TODO RKN: find better name
        internal static IEnumerable<ObjectCreationExpressionSyntax> GetCreatedObjectSyntaxReturnedByMethods(this ITypeSymbol value)
        {
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var method in value.GetNamedMethods())
            {
                if (IsTypeUnderTestCreationMethod(method))
                {
                    var items = method.GetCreatedObjectSyntaxReturnedByMethod();

                    foreach (var item in items)
                    {
                        yield return item;
                    }
                }
            }
        }

        internal static IMethodSymbol GetEnclosingMethod(this ISymbol value)
        {
            var symbol = value;

            while (true)
            {
                switch (symbol)
                {
                    case null: return null;
                    case IMethodSymbol method: return method;
                    case IPropertySymbol property:
                    {
                        if (property.IsIndexer)
                        {
                            return property.GetMethod ?? property.SetMethod;
                        }

                        return property.SetMethod;
                    }

                    case INamespaceOrTypeSymbol _: return null;
                    case IEventSymbol _: return null;
                    case IFieldSymbol _: return null;

                    default:
                        // parameters might be part of properties or methods, so we have to do the loop here
                        symbol = symbol.ContainingSymbol;

                        break;
                }
            }
        }

        internal static string GetGenericArgumentsAsTs(this ITypeSymbol value) => value is INamedTypeSymbol n
                                                                                  ? n.GetGenericArgumentsAsTs()
                                                                                  : string.Empty;

        internal static string GetGenericArgumentsAsTs(this INamedTypeSymbol value)
        {
            var count = value.TypeArguments.Length;

            switch (count)
            {
                case 0: return string.Empty;
                case 1: return "T";
                case 2: return "T1,T2";
                case 3: return "T1,T2,T3";
                case 4: return "T1,T2,T3,T4";
                case 5: return "T1,T2,T3,T4,T5";
                case 6: return "T1,T2,T3,T4,T5,T6";
                case 7: return "T1,T2,T3,T4,T5,T6,T7";
                default: return Enumerable.Range(1, count).Select(_ => string.Concat("T", _.ToString())).ConcatenatedWith(",");
            }
        }

        internal static SeparatedSyntaxList<ArgumentSyntax> GetInvocationArgumentsFrom(this IFieldSymbol value, string invocation) => value.GetAssignmentsVia(invocation)
                                                                                                                                           .Select(_ => _.GetEnclosing<InvocationExpressionSyntax>())
                                                                                                                                           .Select(_ => _.ArgumentList.Arguments)
                                                                                                                                           .FirstOrDefault(_ => _.Count > 0);

        internal static IReadOnlyList<TSymbol> GetMembers<TSymbol>(this ITypeSymbol value) where TSymbol : ISymbol
        {
            var members = value.GetMembers();
            var length = members.Length;

            if (length is 0)
            {
                return Array.Empty<TSymbol>();
            }

            var results = new List<TSymbol>(length);

            for (var index = 0; index < length; index++)
            {
                if (members[index] is TSymbol member && member.IsImplicitlyDeclared is false)
                {
                    results.Add(member);
                }
            }

            return results;
        }

        internal static IEnumerable<TSymbol> GetMembersIncludingInherited<TSymbol>(this ITypeSymbol value) where TSymbol : ISymbol
        {
            foreach (var type in value.IncludingAllBaseTypes())
            {
                var members = type.GetMembers<TSymbol>();
                var count = members.Count;

                if (count > 0)
                {
                    for (var index = 0; index < count; index++)
                    {
                        var member = members[index];

                        if (member.CanBeReferencedByName)
                        {
                            yield return member;
                        }
                    }
                }
            }
        }

        internal static IEnumerable<TSymbol> GetMembersIncludingInherited<TSymbol>(this ITypeSymbol value, string name) where TSymbol : ISymbol
        {
            foreach (var type in value.IncludingAllBaseTypes())
            {
                var members = type.GetMembers(name);
                var length = members.Length;

                if (length > 0)
                {
                    for (var index = 0; index < length; index++)
                    {
                        if (members[index] is TSymbol member && member.IsImplicitlyDeclared is false)
                        {
                            yield return member;
                        }
                    }
                }
            }
        }

        internal static StringBuilder GetMethodSignature(this IMethodSymbol value, StringBuilder builder)
        {
            if (value.IsStatic)
            {
                builder.Append("static ");
            }

            if (value.IsAsync)
            {
                builder.Append("async ");
            }

            AppendMethodNameForKind(value, builder);
            AppendParameters(value.Parameters, builder);

            return builder;

            void AppendMethodNameForKind(IMethodSymbol method, StringBuilder sb)
            {
                switch (method.MethodKind)
                {
                    case MethodKind.Constructor:
                    case MethodKind.StaticConstructor:
                    {
                        sb.Append(method.ContainingType.Name);

                        break;
                    }

                    default:
                    {
                        var returnType = method.ReturnType.MinimalTypeName();

                        sb.Append(returnType).Append(' ').Append(method.Name);

                        if (method.IsGenericMethod)
                        {
                            sb.Append('<');

                            var typeParameters = method.TypeParameters;

                            for (int i = 0, count = typeParameters.Length - 1; i <= count; i++)
                            {
                                sb.Append(typeParameters[i].MinimalTypeName());

                                if (i < count)
                                {
                                    sb.Append(',');
                                }
                            }

                            sb.Append('>');
                        }

                        break;
                    }
                }
            }

            void AppendParameters(ImmutableArray<IParameterSymbol> parameters, StringBuilder sb)
            {
                sb.Append('(');

                for (int i = 0, count = parameters.Length - 1; i <= count; i++)
                {
                    AppendParameterSignature(parameters[i], sb);

                    if (i < count)
                    {
                        sb.Append(',');
                    }
                }

                sb.Append(')');
            }

            void AppendParameterSignature(IParameterSymbol parameter, StringBuilder sb)
            {
                sb.Append(GetModifierSignature(parameter));
                sb.Append(parameter.Type.MinimalTypeName());
            }

            string GetModifierSignature(IParameterSymbol parameter)
            {
                if (parameter.IsParams)
                {
                    return "params ";
                }

                switch (parameter.RefKind)
                {
                    case RefKind.Ref: return "ref ";
                    case RefKind.Out: return "out ";
                    case RefKind.In: return "in ";
                    default: return string.Empty;
                }
            }
        }

        internal static SyntaxToken GetModifier(this IMethodSymbol value, in SyntaxKind kind)
        {
            switch (value.GetSyntax())
            {
                case BaseMethodDeclarationSyntax method: return method.Modifiers.First(kind);
                case AccessorDeclarationSyntax accessor: return accessor.Modifiers.First(kind);

                default:
                    return default;
            }
        }

        internal static SyntaxToken GetModifier(this IParameterSymbol value, in SyntaxKind kind)
        {
            var syntax = value.GetSyntax();

            return syntax.Modifiers.First(kind);
        }

        internal static ITypeSymbol GetReturnType(this IPropertySymbol value) => value.GetMethod?.ReturnType ?? value.SetMethod?.Parameters[0].Type;

        internal static int GetStartingLine(this IMethodSymbol value) => value.Locations.First(_ => _.IsInSource).GetStartingLine();

        internal static SyntaxNode GetSyntaxNodeInSource(this ISymbol value) => value.GetSyntaxNodeInSource<SyntaxNode>();

        internal static TSyntaxNode GetSyntaxNodeInSource<TSyntaxNode>(this ISymbol value) where TSyntaxNode : SyntaxNode
        {
            var references = value.DeclaringSyntaxReferences;
            var length = references.Length;

            if (length > 0)
            {
                var extension = Constants.CSharpFileExtension.AsSpan();

                for (var index = 0; index < length; index++)
                {
                    if (references[index].GetSyntax() is TSyntaxNode node)
                    {
                        // ignore non C# code (might be part of partial classes, e.g. for XAML)
                        var filePath = node.SyntaxTree.FilePath;
                        var filePathSpan = filePath.AsSpan();

                        // Perf: quick catch via span and ordinal comparison (as that is the most likely case)
                        if (filePathSpan.EndsWith(extension, StringComparison.Ordinal))
                        {
                            // MaximumGeneratedCSharpFileExtensionLength
                            var pathLength = filePathSpan.Length;
                            var remainingPathLength = pathLength - Constants.MaximumGeneratedCSharpFileExtensionLength;

                            var span = filePathSpan.Slice(0, pathLength - extension.Length);

                            if (remainingPathLength > 0)
                            {
                                span = span.Slice(remainingPathLength);
                            }

                            // Perf: quick catch find another dot, as we do not need to check for additional extensions in case we do not have any other dot here
                            if (span.LastIndexOfAny('.', Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) >= 0)
                            {
                                if (filePathSpan.EndsWithAny(Constants.GeneratedCSharpFileExtensions, StringComparison.Ordinal))
                                {
                                    // ignore generated code (might be part of partial classes)
                                    continue;
                                }
                            }

                            return node;
                        }

                        // non-performant part: use string and ignore case
                        if (filePathSpan.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                        {
                            if (filePath.EndsWithAny(Constants.GeneratedCSharpFileExtensions, StringComparison.OrdinalIgnoreCase))
                            {
                                // ignore generated code (might be part of partial classes)
                                continue;
                            }

                            return node;
                        }
                    }
                }
            }

            return null;
        }

        internal static SyntaxNode GetSyntax(this ISymbol value)
        {
            switch (value)
            {
                case IFieldSymbol field:
                {
                    // maybe it is an enum member
                    if (field.ContainingType.IsEnum())
                    {
                        return GetSyntax<EnumMemberDeclarationSyntax>(field);
                    }

                    return GetSyntax<FieldDeclarationSyntax>(field);
                }

                case IEventSymbol @event:
                {
                    var eventField = GetSyntax<EventFieldDeclarationSyntax>(@event);

                    if (eventField != null)
                    {
                        return eventField;
                    }

                    return GetSyntax<EventDeclarationSyntax>(@event);
                }

                case IParameterSymbol parameter:
                    return GetSyntax(parameter);

                default:
                    return value?.GetSyntaxNodeInSource();
            }
        }

        internal static ParameterSyntax GetSyntax(this IParameterSymbol value) => value.GetSyntaxNodeInSource<ParameterSyntax>();

        internal static T GetSyntax<T>(this ISymbol value) where T : SyntaxNode
        {
            var node = value.GetSyntaxNodeInSource();

            return node?.GetEnclosing<T>();
        }

        internal static bool HasDocumentationCommentTriviaSyntax(this ISymbol value) => value.GetSyntax()?.HasDocumentationCommentTriviaSyntax() ?? false;

        internal static DocumentationCommentTriviaSyntax[] GetDocumentationCommentTriviaSyntax(this ISymbol value) => value.GetSyntax()?.GetDocumentationCommentTriviaSyntax() ?? Array.Empty<DocumentationCommentTriviaSyntax>();

        internal static ITypeSymbol GetReturnTypeSymbol(this ISymbol value)
        {
            switch (value)
            {
                case IFieldSymbol field: return field.Type;
                case IMethodSymbol method: return method.ReturnType;
                case IPropertySymbol property: return property.Type;
                default: return null;
            }
        }

        internal static IReadOnlyCollection<ISymbol> GetTypeUnderTestMembers(this ITypeSymbol value)
        {
            // TODO: RKN what about base types?
            var members = value.GetMembersIncludingInherited<ISymbol>().ToList();
            var methodTypes = GetTestCreationMethods(members);
            var propertyTypes = members.OfType<IPropertySymbol>().Where(_ => Constants.Names.TypeUnderTestPropertyNames.Contains(_.Name));
            var fieldTypes = members.OfType<IFieldSymbol>().Where(_ => Constants.Names.TypeUnderTestFieldNames.Contains(_.Name));

            return Array.Empty<ISymbol>().Concat(propertyTypes).Concat(fieldTypes).Concat(methodTypes).WhereNotNull().ToHashSet(SymbolEqualityComparer.Default);

            IEnumerable<IMethodSymbol> GetTestCreationMethods(List<ISymbol> symbols)
            {
                var count = symbols.Count;

                if (count > 0)
                {
                    for (var index = 0; index < count; index++)
                    {
                        if (symbols[index] is IMethodSymbol method && method.IsTypeUnderTestCreationMethod())
                        {
                            yield return method;
                        }
                    }
                }
            }
        }

        internal static IReadOnlyCollection<ITypeSymbol> GetTypeUnderTestTypes(this ITypeSymbol value)
        {
            // TODO: RKN what about base types?
            return value.GetTypeUnderTestMembers().Select(_ => _.GetReturnTypeSymbol()).WhereNotNull().ToHashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
        }

        internal static bool HasAttributeApplied(this ISymbol value, string attributeName)
        {
            var attributes = value.GetAttributes();

            if (attributes.Length is 0)
            {
                return false;
            }

            return attributes.Any(_ => _.AttributeClass.InheritsFrom(attributeName));
        }

        internal static bool HasAttribute(this ISymbol value, string attributeName)
        {
            var attributes = value.GetAttributes();

            if (attributes.Length is 0)
            {
                return false;
            }

            return attributes.Any(_ => attributeName == _.AttributeClass?.FullyQualifiedName());
        }

        internal static bool HasAttribute(this ISymbol value, ISet<string> attributeNames)
        {
            var attributes = value.GetAttributes();

            if (attributes.Length is 0)
            {
                return false;
            }

            return attributes.Any(_ => attributeNames.Contains(_.AttributeClass?.Name) || attributeNames.Contains(_.AttributeClass?.FullyQualifiedName()));
        }

        internal static bool HasDependencyObjectParameter(this IMethodSymbol value)
        {
            var parameters = value.Parameters;

            if (parameters.Length is 0)
            {
                return false;
            }

            return parameters.Any(_ => _.Type.IsDependencyObject());
        }

        internal static bool HasFlags(this IParameterSymbol value) => value.Type.HasFlags();

        internal static bool HasFlags(this ITypeSymbol value) => value.HasAttribute(Constants.Names.FlagsAttributeNames);

        internal static bool HasModifier(this IMethodSymbol value, in SyntaxKind kind) => ((BaseMethodDeclarationSyntax)value.GetSyntax()).Modifiers.Any(kind);

        internal static bool Implements<T>(this ITypeSymbol value) => Implements(value, typeof(T).FullName);

        internal static bool Implements(this ITypeSymbol value, string interfaceTypeName)
        {
            switch (value.SpecialType)
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
                case SpecialType.System_Object:
                case SpecialType.System_Enum:
                case SpecialType.System_Delegate:
                case SpecialType.System_MulticastDelegate:
                case SpecialType.System_TypedReference:
                case SpecialType.System_ArgIterator:
                case SpecialType.System_RuntimeArgumentHandle:
                case SpecialType.System_RuntimeFieldHandle:
                case SpecialType.System_RuntimeMethodHandle:
                case SpecialType.System_RuntimeTypeHandle:
                case SpecialType.System_Runtime_CompilerServices_IsVolatile:
                case SpecialType.System_AsyncCallback:
                case SpecialType.System_Runtime_CompilerServices_RuntimeFeature:
                case SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute:
                {
                    return false;
                }
            }

            switch (value.TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Dynamic:
                case TypeKind.Enum:
                case TypeKind.FunctionPointer:
                case TypeKind.Module:
                case TypeKind.Pointer:
                case TypeKind.Submission:
                case TypeKind.TypeParameter:
                {
                    return false;
                }
            }

            var fullName = value.ToString();

            if (fullName == interfaceTypeName)
            {
                return true;
            }

            var interfaces = value.AllInterfaces;
            var length = interfaces.Length;

            if (length > 0)
            {
                for (var index = 0; index < length; index++)
                {
                    var fullInterfaceName = interfaces[index].ToString();

                    if (fullInterfaceName == interfaceTypeName)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool Implements(this ITypeSymbol value, string interfaceTypeName, string interfaceTypeFullQualifiedName) => value.Implements(interfaceTypeName) || value.Implements(interfaceTypeFullQualifiedName);

        internal static bool ImplementsPotentialGeneric(this ITypeSymbol value, Type interfaceType) => ImplementsPotentialGeneric(value, interfaceType.FullName);

        internal static bool ImplementsPotentialGeneric(this ITypeSymbol value, string interfaceType)
        {
            switch (value.SpecialType)
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
                case SpecialType.System_Object:
                case SpecialType.System_Enum:
                case SpecialType.System_Delegate:
                case SpecialType.System_MulticastDelegate:
                case SpecialType.System_TypedReference:
                case SpecialType.System_ArgIterator:
                case SpecialType.System_RuntimeArgumentHandle:
                case SpecialType.System_RuntimeFieldHandle:
                case SpecialType.System_RuntimeMethodHandle:
                case SpecialType.System_RuntimeTypeHandle:
                case SpecialType.System_Runtime_CompilerServices_IsVolatile:
                case SpecialType.System_AsyncCallback:
                case SpecialType.System_Runtime_CompilerServices_RuntimeFeature:
                case SpecialType.System_Runtime_CompilerServices_PreserveBaseOverridesAttribute:
                {
                    return false;
                }
            }

            switch (value.TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Dynamic:
                case TypeKind.Enum:
                case TypeKind.FunctionPointer:
                case TypeKind.Module:
                case TypeKind.Pointer:
                case TypeKind.Submission:
                case TypeKind.TypeParameter:
                {
                    return false;
                }
            }

            var index = interfaceType.IndexOf('`');
            var interfaceTypeWithoutGeneric = index > -1
                                              ? interfaceType.AsSpan(0, index)
                                              : interfaceType.AsSpan();

            var fullName = value.ToString();

            if (fullName.StartsWith(interfaceTypeWithoutGeneric, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            var interfaces = value.AllInterfaces;
            var length = interfaces.Length;

            if (length > 0)
            {
                for (var i = 0; i < length; i++)
                {
                    var implementedInterface = interfaces[i];
                    var fullInterfaceName = implementedInterface.ToString();

                    if (fullInterfaceName.StartsWith(interfaceTypeWithoutGeneric, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool AnyBaseType(this ITypeSymbol value, Func<ITypeSymbol, bool> callback)
        {
            var symbol = value;

            while (true)
            {
                if (callback(symbol))
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

        internal static IReadOnlyCollection<ITypeSymbol> IncludingAllBaseTypes(this ITypeSymbol value)
        {
            var symbol = value;

            var baseTypes = new Queue<ITypeSymbol>(symbol.IsValueType ? 1 : 2); // probably an object, so increase by 1 to skip re-allocation
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

        internal static IReadOnlyCollection<ITypeSymbol> IncludingAllNestedTypes(this ITypeSymbol value)
        {
            var types = new Queue<ITypeSymbol>(value.IsValueType ? 1 : 2); // probably an object, so increase by 1 to skip re-allocation

            CollectAllNestedTypes(value);

            return types;

            void CollectAllNestedTypes(ITypeSymbol symbol)
            {
                types.Enqueue(symbol);

                foreach (var nestedType in symbol.GetTypeMembers())
                {
                    CollectAllNestedTypes(nestedType);
                }
            }
        }

        // ReSharper disable once AssignNullToNotNullAttribute
        internal static bool InheritsFrom<T>(this ITypeSymbol value) => InheritsFrom(value, typeof(T).FullName);

        internal static bool InheritsFrom(this ITypeSymbol value, string baseClassName)
        {
            if (value is null)
            {
                return false;
            }

            switch (value.SpecialType)
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
                {
                    return false;
                }
            }

            switch (value.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Error: // needed for attribute types
                {
                    if (value.IsRecord)
                    {
                        // ignore records here as they can only inherit from other records due to their special nature (such as copy constructors)
                        return false;
                    }

                    return value.AnyBaseType(_ => baseClassName == _.ToString());
                }

                default:
                {
                    return false;
                }
            }
        }

        internal static bool InheritsFrom(this ITypeSymbol value, string baseClassName, string baseClassFullQualifiedName)
        {
            switch (value.SpecialType)
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
                {
                    return false;
                }
            }

            switch (value.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Error: // needed for attribute types
                {
                    if (value.IsRecord)
                    {
                        // ignore records here as they can only inherit from other records due to their special nature (such as copy constructors)
                        return false;
                    }

                    return value.AnyBaseType(_ =>
                                                 {
                                                     var fullName = _.ToString();

                                                     return baseClassName == fullName || baseClassFullQualifiedName == fullName;
                                                 });
                }

                default:
                    return false;
            }
        }

        internal static bool IsRelated(this ITypeSymbol value, ITypeSymbol type)
        {
            switch (type.TypeKind)
            {
                case TypeKind.Interface:
                {
                    // it's an interface implementation, so we do not need an extra type
                    return value.AllInterfaces.Contains(type, SymbolEqualityComparer.Default);
                }

                case TypeKind.Class:
                case TypeKind.Struct:
                {
                    // it's a base type, so we do not need an extra type
                    return value.AnyBaseType(_ => _.Equals(type, SymbolEqualityComparer.Default));
                }

                default:
                {
                    return false;
                }
            }
        }

        internal static bool IsAspNetCoreController(this IMethodSymbol value)
        {
            if (value.DeclaredAccessibility is Accessibility.Public)
            {
                var returnType = value.ReturnType;

                return returnType.TypeKind is TypeKind.Interface
                    && returnType.FullyQualifiedName() is "Microsoft.AspNetCore.Mvc.IActionResult"
                    && value.ContainingType.InheritsFrom("ControllerBase", "Microsoft.AspNetCore.Mvc.ControllerBase");
            }

            return false;
        }

        internal static bool IsAspNetCoreStartUp(this IMethodSymbol value)
        {
            switch (value.Name)
            {
                case "Configure":
                case "ConfigureServices":
                    return value.ReturnsVoid && value.ContainingType?.Name is "Startup";

                default:
                    return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsAsyncTaskBased(this IMethodSymbol value) => value.IsAsync || value.ReturnType.IsTask();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsBoolean(this ITypeSymbol value)
        {
            if (value is null)
            {
                return false;
            }

            if (value.SpecialType is SpecialType.System_Boolean)
            {
                return true;
            }

            return value.IsNullable() && value is INamedTypeSymbol type && type.TypeArguments[0].SpecialType is SpecialType.System_Boolean;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsByte(this ITypeSymbol value) => value.SpecialType is SpecialType.System_Byte;

        internal static bool IsByteArray(this ITypeSymbol value) => value is IArrayTypeSymbol array && array.ElementType.IsByte();

        internal static bool IsIGrouping(this ITypeSymbol value)
        {
            if (value.TypeKind is TypeKind.Interface)
            {
                switch (value.Name)
                {
                    case "IGrouping":
                    case "System.Linq.IGrouping":
                        return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsCancellationToken(this ITypeSymbol value) => value.TypeKind is TypeKind.Struct && value.ToString() == TypeNames.CancellationToken;

        internal static bool IsCoerceValueCallback(this IMethodSymbol value)
        {
            if (value.ReturnType.IsObject())
            {
                var parameters = value.Parameters;

                return parameters.Length is 2 && parameters[0].Type.IsDependencyObject() && parameters[1].Type.IsObject();
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsCommand(this ITypeSymbol value) => value.Implements<ICommand>();

        internal static bool IsConstructor(this ISymbol value) => value is IMethodSymbol m && m.IsConstructor();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsConstructor(this IMethodSymbol value) => value.MethodKind is MethodKind.Constructor;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsPrimaryConstructor(this ISymbol value) => value is IMethodSymbol m && m.IsPrimaryConstructor();

        internal static bool IsPrimaryConstructor(this IMethodSymbol value)
        {
            if (value.IsConstructor())
            {
                switch (value.GetSyntax())
                {
#if VS2022
                    case ClassDeclarationSyntax c: return c.HasPrimaryConstructor();
                    case StructDeclarationSyntax s: return s.HasPrimaryConstructor();
#endif
                    case RecordDeclarationSyntax r: return r.HasPrimaryConstructor();
                }
            }

            return false;
        }

        internal static bool IsDependencyObject(this ITypeSymbol value) => value.InheritsFrom("DependencyObject", "System.Windows.DependencyObject");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsDependencyProperty(this ITypeSymbol value) => value.Name is Constants.DependencyProperty.TypeName || value.Name is Constants.DependencyProperty.FullyQualifiedTypeName;

        internal static bool IsDependencyPropertyChangedEventArgs(this ITypeSymbol value)
        {
            if (value.TypeKind is TypeKind.Struct && value.SpecialType is SpecialType.None)
            {
                switch (value.Name)
                {
                    case "DependencyPropertyChangedEventArgs":
                    case "System.Windows.DependencyPropertyChangedEventArgs":
                        return true;
                }
            }

            return false;
        }

        internal static bool IsDependencyObjectEventHandler(this IMethodSymbol value)
        {
            var parameters = value.Parameters;

            return parameters.Length is 2 && parameters[0].Type.IsDependencyObject() && parameters[1].Type.IsDependencyPropertyChangedEventArgs();
        }

        internal static bool IsDependencyPropertyEventHandler(this IMethodSymbol value)
        {
            var parameters = value.Parameters;

            return parameters.Length is 2 && parameters[0].Type.IsObject() && parameters[1].Type.IsDependencyPropertyChangedEventArgs();
        }

        internal static bool IsDependencyPropertyKey(this ITypeSymbol value) => value.Name is Constants.DependencyPropertyKey.TypeName || value.Name is Constants.DependencyPropertyKey.FullyQualifiedTypeName;

        internal static bool IsDisposable(this ITypeSymbol value)
        {
            var interfaces = value.AllInterfaces;

            return interfaces.Length > 0 && interfaces.Any(_ => _.SpecialType is SpecialType.System_IDisposable);
        }

        internal static bool IsEnhancedByPostSharpAdvice(this ISymbol value) => value.HasAttributeApplied("PostSharp.Aspects.Advices.Advice");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsEnum(this ITypeSymbol value) => value?.TypeKind is TypeKind.Enum;

        internal static bool IsEnumerable(this ITypeSymbol value)
        {
            var specialType = value.SpecialType;

            switch (specialType)
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
                {
                    if (value.TypeKind is TypeKind.Array)
                    {
                        return true;
                    }

                    if (IsEnumerable(specialType))
                    {
                        return true;
                    }

                    if (value.IsValueType)
                    {
                        // value types may also implement the interface
                        return value.Implements<IEnumerable>();
                    }

                    if (value is INamedTypeSymbol s && IsEnumerable(s.ConstructedFrom.SpecialType))
                    {
                        return true;
                    }

                    return value.Implements<IEnumerable>();
                }
            }
        }

        internal static bool IsPrismEvent(this ITypeSymbol value) => value.TypeKind is TypeKind.Class
                                                                  && value.SpecialType is SpecialType.None
                                                                  && value.ToString() != "Microsoft.Practices.Prism.Events.EventBase"
                                                                  && value.InheritsFrom("Microsoft.Practices.Prism.Events.EventBase");

        internal static bool IsEventArgs(this ITypeSymbol value) => value.TypeKind is TypeKind.Class
                                                                 && value.SpecialType is SpecialType.None
                                                                 && value.InheritsFrom<EventArgs>();

        internal static bool IsEventHandler(this IMethodSymbol value)
        {
            switch (value.MethodKind)
            {
                case MethodKind.Constructor:
                case MethodKind.StaticConstructor:
                case MethodKind.EventAdd:
                case MethodKind.EventRemove:
                case MethodKind.PropertyGet:
                case MethodKind.PropertySet:
                case MethodKind.BuiltinOperator:
                case MethodKind.UserDefinedOperator:
                {
                    return false;
                }

                default:
                {
                    var parameters = value.Parameters;

                    return parameters.Length is 2 && parameters[0].Type.IsObject() && parameters[1].Type.IsEventArgs();
                }
            }
        }

        internal static bool IsEventHandler(this ITypeSymbol value)
        {
            if (value.TypeKind != TypeKind.Delegate)
            {
                return false;
            }

            switch (value.Name)
            {
                case nameof(EventHandler):
                case nameof(PropertyChangingEventHandler):
                case nameof(PropertyChangedEventHandler):
                    return true;

                default:
                    return false;
            }
        }

        internal static bool IsException(this ITypeSymbol value) => value != null
                                                                 && value.TypeKind is TypeKind.Class
                                                                 && value.SpecialType is SpecialType.None
                                                                 && value.OriginalDefinition.InheritsFrom<Exception>();

        internal static bool IsException(this ArgumentSyntax value, SemanticModel semanticModel)
        {
            if (value is null)
            {
                return false;
            }

            var argumentType = value.Expression is InvocationExpressionSyntax i && i.Expression is MemberAccessExpressionSyntax m
                               ? m.GetTypeSymbol(semanticModel)
                               : value.GetTypeSymbol(semanticModel);

            return argumentType.IsException();
        }

        internal static bool IsFactory(this ITypeSymbol value)
        {
            switch (value?.TypeKind)
            {
                case TypeKind.Class:
                case TypeKind.Interface:
                {
                    var valueName = value.Name.AsSpan();

                    return valueName.EndsWith(Constants.Names.Factory, StringComparison.Ordinal) && valueName.EndsWith(nameof(TaskFactory), StringComparison.Ordinal) is false;
                }

                default:
                    return false;
            }
        }

        internal static bool IsGenerated(this ITypeSymbol value) => value?.TypeKind is TypeKind.Class && value.HasAttribute(Constants.Names.GeneratedAttributeNames);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsGeneric(this ITypeSymbol value) => value is INamedTypeSymbol type && type.TypeArguments.Length > 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsGuid(this ITypeSymbol value) => value.IsValueType && value.Name == nameof(Guid);

        internal static bool IsImport(this ISymbol value) => value.HasAttribute(Constants.Names.ImportAttributeNames);

        internal static bool IsImportingConstructor(this ISymbol value) => value.IsConstructor() && value.HasAttribute(Constants.Names.ImportingConstructorAttributeNames);

        internal static bool IsInterfaceImplementation<TSymbol>(this TSymbol value) where TSymbol : class, ISymbol
        {
            if (value.IsStatic)
            {
                return false;
            }

            if (value.CanBeReferencedByName is false)
            {
                // cannot be an interface method as those have names
                return false;
            }

            switch (value)
            {
                case IFieldSymbol _:
                case ITypeSymbol _:
                    return false;

                case IMethodSymbol method:
                    return method.IsInterfaceImplementation();

                case IPropertySymbol p when p.ExplicitInterfaceImplementations.Any():
                case IEventSymbol e when e.ExplicitInterfaceImplementations.Any():
                    return true;
            }

            var typeSymbol = value.ContainingType;

            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Enum:
                case TypeKind.Interface:
                case TypeKind.Pointer:
                case TypeKind.FunctionPointer:
                case TypeKind.TypeParameter:
                case TypeKind.Submission:
                case TypeKind.Module:
                    return false;
            }

            return value.IsInterfaceImplementation(typeSymbol);
        }

        internal static bool IsInterfaceImplementation(this IMethodSymbol value)
        {
            if (value.IsStatic)
            {
                return false;
            }

            switch (value.MethodKind)
            {
                case MethodKind.AnonymousFunction:
                case MethodKind.Constructor:
                case MethodKind.SharedConstructor:
                case MethodKind.Destructor:
                case MethodKind.BuiltinOperator:
                case MethodKind.UserDefinedOperator:
                case MethodKind.LocalFunction:
                case MethodKind.FunctionPointerSignature:
                    return false;

                case MethodKind.ExplicitInterfaceImplementation:
                    return true;
            }

            if (value.CanBeReferencedByName is false)
            {
                // cannot be an interface method as those have names
                return false;
            }

            var typeSymbol = value.ContainingType;

            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Enum:
                case TypeKind.Interface:
                case TypeKind.Pointer:
                case TypeKind.FunctionPointer:
                case TypeKind.TypeParameter:
                case TypeKind.Submission:
                case TypeKind.Module:
                    return false;
            }

            if (value.ExplicitInterfaceImplementations.Any())
            {
                return true;
            }

            return value.IsInterfaceImplementation(typeSymbol);
        }

        internal static bool IsInterfaceImplementationOf<T>(this IMethodSymbol value)
        {
            if (value.IsStatic)
            {
                return false;
            }

            switch (value.MethodKind)
            {
                case MethodKind.AnonymousFunction:
                case MethodKind.Constructor:
                case MethodKind.StaticConstructor:
                case MethodKind.Destructor:
                case MethodKind.EventAdd:
                case MethodKind.EventRemove:
                case MethodKind.PropertyGet:
                case MethodKind.PropertySet:
                case MethodKind.BuiltinOperator:
                case MethodKind.UserDefinedOperator:
                case MethodKind.LocalFunction:
                case MethodKind.FunctionPointerSignature:
                    return false;
            }

            if (value.CanBeReferencedByName is false)
            {
                // cannot be an interface method as those have names
                return false;
            }

            var typeSymbol = value.ContainingType;

            switch (typeSymbol.TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Enum:
                case TypeKind.Interface:
                case TypeKind.Pointer:
                case TypeKind.FunctionPointer:
                case TypeKind.TypeParameter:
                case TypeKind.Submission:
                case TypeKind.Module:
                    return false;
            }

            // ReSharper disable once AssignNullToNotNullAttribute
            var interfaceTypeName = typeof(T).FullName;

            if (typeSymbol.Implements(interfaceTypeName))
            {
                return value.IsInterfaceImplementation(typeSymbol, interfaceTypeName);
            }

            return false;
        }

        internal static bool IsLinqExtensionMethod(this ISymbol value)
        {
            if (value is IMethodSymbol method)
            {
                // this is an extension method !
                if (method.IsExtensionMethod)
                {
                    var ns = method.ContainingNamespace;

                    if (ns.Name == nameof(System.Linq) && ns.ContainingNamespace.Name == nameof(System))
                    {
                        return true;
                    }

                    if (ns.FullyQualifiedName().StartsWith("System.Linq", StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        internal static bool IsMultiValueConverter(this ITypeSymbol value) => value.Implements(Constants.Names.IMultiValueConverter, Constants.Names.IMultiValueConverterFullName)
                                                                           || value.InheritsFrom(Constants.Names.IMultiValueConverter, Constants.Names.IMultiValueConverterFullName);

        // ignore special situation for task factory
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsNullable(this ITypeSymbol value) => value.IsValueType && value.OriginalDefinition.SpecialType is SpecialType.System_Nullable_T;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsObject(this ITypeSymbol value) => value.SpecialType is SpecialType.System_Object;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsPartial(this ITypeSymbol value) => value.Locations.Length > 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsPartial(this IMethodSymbol value) => value.HasModifier(SyntaxKind.PartialKeyword);

        internal static bool IsPubliclyVisible(this ISymbol value)
        {
            switch (value.DeclaredAccessibility)
            {
                case Accessibility.NotApplicable:
                case Accessibility.Private:
                    return false;

                default:
                    return true;
            }
        }

        internal static bool IsRoutedEvent(this ITypeSymbol value)
        {
            if (value.TypeKind is TypeKind.Class && value.IsSealed)
            {
                switch (value.Name)
                {
                    case "RoutedEvent":
                    case "System.Windows.RoutedEvent":
                        return true;
                }
            }

            return false;
        }

        internal static bool IsSerializationConstructor(this IMethodSymbol value) => value.IsConstructor() && value.Parameters.Length is 2 && value.Parameters[0].IsSerializationInfoParameter() && value.Parameters[1].IsStreamingContextParameter();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsSerializationInfoParameter(this IParameterSymbol value) => value.Type.Name == nameof(SerializationInfo);

        internal static bool IsSpecialAccessor(this IMethodSymbol value)
        {
            switch (value.MethodKind)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsStreamingContextParameter(this IParameterSymbol value) => value.Type.Name == nameof(StreamingContext);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsString(this ITypeSymbol value) => value?.SpecialType is SpecialType.System_String;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsTask(this ITypeSymbol value) => value?.Name == nameof(Task);

        internal static bool IsTestClass(this ITypeSymbol value)
        {
            if (value?.TypeKind is TypeKind.Class && value.IsRecord is false)
            {
                var name = value.Name;

                if (name.EndsWith("Tests", StringComparison.Ordinal)
                 || name.EndsWith("Test", StringComparison.Ordinal)
                 || name.EndsWith("test", StringComparison.Ordinal))
                {
                    return true;
                }

                return value.HasAttribute(Constants.Names.TestClassAttributeNames);
            }

            return false;
        }

        internal static bool IsTestMethod(this IMethodSymbol value) => value.IsTestSpecificMethod(Constants.Names.TestMethodAttributeNames);

        internal static bool IsTestOneTimeSetUpMethod(this IMethodSymbol value) => value.IsTestSpecificMethod(Constants.Names.TestOneTimeSetupAttributeNames);

        internal static bool IsTestOneTimeTearDownMethod(this IMethodSymbol value) => value.IsTestSpecificMethod(Constants.Names.TestOneTimeTearDownAttributeNames);

        internal static bool IsTestSetUpMethod(this IMethodSymbol value) => value.IsTestSpecificMethod(Constants.Names.TestSetupAttributeNames);

        internal static bool IsTestTearDownMethod(this IMethodSymbol value) => value.IsTestSpecificMethod(Constants.Names.TestTearDownAttributeNames);

        internal static bool IsTypeUnderTestCreationMethod(this IMethodSymbol value) => value.ReturnsVoid is false && Constants.Names.TypeUnderTestMethodNames.Contains(value.Name);

        internal static bool IsValidateValueCallback(this IMethodSymbol value)
        {
            if (value.ReturnType.IsBoolean())
            {
                var parameters = value.Parameters;

                return parameters.Length is 1 && parameters[0].Type.IsObject();
            }

            return false;
        }

        internal static bool IsValueConverter(this ITypeSymbol value) => value.Implements(Constants.Names.IValueConverter, Constants.Names.IValueConverterFullName)
                                                                      || value.InheritsFrom(Constants.Names.IValueConverter, Constants.Names.IValueConverterFullName);

        /// <summary>
        /// Determines whether a <see cref="IFieldSymbol"/> of the containing type has the same name as the given <see cref="IParameterSymbol"/>.
        /// </summary>
        /// <param name="value">
        /// The symbol to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the containing <see cref="INamedTypeSymbol"/> contains a <see cref="IFieldSymbol"/> that matches the name of <paramref name="value"/>; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool MatchesField(this IParameterSymbol value)
        {
            string[] fieldNames = null;

            foreach (var field in value.ContainingType.GetMembersIncludingInherited<IFieldSymbol>())
            {
                if (fieldNames is null)
                {
                    // performance optimization as it is likely that there is more than a single field(s)
                    var parameterName = value.Name;

                    var prefixes = Constants.Markers.FieldPrefixes;
                    var prefixesLength = prefixes.Length;

                    fieldNames = new string[prefixesLength];

                    for (var index = 0; index < prefixesLength; index++)
                    {
                        fieldNames[index] = prefixes[index] + parameterName;
                    }
                }

                if (field.Name.EqualsAny(fieldNames))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether a <see cref="IPropertySymbol"/> of the containing type has the same name as the given <see cref="IParameterSymbol"/>.
        /// </summary>
        /// <param name="value">
        /// The symbol to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the containing <see cref="INamedTypeSymbol"/> contains a <see cref="IPropertySymbol"/> that matches the name of <paramref name="value"/>; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool MatchesProperty(this IParameterSymbol value)
        {
            var valueName = value.Name;

            return value.ContainingType.GetMembersIncludingInherited<IPropertySymbol>().Any(_ => string.Equals(valueName, _.Name, StringComparison.OrdinalIgnoreCase));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string MinimalTypeName(this ITypeSymbol value) => value.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        internal static bool NameMatchesTypeName(this ISymbol value, ITypeSymbol type, in ushort minimumUpperCaseLetters = 0)
        {
            var symbolName = value.Name;

            if (symbolName.Equals(type.Name, StringComparison.OrdinalIgnoreCase))
            {
                // ignore all those that have a lower case only
                return symbolName.HasUpperCaseLettersAbove(minimumUpperCaseLetters);
            }

            var typeName = type.GetNameWithoutInterfacePrefix();

            if (symbolName.Equals(typeName, StringComparison.OrdinalIgnoreCase))
            {
                // there must be at least a minimum of upper case letters (except the first character)
                if (symbolName.HasUpperCaseLettersAbove(minimumUpperCaseLetters))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool TryGetGenericArgumentCount(this ITypeSymbol value, out int result)
        {
            result = 0;

            if (value is INamedTypeSymbol namedType)
            {
                result = namedType.TypeArguments.Length;
            }

            return result > 0;
        }

        internal static bool TryGetGenericArgumentType(this ITypeSymbol value, out ITypeSymbol result, in int index = 0)
        {
            result = null;

            if (value is INamedTypeSymbol namedType)
            {
                var typeArguments = namedType.TypeArguments;

                if (typeArguments.Length >= index + 1)
                {
                    result = typeArguments[index];
                }
            }

            return result != null;
        }

        private static ReadOnlySpan<char> GetNameWithoutInterfacePrefix(this ITypeSymbol value)
        {
            var typeName = value.Name.AsSpan();

            if (value.TypeKind is TypeKind.Interface && typeName.Length > 2 && typeName[0] is 'I' && typeName[1].IsUpperCase())
            {
                return typeName.Slice(1);
            }

            return typeName;
        }

        private static bool IsInterfaceImplementation<TSymbol>(this TSymbol value, ITypeSymbol typeSymbol) where TSymbol : ISymbol
        {
            var interfaces = typeSymbol.AllInterfaces;

            if (interfaces.Length > 0)
            {
                return value.IsInterfaceImplementation(typeSymbol, interfaces);
            }

            return false;
        }

        private static bool IsInterfaceImplementation<TSymbol>(this TSymbol value, ITypeSymbol typeSymbol, string interfaceTypeName) where TSymbol : ISymbol
        {
            var interfaces = typeSymbol.AllInterfaces;

            if (interfaces.Length > 0)
            {
                return value.IsInterfaceImplementation(typeSymbol, interfaces.Where(_ => _.Name == interfaceTypeName));
            }

            return false;
        }

        private static bool IsInterfaceImplementation<TSymbol>(this TSymbol value, ITypeSymbol typeSymbol, in ImmutableArray<INamedTypeSymbol> implementedInterfaces) where TSymbol : ISymbol
        {
            var name = value.Name;

            // Perf: do not use enumerable
            for (int index = 0, length = implementedInterfaces.Length; index < length; index++)
            {
                var members = implementedInterfaces[index].GetMembers(name);
                var membersLength = members.Length;

                if (membersLength > 0)
                {
                    // Perf: do not use Linq
                    for (var memberIndex = 0; memberIndex < membersLength; memberIndex++)
                    {
                        if (members[memberIndex] is TSymbol symbol && value.Equals(typeSymbol.FindImplementationForInterfaceMember(symbol), SymbolEqualityComparer.Default))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsInterfaceImplementation<TSymbol>(this TSymbol value, ITypeSymbol typeSymbol, IEnumerable<INamedTypeSymbol> implementedInterfaces) where TSymbol : ISymbol
        {
            var name = value.Name;

            // ReSharper disable once LoopCanBePartlyConvertedToQuery
            foreach (var implementedInterface in implementedInterfaces)
            {
                var members = implementedInterface.GetMembers(name);
                var membersLength = members.Length;

                if (membersLength > 0)
                {
                    // Perf: do not use Linq
                    for (var memberIndex = 0; memberIndex < membersLength; memberIndex++)
                    {
                        if (members[memberIndex] is TSymbol symbol && value.Equals(typeSymbol.FindImplementationForInterfaceMember(symbol), SymbolEqualityComparer.Default))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static bool IsTestSpecificMethod(this IMethodSymbol value, ISet<string> attributeNames) => value?.MethodKind is MethodKind.Ordinary && value.IsPubliclyVisible() && value.HasAttribute(attributeNames);

        private static List<T> GetNamedSymbols<T>(IReadOnlyList<T> symbols) where T : ISymbol
        {
            var count = symbols.Count;
            var results = new List<T>(count);

            for (var i = 0; i < count; i++)
            {
                var symbol = symbols[i];

                if (symbol.CanBeReferencedByName)
                {
                    results.Add(symbol);
                }
            }

            return results;
        }

        private static bool IsLocalFunctionContainerCore(SyntaxNode node)
        {
            switch (node.RawKind)
            {
                case (int)SyntaxKind.Block: // 8792
                case (int)SyntaxKind.LocalFunctionStatement: // 8830
                case (int)SyntaxKind.MethodDeclaration: // 8875,
                case (int)SyntaxKind.ConstructorDeclaration: // 8878
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsEnumerable(in SpecialType type)
        {
            switch (type)
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
    }
}