using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    public abstract class NamingAnalyzer : Analyzer
    {
        private static readonly ConcurrentDictionary<string, string> PluralNames = new ConcurrentDictionary<string, string>();

        protected NamingAnalyzer(string diagnosticId, SymbolKind kind = SymbolKind.Method, bool isEnabledByDefault = true) : base(nameof(Naming), diagnosticId, kind, isEnabledByDefault)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.MethodKind == MethodKind.ExplicitInterfaceImplementation || method.IsOverride)
                return Enumerable.Empty<Diagnostic>();

            return method.MethodKind == MethodKind.Ordinary
                       ? AnalyzeOrdinaryMethod(method)
                       : Enumerable.Empty<Diagnostic>();
        }

        protected virtual IEnumerable<Diagnostic> AnalyzeOrdinaryMethod(IMethodSymbol method) => Enumerable.Empty<Diagnostic>();

        protected IEnumerable<Diagnostic> AnalyzeEntityMarkers(ISymbol symbol)
        {
            if (!symbol.Name.HasEntityMarker()) return Enumerable.Empty<Diagnostic>();

            var expected = HandleSpecialEntityMarkerSituations(symbol.Name);

            if (expected.HasCollectionMarker())
                expected = FindPluralName(expected, StringComparison.OrdinalIgnoreCase, Constants.CollectionMarkers);

            return new[] { ReportIssue(symbol, expected) };

        }

        private static string HandleSpecialEntityMarkerSituations(string symbolName)
        {
            var name = symbolName.RemoveAll(Constants.EntityMarkers);
            switch (name.Length)
            {
                case 0: return symbolName[0].IsUpperCase() ? "Entity" : "entity";
                case 1:
                    switch (name[0])
                    {
                        case 's': return "entities";
                        case '_': return "_entity";
                        default: return name;
                    }
                case 2:
                    switch (name)
                    {
                        case "s_": return "s_entity";
                        case "m_": return "m_entity";
                        default: return name;
                    }
                default: return name;
            }
        }

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol) => Constants.CollectionMarkers.Select(suffix => AnalyzeCollectionSuffix(symbol, suffix)).FirstOrDefault(_ => _ != null);

        protected Diagnostic AnalyzeCollectionSuffix(ISymbol symbol, string suffix, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            var betterName = FindPluralName(symbol.Name, comparison, suffix);
            return betterName.IsNullOrWhiteSpace() ? null : ReportIssue(symbol, betterName);
        }

        protected void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
        {
            var node = (LocalDeclarationStatementSyntax)context.Node;
            if (node.IsConst) return;

            var semanticModel = context.SemanticModel;
            var type = semanticModel.GetTypeInfo(node.Declaration.Type).Type;
            if (!ShallAnalyze(type)) return;

            var diagnostics = Analyze(semanticModel, node.Declaration.Variables.Select(_ => _.Identifier).ToArray());
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        protected virtual void AnalyzeDeclarationPattern(SyntaxNodeAnalysisContext context)
        {
            var node = (DeclarationPatternSyntax)context.Node;
            var semanticModel = context.SemanticModel;

            if (!ShallAnalyze(semanticModel.GetTypeInfo(node.Type).Type)) return;

            var diagnostics = Analyze(semanticModel, node.Designation);
            foreach (var diagnostic in diagnostics)
            {
                context.ReportDiagnostic(diagnostic);
            }
        }

        private IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, VariableDesignationSyntax node)
        {
            switch (node)
            {
                case SingleVariableDesignationSyntax s:
                    return Analyze(semanticModel, s.Identifier);

                case ParenthesizedVariableDesignationSyntax s:
                    return s.Variables.SelectMany(_ => Analyze(semanticModel, _));

                default:
                    return Enumerable.Empty<Diagnostic>();
            }
        }

        protected virtual bool ShallAnalyze(ITypeSymbol symbol) => false;

        protected virtual IEnumerable<Diagnostic> Analyze(SemanticModel semanticModel, params SyntaxToken[] identifiers) => Enumerable.Empty<Diagnostic>();

        protected static string FindPluralName(string symbolName, StringComparison comparison = StringComparison.OrdinalIgnoreCase, params string[] suffixes)
        {
            foreach (var suffix in suffixes)
            {
                if (symbolName.EqualsAny(StringComparison.Ordinal, "blackList" ,"whiteList", "array", "list", "collection")) continue;

                if (!symbolName.EndsWith(suffix, comparison)) continue;

                var proposedName = symbolName.WithoutSuffix(suffix);
                if (symbolName.IsEntityMarker())
                    proposedName = proposedName.RemoveAll(Constants.EntityMarkers);

                return GetPluralName(symbolName, proposedName, comparison);
            }

            return null;
        }

        protected static string GetPluralName(string symbolName, string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase) => PluralNames.GetOrAdd(symbolName, _ => CreatePluralName(proposedName, comparison));

        private static string CreatePluralName(string proposedName, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (proposedName.EndsWith("y", comparison)) return proposedName.Substring(0, proposedName.Length - 1) + "ies";
            if (proposedName.EndsWith("ys", comparison)) return proposedName.Substring(0, proposedName.Length - 2) + "ies";
            if (proposedName.EndsWith("ss", comparison)) return proposedName + "es";
            if (proposedName.EndsWith("ed", comparison)) return proposedName;
            if (proposedName.EndsWith("child", comparison)) return proposedName + "ren";
            if (proposedName.EndsWith("children", comparison)) return proposedName;
            if (proposedName.EndsWith("complete", comparison)) return "all";
            if (proposedName.EndsWith("Data", comparison)) return proposedName;
            if (proposedName.EndsWith("Datas", comparison)) return proposedName.Substring(0, proposedName.Length - 1);
            if (proposedName.EndsWith("ndex", comparison)) return proposedName.Substring(0, proposedName.Length - 2) + "ices";
            if (proposedName.EndsWith("nformation", comparison)) return proposedName;
            if (proposedName.EndsWith("nformations", comparison)) return proposedName.Substring(0, proposedName.Length - 1);

            var pluralName = proposedName;
            if (proposedName.EndsWith("ToConvert", comparison))
                pluralName = proposedName.Substring(0, proposedName.Length - "ToConvert".Length);

            if (proposedName.EndsWith("ToModel", comparison))
                pluralName = proposedName.Substring(0, proposedName.Length - "ToModel".Length);

            if (proposedName.HasEntityMarker())
                pluralName = proposedName.RemoveAll(Constants.EntityMarkers);

            var candidate = pluralName.EndsWith("s", comparison) ? pluralName : pluralName + "s";

            if (candidate.Equals("bases", comparison)) return "items"; // special handling
            if (candidate.Equals("_bases", comparison)) return "_items"; // special handling
            if (candidate.Equals("m_bases", comparison)) return "m_items"; // special handling
            if (candidate.Equals("sources", comparison)) return "source"; // special handling
            if (candidate.Equals("_sources", comparison)) return "_source"; // special handling
            if (candidate.Equals("m_sources", comparison)) return "m_source"; // special handling

            return candidate;
        }
    }
}