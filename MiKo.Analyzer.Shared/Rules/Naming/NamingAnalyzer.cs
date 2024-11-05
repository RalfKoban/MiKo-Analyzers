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

        protected static Dictionary<string, string> CreateBetterNameProposal(string betterName) => new Dictionary<string, string> { { Constants.AnalyzerCodeFixSharedData.BetterName, betterName } };

        protected static string FindBetterNameForEntityMarker(string symbolName)
        {
            var expected = HandleSpecialEntityMarkerSituations(symbolName);

            if (expected.HasCollectionMarker())
            {
                var plural = Pluralizer.GetPluralName(expected, StringComparison.OrdinalIgnoreCase, Constants.Markers.Collections);

                // symbol may have both Entity and Collection marker, such as 'ModelCollection', so 'plural' may be null
                expected = plural ?? (symbolName[0].IsUpperCase() ? Constants.Entities : Constants.entities);
            }

            return expected;
        }

        protected sealed override IEnumerable<Diagnostic> AnalyzeNamespace(INamespaceSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                                ? AnalyzeName(symbol, compilation)
                                                                                                                                : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeType(INamedTypeSymbol symbol, Compilation compilation) => ShallAnalyze(symbol)
                                                                                                                           ? AnalyzeName(symbol, compilation)
                                                                                                                           : Enumerable.Empty<Diagnostic>();

        protected sealed override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol, Compilation compilation)
        {
            if (ShallAnalyze(symbol))
            {
                foreach (var issue in AnalyzeName(symbol, compilation))
                {
                    yield return issue;
                }
            }

            if (ShallAnalyzeLocalFunctions(symbol))
            {
                foreach (var issue in AnalyzeLocalFunctions(symbol, compilation))
                {
                    yield return issue;
                }
            }
        }

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

        protected IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation)
        {
            var localFunctions = symbol.GetLocalFunctions();

            if (localFunctions.Count == 0)
            {
                return Enumerable.Empty<Diagnostic>();
            }

            var semanticModel = compilation.GetSemanticModel(symbol.GetSyntax().SyntaxTree);

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
                if (method.HasAttributeApplied("System.Text.Json.Serialization.JsonConstructorAttribute")
                 || method.HasAttributeApplied("Newtonsoft.Json.JsonConstructorAttribute"))
                {
                    // ignore Json constructors
                    return false;
                }
            }

            return true;
        }

        protected virtual bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => false;

        protected virtual bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        protected virtual IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected virtual IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeEntityMarkers(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            if (symbolName.HasEntityMarker())
            {
                var betterName = FindBetterNameForEntityMarker(symbolName);

                return new[] { Issue(symbol, betterName, CreateBetterNameProposal(betterName)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol) => Constants.Markers.Collections.Select(_ => AnalyzeCollectionSuffix(symbol, _)).FirstOrDefault(_ => _ != null);

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol, string suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var betterName = Pluralizer.GetPluralName(symbol.Name, comparison, suffix);

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

            if (ShallAnalyze(type) is false)
            {
                return;
            }

            var issues = AnalyzeIdentifiers(semanticModel, type, node.Declaration.Variables.ToArray(_ => _.Identifier));

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

            var issues = Analyze(semanticModel, type, node.Designation);

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

            var issues = AnalyzeIdentifiers(semanticModel, type, node.Identifier);

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

            var issues = AnalyzeIdentifiers(semanticModel, type, variableDeclaration.Variables.ToArray(_ => _.Identifier));

            ReportDiagnostics(context, issues);
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => Enumerable.Empty<Diagnostic>();

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

        private IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, ITypeSymbol type, VariableDesignationSyntax node)
        {
            switch (node)
            {
                case SingleVariableDesignationSyntax s:
                    return AnalyzeIdentifiers(semanticModel, type, s.Identifier);

                case ParenthesizedVariableDesignationSyntax s:
                    return s.Variables.SelectMany(_ => Analyze(semanticModel, type, _));

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }
    }
}