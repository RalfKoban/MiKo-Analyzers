using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1044_CommandSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1044";

        private const string Suffix = "Command";
        private const string CreatePrefix = MiKo_1016_FactoryMethodsAnalyzer.Prefix;

        private static readonly string[] SingleSuffix = { Suffix };
        private static readonly string[] Suffixes = { "_command", Suffix };

        public MiKo_1044_CommandSuffixAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Field);

        protected override bool ShallAnalyze(ITypeSymbol symbol) => symbol.IsCommand();

        protected override bool ShallAnalyze(IMethodSymbol symbol)
        {
            if (symbol.AssociatedSymbol is IPropertySymbol)
            {
                // ignore property getters or setters as they are already reported
                return false;
            }

            if (ShallAnalyze(symbol.ReturnType) is false)
            {
                return false;
            }

            if (symbol.Name.StartsWith(CreatePrefix, StringComparison.Ordinal))
            {
                // ignore factory methods
                return false;
            }

            if (symbol.ContainingType.IsTestClass())
            {
                // ignore code in tests, such as test methods
                return false;
            }

            return true;
        }

        protected override bool ShallAnalyze(IPropertySymbol symbol) => symbol.IsIndexer is false && ShallAnalyze(symbol.Type) && symbol.ContainingType.IsTestClass() is false;

        protected override bool ShallAnalyze(IFieldSymbol symbol) => ShallAnalyze(symbol.Type) && symbol.ContainingType.IsTestClass() is false;

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeNames(symbol, Suffixes);

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol) => AnalyzeNames(symbol, SingleSuffix);

        private IEnumerable<Diagnostic> AnalyzeNames(ISymbol symbol, IEnumerable<string> suffixes)
        {
            var symbolName = symbol.Name;

            if (suffixes.None(_ => symbolName.EndsWith(_, StringComparison.Ordinal)))
            {
                yield return Issue(symbol, Suffix, CreateBetterNameProposal(symbolName + Suffix));
            }
        }
    }
}