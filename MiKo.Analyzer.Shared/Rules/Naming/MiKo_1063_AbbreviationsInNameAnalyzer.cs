using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

using MiKoSolutions.Analyzers.Linguistics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1063_AbbreviationsInNameAnalyzer : NamingLocalVariableAnalyzer
    {
        public const string Id = "MiKo_1063";

        public MiKo_1063_AbbreviationsInNameAnalyzer() : base(Id)
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context)
        {
            InitializeCore(context, SymbolKind.Namespace, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field, SymbolKind.Parameter);

            base.InitializeCore(context);
        }

        protected override IEnumerable<Diagnostic> AnalyzeIdentifiers(SemanticModel semanticModel, ITypeSymbol type, params SyntaxToken[] identifiers) => identifiers.Select(_ => _.GetSymbol(semanticModel)).SelectMany(AnalyzeName);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsExtern is false && base.ShallAnalyze(symbol);

        protected override bool ShallAnalyze(IParameterSymbol symbol)
        {
            if (symbol.ContainingSymbol is IMethodSymbol method && method.IsExtern)
            {
                return false;
            }

            return base.ShallAnalyze(symbol);
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(INamespaceSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IParameterSymbol symbol, Compilation compilation)
        {
            switch (symbol.Name)
            {
                case "paramName": // used in exceptions
                case "lParam": // used by Windows C++ API
                case "wParam": // used by Windows C++ API
                    return Array.Empty<Diagnostic>();

                default:
                    return AnalyzeName(symbol);
            }
        }

        private Diagnostic[] AnalyzeName(ISymbol symbol) => AnalyzeName(symbol.Name, symbol);

        private Diagnostic[] AnalyzeName(IFieldSymbol symbol)
        {
            var symbolName = symbol.Name;

            // remove any field marker
            var fieldPrefix = GetFieldPrefix(symbolName);
            var prefixLength = fieldPrefix.Length;

            return prefixLength > 0
                   ? AnalyzeName(symbolName.Substring(prefixLength), symbol, fieldPrefix)
                   : AnalyzeName(symbolName, symbol);
        }

        private Diagnostic[] AnalyzeName(string symbolName, ISymbol symbol, string prefix = "")
        {
            var findings = AbbreviationDetector.Find(symbolName);
            var findingsLength = findings.Length;

            if (findingsLength == 0)
            {
                return Array.Empty<Diagnostic>();
            }

            var betterName = AbbreviationDetector.ReplaceAllAbbreviations(symbolName, findings);
            var betterNameProposal = CreateBetterNameProposal(prefix.Length > 0 ? prefix + betterName : betterName);

            var issues = new Diagnostic[findingsLength];

            for (var index = 0; index < findingsLength; index++)
            {
                var pair = findings[index];

                issues[index] = Issue(symbol, pair.Key, pair.Value, betterNameProposal);
            }

            return issues;
        }
    }
}