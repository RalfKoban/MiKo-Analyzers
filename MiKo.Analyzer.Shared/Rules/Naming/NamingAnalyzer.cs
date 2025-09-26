using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingAnalyzer : Analyzer
    {
        private static readonly string[] Splitters = { "Of", "With", "To", "In", "From" };

        protected NamingAnalyzer(string diagnosticId, in SymbolKind kind = SymbolKind.Method) : base(nameof(Naming), diagnosticId, kind)
        {
        }

        protected static Pair[] CreateBetterNameProposal(string betterName) => new[] { new Pair(Constants.AnalyzerCodeFixSharedData.BetterName, betterName) };

        protected static string FindBetterNameForEntityMarker(string symbolName)
        {
            var expected = HandleSpecialEntityMarkerSituations(symbolName);

            if (expected.HasCollectionMarker())
            {
                var plural = FindBetterNameForCollectionSuffix(expected);

                // symbol may have both Entity and Collection marker, such as 'ModelCollection', so 'plural' may be null
                expected = plural ?? (symbolName[0].IsUpperCase() ? Constants.Entities : Constants.entities);
            }

            return expected;
        }

        protected static string FindBetterNameForCollectionSuffix(string name)
        {
            var pluralName = FindPluralName(name.AsSpan(), out _);

            return name.Equals(pluralName, StringComparison.Ordinal) ? null : pluralName;
        }

        protected static string FindBetterNameForStructuralDesignPattern(string name, string prefix = "")
        {
            var startIndex = prefix.Length;

            foreach (var pair in Constants.Names.StructuralDesignPatternNames)
            {
                if (name.EndsWith(pair.Key, StringComparison.OrdinalIgnoreCase))
                {
                    var count = name.Length - pair.Key.Length;

                    if (count is 0)
                    {
                        return pair.Value;
                    }

                    var builder = StringBuilderCache.Acquire();

                    if (startIndex > 0)
                    {
                        builder.Append(prefix);
                    }

                    builder.Append(pair.Value);
                    builder.Append(name, startIndex, count - startIndex);
                    builder.ToUpperCaseAt(pair.Value.Length + startIndex);

                    return builder.ToStringAndRelease();
                }
            }

            return name;
        }

#pragma warning disable CA1021
        protected static string FindPluralName(in ReadOnlySpan<char> originalName, out string singularName)
        {
            if (originalName.EndsWith('s'))
            {
                singularName = originalName.ToString();

                var pluralName = Pluralizer.GetPluralName(singularName, StringComparison.Ordinal);

                if (pluralName != null && originalName.SequenceEqual(pluralName.AsSpan()))
                {
                    singularName = originalName.Slice(0, originalName.Length - 1).ToString();

                    return null; // seems the original name is already the plural name, so we do not report that
                }

                return pluralName;
            }

            var index = originalName.IndexOfAny(Splitters);

            if (index > 0)
            {
                var nameToInspect = originalName.Slice(0, index);

                var pluralName = FindPluralName(nameToInspect, out singularName);

                if (pluralName is null)
                {
                    return null; // seems the original name is already the plural name, so we do not report that
                }

                var remainingPart = originalName.Slice(index);

                singularName = singularName.ConcatenatedWith(remainingPart);

                return pluralName.ConcatenatedWith(remainingPart);
            }
            else
            {
                var pluralName = originalName.EndsWithNumber()
                                 ? originalName.WithoutNumberSuffix()
                                 : originalName;

                singularName = pluralName.ToString();

                if (pluralName.EndsWithAny(Constants.Markers.Collections, StringComparison.OrdinalIgnoreCase))
                {
                    singularName = singularName.AsCachedBuilder()
                                               .ReplaceWithProbe("lementNodeList", "lementList")
                                               .ReplaceWithProbe("lementReferenceNodeList", "lementList")
                                               .ToStringAndRelease();

                    return Pluralizer.GetPluralName(singularName, StringComparison.OrdinalIgnoreCase, Constants.Markers.Collections);
                }

                return Pluralizer.GetPluralName(singularName);
            }
        }
#pragma warning restore CA1021

        protected static string GetFieldPrefix(string fieldName) => GetFieldPrefix(fieldName.AsSpan());

        protected static string GetFieldPrefix(in ReadOnlySpan<char> fieldName)
        {
            var fieldPrefixes = Constants.Markers.FieldPrefixes;

            for (int index = 0, prefixesLength = fieldPrefixes.Length; index < prefixesLength; index++)
            {
                var prefix = fieldPrefixes[index];

                if (prefix.Length > 0 && fieldName.StartsWith(prefix))
                {
                    return prefix;
                }
            }

            return string.Empty;
        }

        protected static bool IsNameForStructuralDesignPattern(string name) => name.EndsWithAny(Constants.Names.StructuralDesignPatternNames.Keys, StringComparison.OrdinalIgnoreCase);

        protected sealed override IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                ? AnalyzeName(symbol, compilation)
                                                                                                                                : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                           ? AnalyzeName(symbol, compilation)
                                                                                                                           : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            IEnumerable<Diagnostic> namingIssues = Array.Empty<Diagnostic>();
            IEnumerable<Diagnostic> localFunctionIssues = Array.Empty<Diagnostic>();

            if (ShallAnalyze(symbol))
            {
                namingIssues = AnalyzeName(symbol, compilation);
            }

            if (ShallAnalyzeLocalFunctions(symbol))
            {
                localFunctionIssues = AnalyzeLocalFunctions(symbol, compilation);
            }

            var noNamingIssues = namingIssues.IsEmptyArray();
            var noLocalFunctionIssues = localFunctionIssues.IsEmptyArray();

            if (noLocalFunctionIssues)
            {
                if (noNamingIssues)
                {
                    // nothing to report here
                    return Array.Empty<Diagnostic>();
                }

                return namingIssues;
            }

            return namingIssues.Concat(localFunctionIssues);
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                              ? AnalyzeName(symbol, compilation)
                                                                                                                              : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                        ? AnalyzeName(symbol, compilation)
                                                                                                                        : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                        ? AnalyzeName(symbol, compilation)
                                                                                                                        : Array.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                ? AnalyzeName(symbol, compilation)
                                                                                                                                : Array.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation)
        {
            var localFunctions = symbol.GetLocalFunctions();

            if (localFunctions.Count is 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var semanticModel = compilation.GetSemanticModel(localFunctions[0].SyntaxTree);

            return localFunctions.Select(_ => _.GetSymbol(semanticModel))
                                 .Where(ShallAnalyzeLocalFunction)
                                 .SelectMany(_ => AnalyzeName(_, compilation));
        }

        protected virtual bool ShallAnalyze(INamespaceSymbol symbol) => symbol.CanBeReferencedByName && symbol.IsGlobalNamespace is false;

        protected virtual bool ShallAnalyze(ITypeSymbol symbol) => symbol.CanBeReferencedByName;

        protected virtual bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.CanBeReferencedByName is false)
            {
                return false;
            }

            if (symbol.IsOverride)
            {
                return false;
            }

            switch (symbol.MethodKind)
            {
                case MethodKind.Ordinary:
                    return symbol.IsInterfaceImplementation() is false;

                case MethodKind.LocalFunction:
                    return true;

                default:
                    return false;
            }
        }

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => symbol.CanBeReferencedByName && symbol.IsOverride is false && symbol.IsInterfaceImplementation() is false;

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => symbol.CanBeReferencedByName && symbol.IsOverride is false && symbol.IsInterfaceImplementation() is false;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => symbol.CanBeReferencedByName && symbol.IsOverride is false;

        protected virtual bool ShallAnalyze(IParameterSymbol symbol)
        {
            if (symbol.CanBeReferencedByName is false)
            {
                return false;
            }

            if (symbol.IsOverride)
            {
                return false;
            }

            if (symbol.ContainingSymbol is IMethodSymbol method && method.IsConstructor())
            {
                if (method.HasAttribute("System.Text.Json.Serialization.JsonConstructorAttribute")
                 || method.HasAttribute("Newtonsoft.Json.JsonConstructorAttribute"))
                {
                    // ignore Json constructors
                    return false;
                }
            }

            return true;
        }

        protected virtual bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false;

        protected virtual bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        protected virtual IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => Array.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeEntityMarkers(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            if (symbolName.HasEntityMarker())
            {
                var betterName = FindBetterNameForEntityMarker(symbolName);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Array.Empty<Diagnostic>();
        }

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol)
        {
            var betterName = FindBetterNameForCollectionSuffix(symbol.Name);

            if (betterName.IsNullOrWhiteSpace())
            {
                return null;
            }

            return Issue(symbol, betterName, CreateBetterNameProposal(betterName));
        }

        protected void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;

            if (node.IsConst)
            {
                return;
            }

            var semanticModel = context.SemanticModel;
            var type = node.Declaration.GetTypeSymbol(semanticModel);

            if (type is null)
            {
                // may happen for a "ref var xyz" value
                return;
            }

            if (ShallAnalyze(type))
            {
                var issues = AnalyzeIdentifiers(semanticModel, type, node.Declaration.Variables.ToArray(_ => _.Identifier));

                ReportDiagnostics(context, issues);
            }
        }

        protected void AnalyzeDeclarationExpression(SyntaxNodeAnalysisContext context)
        {
            var declaration = (DeclarationExpressionSyntax)context.Node;

            if (declaration.Parent is ArgumentSyntax argument && argument.RefKindKeyword.IsKind(SyntaxKind.OutKeyword))
            {
                var semanticModel = context.SemanticModel;
                var type = declaration.GetTypeSymbol(semanticModel);

                if (ShallAnalyze(type))
                {
                    var issues = Analyze(semanticModel, type, declaration.Designation);

                    ReportDiagnostics(context, issues);
                }
            }
        }

        protected virtual void AnalyzeDeclarationPattern(SyntaxNodeAnalysisContext context)
        {
            var node = (DeclarationPatternSyntax)context.Node;

            var semanticModel = context.SemanticModel;
            var type = node.Type.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type))
            {
                var issues = Analyze(semanticModel, type, node.Designation);

                ReportDiagnostics(context, issues);
            }
        }

        protected virtual void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (ForEachStatementSyntax)context.Node;

            var semanticModel = context.SemanticModel;
            var type = node.Type.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type))
            {
                var issues = AnalyzeIdentifiers(semanticModel, type, node.Identifier);

                ReportDiagnostics(context, issues);
            }
        }

        protected virtual void AnalyzeForStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (ForStatementSyntax)context.Node;
            var variableDeclaration = node.Declaration;

            if (variableDeclaration is null)
            {
                // ignore variables that are not set
                return;
            }

            var semanticModel = context.SemanticModel;
            var type = variableDeclaration.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type))
            {
                var issues = AnalyzeIdentifiers(semanticModel, type, variableDeclaration.Variables.ToArray(_ => _.Identifier));

                ReportDiagnostics(context, issues);
            }
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => Array.Empty<Diagnostic>();

        private static string HandleSpecialEntityMarkerSituations(string symbolName)
        {
            var name = symbolName.Without(Constants.Markers.Models);

            switch (name.Length)
            {
                case 0:
                    return symbolName[0].IsUpperCase() ? Constants.Entity : Constants.entity;

                case 1:
                    switch (name)
                    {
                        case "s": return Constants.entities;
                        case Constants.Markers.MemberFieldPrefix: return Constants.Markers.MemberFieldPrefix + Constants.entity;
                        default: return name;
                    }

                case 2:
                    switch (name)
                    {
                        case Constants.Markers.AlternativeMemberFieldPrefix: return Constants.Markers.AlternativeMemberFieldPrefix + Constants.entity;
                        case Constants.Markers.StaticFieldPrefix: return Constants.Markers.StaticFieldPrefix + Constants.entity;
                        case Constants.Markers.ThreadStaticFieldPrefix: return Constants.Markers.ThreadStaticFieldPrefix + Constants.entity;
                        default: return name;
                    }

                default:
                {
                    var index = 0;

                    var fieldPrefixes = Constants.Markers.FieldPrefixes;

                    for (int i = 0, length = fieldPrefixes.Length; i < length; i++)
                    {
                        var prefix = fieldPrefixes[i];

                        if (symbolName.StartsWith(prefix, StringComparison.Ordinal))
                        {
                            index = prefix.Length;
                        }
                    }

                    if (symbolName[index].IsUpperCase() && name[index].IsLowerCase())
                    {
                        return name.ToUpperCaseAt(index);
                    }

                    if (symbolName[index].IsLowerCase() && name[index].IsUpperCase())
                    {
                        return name.ToLowerCaseAt(index);
                    }

                    return name;
                }
            }
        }

        private IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, ITypeSymbol type, VariableDesignationSyntax node)
        {
            switch (node)
            {
                case SingleVariableDesignationSyntax s:
                    return AnalyzeIdentifiers(semanticModel, type, s.Identifier);

                case ParenthesizedVariableDesignationSyntax s:
                    return s.Variables.SelectMany(_ => Analyze(semanticModel, type, _));

                default:
                    return Array.Empty<Diagnostic>();
            }
        }
    }
}