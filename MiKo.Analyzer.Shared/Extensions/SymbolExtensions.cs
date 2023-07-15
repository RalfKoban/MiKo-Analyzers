using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
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

        private static readonly string[] GeneratedCSharpFileExtensions =
                                                                         {
                                                                             ".g.cs",
                                                                             ".generated.cs",
                                                                             ".Designer.cs",
                                                                         };

        private static readonly SyntaxKind[] LocalFunctionContainerSyntaxKinds =
                                                                                 {
                                                                                     SyntaxKind.MethodDeclaration,
                                                                                     SyntaxKind.Block,
                                                                                     SyntaxKind.ConstructorDeclaration,
                                                                                     SyntaxKind.LocalFunctionStatement,
                                                                                 };

        internal static IEnumerable<IMethodSymbol> GetExtensionMethods(this ITypeSymbol value) => value.GetMethods().Where(_ => _.IsExtensionMethod);

        internal static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol value) => value.GetMembers<IMethodSymbol>();

        internal static IEnumerable<IMethodSymbol> GetMethods(this ITypeSymbol value, MethodKind kind)
        {
            // note that methods with MethodKind.Constructor cannot be referenced by name
            var methods = kind == MethodKind.Ordinary
                          ? value.GetNamedMethods()
                          : value.GetMethods();

            return methods.Where(_ => _.MethodKind == kind);
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
        internal static IEnumerable<IMethodSymbol> GetNamedMethods(this ITypeSymbol value) => value.GetMethods().Where(_ => _.CanBeReferencedByName);

        internal static IEnumerable<IPropertySymbol> GetProperties(this ITypeSymbol value) => value.GetMembers<IPropertySymbol>().Where(_ => _.CanBeReferencedByName);

        internal static IEnumerable<IFieldSymbol> GetFields(this ITypeSymbol value) => value.GetMembers<IFieldSymbol>().Where(_ => _.CanBeReferencedByName);

        internal static IEnumerable<LocalFunctionStatementSyntax> GetLocalFunctions(this IMethodSymbol value)
        {
            return value.GetSyntaxNodes()
                        .SelectMany(_ => _.DescendantNodes(__ => __.IsAnyKind(LocalFunctionContainerSyntaxKinds)))
                        .OfType<LocalFunctionStatementSyntax>();
        }

        internal static bool ContainsExtensionMethods(this INamedTypeSymbol value) => value.TypeKind == TypeKind.Class && value.IsStatic && value.MightContainExtensionMethods && value.GetExtensionMethods().Any();

        internal static INamedTypeSymbol FindContainingType(this SyntaxNodeAnalysisContext value) => FindContainingType(value.ContainingSymbol);

        internal static INamedTypeSymbol FindContainingType(this ISymbol value) => value is INamedTypeSymbol type
                                                                                   ? type
                                                                                   : value?.ContainingType;

        internal static string FullyQualifiedName(this ISymbol value, bool useAlias = true)
        {
            switch (value)
            {
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
                return Enumerable.Empty<MemberAccessExpressionSyntax>();
            }

            return field.DescendantNodes<MemberAccessExpressionSyntax>(_ => _.ToCleanedUpString() == invocation);
        }

        internal static IEnumerable<string> GetAttributeNames(this ISymbol value) => value.GetAttributes().Select(_ => _.AttributeClass.Name);

        internal static IEnumerable<ObjectCreationExpressionSyntax> GetCreatedObjectSyntaxReturnedByMethod(this IMethodSymbol value)
        {
            var method = (MethodDeclarationSyntax)value.GetSyntax();

            foreach (var createdObject in method.DescendantNodes<ObjectCreationExpressionSyntax>())
            {
                switch (createdObject.Parent)
                {
                    case ArrowExpressionClauseSyntax arrow when arrow.Parent == method:
                    case ReturnStatementSyntax rs when rs.Parent == method:
                    {
                        yield return createdObject;

                        break;
                    }
                }
            }
        }

        // TODO RKN: find better name
        internal static IEnumerable<ObjectCreationExpressionSyntax> GetCreatedObjectSyntaxReturnedByMethods(this ITypeSymbol value)
        {
            return value.GetNamedMethods().Where(IsTypeUnderTestCreationMethod).SelectMany(GetCreatedObjectSyntaxReturnedByMethod);
        }

        internal static IMethodSymbol GetEnclosingMethod(this ISymbol value)
        {
            var symbol = value;

            while (true)
            {
                switch (symbol)
                {
                    case null: return null;
                    case ITypeSymbol _: return null;
                    case IMethodSymbol method: return method;
                    case IPropertySymbol property:
                    {
                        if (property.IsIndexer)
                        {
                            return property.GetMethod ?? property.SetMethod;
                        }

                        return property.SetMethod;
                    }

                    default:
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
                default: return Enumerable.Range(1, count).Select(_ => "T" + _).ConcatenatedWith(",");
            }
        }

        internal static SeparatedSyntaxList<ArgumentSyntax> GetInvocationArgumentsFrom(this IFieldSymbol value, string invocation) => value.GetAssignmentsVia(invocation)
                                                                                                                                             .Select(_ => _.GetEnclosing<InvocationExpressionSyntax>())
                                                                                                                                             .Select(_ => _.ArgumentList)
                                                                                                                                             .Select(_ => _.Arguments)
                                                                                                                                             .FirstOrDefault(_ => _.Count > 0);

        internal static IEnumerable<TSymbol> GetMembers<TSymbol>(this ITypeSymbol value) where TSymbol : ISymbol => value.GetMembers().Where(_ => _.IsImplicitlyDeclared is false).OfType<TSymbol>();

        internal static IEnumerable<TSymbol> GetMembersIncludingInherited<TSymbol>(this ITypeSymbol value) where TSymbol : ISymbol => value.IncludingAllBaseTypes().SelectMany(_ => _.GetMembers<TSymbol>()).Where(_ => _.CanBeReferencedByName);

        internal static string GetMethodSignature(this IMethodSymbol value)
        {
            var builder = new StringBuilder();

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

            var signature = builder.ToString();

            return signature;

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

                        sb.Append(returnType).Append(" ").Append(method.Name);

                        if (method.IsGenericMethod)
                        {
                            sb.Append("<");

                            var typeParameters = method.TypeParameters;
                            var count = typeParameters.Length - 1;

                            for (var i = 0; i <= count; i++)
                            {
                                sb.Append(typeParameters[i].MinimalTypeName());

                                if (i < count)
                                {
                                    sb.Append(",");
                                }
                            }

                            sb.Append(">");
                        }

                        break;
                    }
                }
            }

            void AppendParameters(ImmutableArray<IParameterSymbol> parameters, StringBuilder sb)
            {
                sb.Append("(");

                var count = parameters.Length - 1;

                for (var i = 0; i <= count; i++)
                {
                    AppendParameterSignature(parameters[i], sb);

                    if (i < count)
                    {
                        sb.Append(",");
                    }
                }

                sb.Append(")");
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
                    default: return string.Empty;
                }
            }
        }

        internal static ITypeSymbol GetReturnType(this IPropertySymbol value) => value.GetMethod?.ReturnType ?? value.SetMethod?.Parameters[0].Type;

        internal static int GetStartingLine(this IMethodSymbol value) => value.Locations.First(_ => _.IsInSource).GetStartingLine();

        internal static IEnumerable<SyntaxNode> GetSyntaxNodes(this ISymbol value)
        {
            foreach (var node in value.DeclaringSyntaxReferences.Select(_ => _.GetSyntax()))
            {
                var location = node.GetLocation();

                var sourceTree = location.SourceTree;

                // "location.IsInSource" also checks SourceTree for null but ReSharper is not aware of it
                if (sourceTree is null)
                {
                    continue;
                }

                var filePath = sourceTree.FilePath;

                // ignore non C# code (might be part of partial classes, e.g. for XAML)
                if (filePath.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                {
                    // ignore generated code (might be part of partial classes)
                    if (filePath.EndsWithAny(GeneratedCSharpFileExtensions, StringComparison.OrdinalIgnoreCase) is false)
                    {
                        yield return node;
                    }
                }
            }
        }

        internal static SyntaxNode GetSyntax(this ISymbol value)
        {
            switch (value)
            {
                case IFieldSymbol field:
                {
                    var fieldNode = GetSyntax<FieldDeclarationSyntax>(field);

                    if (fieldNode != null)
                    {
                        return fieldNode;
                    }

                    // maybe it is an enum member
                    return GetSyntax<EnumMemberDeclarationSyntax>(field);
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
                    return value.GetSyntaxNodes().FirstOrDefault();
            }
        }

        internal static ParameterSyntax GetSyntax(this IParameterSymbol value) => value.GetSyntaxNodes().OfType<ParameterSyntax>().FirstOrDefault();

        internal static T GetSyntax<T>(this ISymbol value) where T : SyntaxNode => value.GetSyntaxNodes()
                                                                                        .Select(_ => _.GetEnclosing<T>())
                                                                                        .FirstOrDefault();

        internal static DocumentationCommentTriviaSyntax GetDocumentationCommentTriviaSyntax(this ISymbol value) => value.GetSyntax()?.GetDocumentationCommentTriviaSyntax();

        internal static IReadOnlyList<ITypeSymbol> GetTypeUnderTestTypes(this ITypeSymbol value)
        {
            // TODO: RKN what about base types?
            var members = value.GetMembersIncludingInherited<ISymbol>().ToList();
            var methodTypes = members.OfType<IMethodSymbol>().Where(IsTypeUnderTestCreationMethod).Select(_ => _.ReturnType);
            var propertyTypes = members.OfType<IPropertySymbol>().Where(_ => Constants.Names.TypeUnderTestPropertyNames.Contains(_.Name)).Select(_ => _.GetReturnType());
            var fieldTypes = members.OfType<IFieldSymbol>().Where(_ => Constants.Names.TypeUnderTestFieldNames.Contains(_.Name)).Select(_ => _.Type);

            return propertyTypes.Concat(fieldTypes).Concat(methodTypes).Where(_ => _ != null).Distinct(SymbolEqualityComparer.Default).Cast<ITypeSymbol>().ToList();
        }

        internal static bool HasAttributeApplied(this ISymbol value, string attributeName) => value.GetAttributes().Any(_ => _.AttributeClass.InheritsFrom(attributeName));

        internal static bool HasDependencyObjectParameter(this IMethodSymbol value) => value.Parameters.Any(_ => _.Type.IsDependencyObject());

        internal static bool Implements<T>(this ITypeSymbol value) => Implements(value, string.Intern(typeof(T).FullName));

        internal static bool Implements(this ITypeSymbol value, string interfaceType)
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
                case TypeKind.Delegate:
                case TypeKind.Dynamic:
                case TypeKind.Enum:
                case TypeKind.Module:
                case TypeKind.Pointer:
                case TypeKind.TypeParameter:
                case TypeKind.Submission:
                case TypeKind.FunctionPointer:
                {
                    return false;
                }
            }

            var fullName = string.Intern(value.ToString());

            if (fullName == interfaceType)
            {
                return true;
            }

            foreach (var implementedInterface in value.AllInterfaces)
            {
                var fullInterfaceName = string.Intern(implementedInterface.ToString());

                if (fullInterfaceName == interfaceType)
                {
                    return true;
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
                {
                    return false;
                }
            }

            switch (value.TypeKind)
            {
                case TypeKind.Delegate:
                case TypeKind.Dynamic:
                case TypeKind.Enum:
                case TypeKind.Module:
                case TypeKind.Pointer:
                case TypeKind.TypeParameter:
                case TypeKind.Submission:
                case TypeKind.FunctionPointer:
                {
                    return false;
                }
            }

            var index = interfaceType.IndexOf('`');
            var interfaceTypeWithoutGeneric = index > -1
                                              ? interfaceType.Substring(0, index)
                                              : interfaceType;

            var fullName = string.Intern(value.ToString());

            if (fullName.StartsWith(interfaceTypeWithoutGeneric, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            foreach (var implementedInterface in value.AllInterfaces)
            {
                var fullInterfaceName = string.Intern(implementedInterface.ToString());

                if (fullInterfaceName.StartsWith(interfaceTypeWithoutGeneric, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool AnyBaseType(this ITypeSymbol value, Predicate<ITypeSymbol> callback)
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

        internal static IEnumerable<ITypeSymbol> IncludingAllBaseTypes(this ITypeSymbol value)
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

        internal static IEnumerable<ITypeSymbol> IncludingAllNestedTypes(this ITypeSymbol value)
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
        internal static bool InheritsFrom<T>(this ITypeSymbol value) => InheritsFrom(value, string.Intern(typeof(T).FullName));

        internal static bool InheritsFrom(this ITypeSymbol value, string baseClass)
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
                    return value.AnyBaseType(_ => baseClass == string.Intern(_.ToString()));
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
                    return value.AnyBaseType(_ =>
                                                 {
                                                     var fullName = string.Intern(_.ToString());

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
                    // its an interface implementation, so we do not need an extra type
                    return value.AllInterfaces.Contains(type, SymbolEqualityComparer.Default);
                }

                case TypeKind.Class:
                case TypeKind.Struct:
                {
                    // its a base type, so we do not need an extra type
                    return value.AnyBaseType(_ => _.Equals(type, SymbolEqualityComparer.Default));
                }

                default:
                {
                    return false;
                }
            }
        }

        internal static bool IsAsyncTaskBased(this IMethodSymbol value) => value.IsAsync || value.ReturnType.IsTask();

        internal static bool IsBoolean(this ITypeSymbol value) => value.SpecialType == SpecialType.System_Boolean;

        internal static bool IsByte(this ITypeSymbol value) => value.SpecialType == SpecialType.System_Byte;

        internal static bool IsByteArray(this ITypeSymbol value) => value is IArrayTypeSymbol array && array.ElementType.IsByte();

        internal static bool IsIGrouping(this ITypeSymbol value)
        {
            if (value.TypeKind == TypeKind.Interface)
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

        internal static bool IsCancellationToken(this ITypeSymbol value) => value.TypeKind == TypeKind.Struct && string.Intern(value.ToString()) == TypeNames.CancellationToken;

        internal static bool IsCoerceValueCallback(this IMethodSymbol value)
        {
            if (value.ReturnType.IsObject())
            {
                var parameters = value.Parameters;

                return parameters.Length == 2 && parameters[0].Type.IsDependencyObject() && parameters[1].Type.IsObject();
            }

            return false;
        }

        internal static bool IsCommand(this ITypeSymbol value) => value.Implements<ICommand>();

        internal static bool IsConstructor(this ISymbol value) => value is IMethodSymbol m && m.IsConstructor();

        internal static bool IsConstructor(this IMethodSymbol value) => value.MethodKind == MethodKind.Constructor;

        internal static bool IsPrimaryConstructor(this IMethodSymbol value) => value.IsConstructor() && value.GetSyntax() is RecordDeclarationSyntax record && record.HasPrimaryConstructor();

        internal static bool IsDependencyObject(this ITypeSymbol value) => value.InheritsFrom("DependencyObject", "System.Windows.DependencyObject");

        internal static bool IsDependencyProperty(this ITypeSymbol value) => value.Name == Constants.DependencyProperty.TypeName || value.Name == Constants.DependencyProperty.FullyQualifiedTypeName;

        internal static bool IsDependencyPropertyChangedEventArgs(this ITypeSymbol value)
        {
            if (value.TypeKind == TypeKind.Struct && value.SpecialType == SpecialType.None)
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

            return parameters.Length == 2 && parameters[0].Type.IsDependencyObject() && parameters[1].Type.IsDependencyPropertyChangedEventArgs();
        }

        internal static bool IsDependencyPropertyEventHandler(this IMethodSymbol value)
        {
            var parameters = value.Parameters;

            return parameters.Length == 2 && parameters[0].Type.IsObject() && parameters[1].Type.IsDependencyPropertyChangedEventArgs();
        }

        internal static bool IsDependencyPropertyKey(this ITypeSymbol value) => value.Name == Constants.DependencyPropertyKey.TypeName || value.Name == Constants.DependencyPropertyKey.FullyQualifiedTypeName;

        internal static bool IsDisposable(this ITypeSymbol value) => value.AllInterfaces.Any(_ => _.SpecialType == SpecialType.System_IDisposable);

        internal static bool IsEnhancedByPostSharpAdvice(this ISymbol value) => value.HasAttributeApplied("PostSharp.Aspects.Advices.Advice");

        internal static bool IsEnum(this ITypeSymbol value) => value?.TypeKind == TypeKind.Enum;

        internal static bool IsEnumerable(this ITypeSymbol value)
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
                    return false;

                default:
                    if (IsEnumerable(value.SpecialType))
                    {
                        return true;
                    }

                    if (value.TypeKind == TypeKind.Array)
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

        internal static bool IsEnumerable(this SpecialType value)
        {
            switch (value)
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

        internal static bool IsEventArgs(this ITypeSymbol value) => value.TypeKind == TypeKind.Class
                                                                 && value.SpecialType == SpecialType.None
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

                    return parameters.Length == 2 && parameters[0].Type.IsObject() && parameters[1].Type.IsEventArgs();
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
                                                                     && value.TypeKind == TypeKind.Class
                                                                     && value.SpecialType == SpecialType.None
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

        internal static bool IsFactory(this ITypeSymbol value) => value.Name.EndsWith("Factory", StringComparison.Ordinal) && value.Name.EndsWith("TaskFactory", StringComparison.Ordinal) is false;

        internal static bool IsGenerated(this ITypeSymbol value) => value?.TypeKind == TypeKind.Class && value.GetAttributeNames().Any(Constants.Names.GeneratedAttributeNames.Contains);

        internal static bool IsGeneric(this ITypeSymbol value) => value is INamedTypeSymbol type && type.TypeArguments.Length > 0;

        internal static bool IsGuid(this ITypeSymbol value) => value.IsValueType && value.Name == nameof(Guid);

        internal static bool IsImport(this ISymbol value) => value.GetAttributeNames().Any(Constants.Names.ImportAttributeNames.Contains);

        internal static bool IsImportingConstructor(this ISymbol value) => value.IsConstructor() && value.GetAttributeNames().Any(Constants.Names.ImportingConstructorAttributeNames.Contains);

        internal static bool IsInterfaceImplementation<TSymbol>(this TSymbol value) where TSymbol : class, ISymbol
        {
            if (value.IsStatic)
            {
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

            if (value.CanBeReferencedByName is false)
            {
                // cannot be an interface method as those have names
                return false;
            }

            var symbols = typeSymbol.AllInterfaces.SelectMany(_ => _.GetMembers<TSymbol>()).Where(_ => _.CanBeReferencedByName);

            return symbols.Any(_ => value.Equals(typeSymbol.FindImplementationForInterfaceMember(_)));
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

            if (value.CanBeReferencedByName is false)
            {
                // cannot be an interface method as those have names
                return false;
            }

            var methodName = value.Name;

            var symbols = typeSymbol.AllInterfaces.SelectMany(_ => _.GetMembers(methodName).OfType<IMethodSymbol>());
            var result = symbols.Any(_ => ReferenceEquals(value, typeSymbol.FindImplementationForInterfaceMember(_)));

            return result;
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
            var interfaceTypeName = string.Intern(typeof(T).FullName);

            if (typeSymbol.Implements(interfaceTypeName))
            {
                var methodName = value.Name;

                var methodSymbols = typeSymbol.AllInterfaces
                                              .Where(_ => _.Name == interfaceTypeName)
                                              .SelectMany(_ => _.GetMembers(methodName).OfType<IMethodSymbol>());

                return methodSymbols.Any(_ => ReferenceEquals(value, typeSymbol.FindImplementationForInterfaceMember(_)));
            }

            return false;
        }

        internal static bool IsLinqExtensionMethod(this ISymbol value)
        {
            if (value is IMethodSymbol method)
            {
                // this is an extension method !
                if (method.IsExtensionMethod && method.ContainingNamespace.FullyQualifiedName().StartsWith("System.Linq", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        internal static bool IsMultiValueConverter(this ITypeSymbol value) => value.Implements(Constants.Names.IMultiValueConverter, Constants.Names.IMultiValueConverterFullName)
                                                                           || value.InheritsFrom(Constants.Names.IMultiValueConverter, Constants.Names.IMultiValueConverterFullName);

        // ignore special situation for task factory
        internal static bool IsNullable(this ITypeSymbol value) => value.IsValueType && value.Name == nameof(Nullable);

        internal static bool IsObject(this ITypeSymbol value) => value.SpecialType == SpecialType.System_Object;

        internal static bool IsPartial(this ITypeSymbol value) => value.Locations.Length > 1;

        internal static bool IsPartial(this IMethodSymbol value) => ((BaseMethodDeclarationSyntax)value.GetSyntax()).Modifiers.Any(_ => _.IsKind(SyntaxKind.PartialKeyword));

        internal static bool IsPubliclyVisible(this ISymbol symbol)
        {
            switch (symbol.DeclaredAccessibility)
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
            if (value.TypeKind == TypeKind.Class && value.IsSealed)
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

        internal static bool IsSerializationConstructor(this IMethodSymbol value) => value.IsConstructor() && value.Parameters.Length == 2 && value.Parameters[0].IsSerializationInfoParameter() && value.Parameters[1].IsStreamingContextParameter();

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

        internal static bool IsStreamingContextParameter(this IParameterSymbol value) => value.Type.Name == nameof(StreamingContext);

        internal static bool IsString(this ITypeSymbol value) => value?.SpecialType == SpecialType.System_String;

        internal static bool IsTask(this ITypeSymbol value) => value?.Name == nameof(Task);

        internal static bool IsTestClass(this ITypeSymbol value) => value?.TypeKind == TypeKind.Class && value.GetAttributeNames().Any(Constants.Names.TestClassAttributeNames.Contains);

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

                return parameters.Length == 1 && parameters[0].Type.IsObject();
            }

            return false;
        }

        internal static bool IsValueConverter(this ITypeSymbol value) => value.Implements(Constants.Names.IValueConverter, Constants.Names.IValueConverterFullName)
                                                                      || value.InheritsFrom(Constants.Names.IValueConverter, Constants.Names.IValueConverterFullName);

        /// <summary>
        /// Determines whether a <see cref="IFieldSymbol"/> of the containing type has the same name as the given <see cref="IParameterSymbol"/>.
        /// </summary>
        /// <param name="value">The symbol to inspect.</param>
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

                    fieldNames = Constants.Markers.FieldPrefixes.Select(_ => _ + parameterName).ToArray();
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
        /// <param name="value">The symbol to inspect.</param>
        /// <returns>
        /// <see langword="true"/> if the containing <see cref="INamedTypeSymbol"/> contains a <see cref="IPropertySymbol"/> that matches the name of <paramref name="value"/>; otherwise, <see langword="false"/>.
        /// </returns>
        internal static bool MatchesProperty(this IParameterSymbol value)
        {
            var valueName = value.Name;

            return value.ContainingType.GetMembersIncludingInherited<IPropertySymbol>().Any(_ => string.Equals(valueName, _.Name, StringComparison.OrdinalIgnoreCase));
        }

        internal static string MinimalTypeName(this ITypeSymbol value) => value.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        internal static bool NameMatchesTypeName(this ISymbol value, ITypeSymbol type, ushort minimumUpperCaseLetters = 0)
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
                if (typeName.HasUpperCaseLettersAbove(minimumUpperCaseLetters))
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

        internal static bool TryGetGenericArgumentType(this ITypeSymbol value, out ITypeSymbol result, int index = 0)
        {
            result = null;

            if (value is INamedTypeSymbol namedType && namedType.TypeArguments.Length >= index + 1)
            {
                result = namedType.TypeArguments[index];
            }

            return result != null;
        }

        private static ReadOnlySpan<char> GetNameWithoutInterfacePrefix(this ITypeSymbol value)
        {
            var typeName = value.Name.AsSpan();

            if (value.TypeKind == TypeKind.Interface && typeName.Length > 1 && typeName.StartsWith('I'))
            {
                return typeName.Slice(1);
            }

            return typeName;
        }

        private static bool IsTestSpecificMethod(this IMethodSymbol value, IEnumerable<string> attributeNames) => value.MethodKind == MethodKind.Ordinary && value.IsPubliclyVisible() && value.GetAttributeNames().Any(attributeNames.Contains);
    }
}