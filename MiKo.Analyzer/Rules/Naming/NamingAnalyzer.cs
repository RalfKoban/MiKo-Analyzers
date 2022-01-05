using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingAnalyzer : Analyzer
    {
        protected NamingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method) : base(nameof(Naming), diagnosticId, kind)
        {
        }

        protected static string FindBetterNameForEntityMarker(ISymbol symbol)
        {
            var expected = HandleSpecialEntityMarkerSituations(symbol.Name);

            if (expected.HasCollectionMarker())
            {
                var plural = Pluralizer.GetPluralName(expected, StringComparison.OrdinalIgnoreCase, Constants.Markers.Collections);

                // symbol may have both Entity and Collection marker, such as 'ModelCollection', so 'plural' may be null
                expected = plural ?? (symbol.Name[0].IsUpperCase() ? Constants.Entities : Constants.entities);
            }

            return expected;
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                    ? AnalyzeName(symbol, compilation)
                                                                                                                                    : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                               ? AnalyzeName(symbol, compilation)
                                                                                                                               : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                              ? AnalyzeName(symbol, compilation)
                                                                                                                              : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeProperty(IPropertySymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                  ? AnalyzeName(symbol, compilation)
                                                                                                                                  : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeField(IFieldSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                            ? AnalyzeName(symbol, compilation)
                                                                                                                            : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeEvent(IEventSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                            ? AnalyzeName(symbol, compilation)
                                                                                                                            : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeParameter(IParameterSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                    ? AnalyzeName(symbol, compilation)
                                                                                                                                    : Enumerable.Empty<Diagnostic>();

        protected virtual bool ShallAnalyze(INamespaceSymbol symbol) => symbol.IsGlobalNamespace is false;

        protected virtual bool ShallAnalyze(ITypeSymbol symbol) => true;

        protected virtual bool ShallAnalyze(IMethodSymbol symbol) => symbol.MethodKind == MethodKind.Ordinary && symbol.IsOverride is false && symbol.IsInterfaceImplementation() is false;

        protected virtual bool ShallAnalyze(IPropertySymbol symbol) => symbol.IsOverride is false && symbol.IsInterfaceImplementation() is false;

        protected virtual bool ShallAnalyze(IEventSymbol symbol) => symbol.IsOverride is false && symbol.IsInterfaceImplementation() is false;

        protected virtual bool ShallAnalyze(IFieldSymbol symbol) => symbol.IsOverride is false;

        protected virtual bool ShallAnalyze(IParameterSymbol symbol) => symbol.IsOverride is false;

        protected virtual IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeEntityMarkers(ISymbol symbol)
        {
            if (symbol.Name.HasEntityMarker() is false)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var expected = FindBetterNameForEntityMarker(symbol);

            return new[] { Issue(symbol, expected) };
        }

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol) => Constants.Markers.Collections.Select(_ => AnalyzeCollectionSuffix(symbol, _)).FirstOrDefault(_ => _ != null);

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol, string suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var betterName = Pluralizer.GetPluralName(symbol.Name, comparison, suffix);

            return betterName.IsNullOrWhiteSpace() ? null : Issue(symbol, betterName);
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

            if (ShallAnalyze(type) is false)
            {
                return;
            }

            var issues = AnalyzeIdentifiers(semanticModel, node.Declaration.Variables.Select(_ => _.Identifier).ToArray());

            ReportDiagnostics(context, issues);
        }

        protected virtual void AnalyzeDeclarationPattern(SyntaxNodeAnalysisContext context)
        {
            var node = (DeclarationPatternSyntax)context.Node;

            var semanticModel = context.SemanticModel;
            var type = node.Type.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type) is false)
            {
                return;
            }

            var issues = Analyze(semanticModel, node.Designation);

            ReportDiagnostics(context, issues);
        }

        protected virtual void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (ForEachStatementSyntax)context.Node;

            var semanticModel = context.SemanticModel;
            var type = node.Type.GetTypeSymbol(semanticModel);

            if (ShallAnalyze(type) is false)
            {
                return;
            }

            var issues = AnalyzeIdentifiers(semanticModel, node.Identifier);

            ReportDiagnostics(context, issues);
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

            if (ShallAnalyze(type) is false)
            {
                return;
            }

            var issues = AnalyzeIdentifiers(semanticModel, variableDeclaration.Variables.Select(_ => _.Identifier).ToArray());

            ReportDiagnostics(context, issues);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, params SyntaxToken[] identifiers) => Enumerable.Empty<Diagnostic>();

        private static string HandleSpecialEntityMarkerSituations(string symbolName)
        {
            var name = symbolName.Without(Constants.Markers.Entities);
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

                    foreach (var prefix in Constants.Markers.FieldPrefixes)
                    {
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

        private IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, VariableDesignationSyntax node)
        {
            switch (node)
            {
                case SingleVariableDesignationSyntax s:
                    return AnalyzeIdentifiers(semanticModel, s.Identifier);

                case ParenthesizedVariableDesignationSyntax s:
                    return s.Variables.SelectMany(_ => Analyze(semanticModel, _));

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }
    }
}