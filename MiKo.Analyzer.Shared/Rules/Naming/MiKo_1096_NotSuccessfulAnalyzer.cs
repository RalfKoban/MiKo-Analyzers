using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1096_NotSuccessfulAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1096";

        private static readonly string[] WrongPhrases =
                                                        {
                                                            "Not_Successful",
                                                            "NotSuccessful",
                                                            "Not_Sucessful", // to find typos
                                                            "NotSucessful", // to find typos
                                                            "Not_Succesful", // to find typos
                                                            "NotSuccesful", // to find typos
                                                        };

        public MiKo_1096_NotSuccessfulAnalyzer() : base(Id, (SymbolKind)(-1))
        {
        }

        protected override void InitializeCore(CompilationStartAnalysisContext context) => InitializeCore(context, SymbolKind.NamedType, SymbolKind.Method, SymbolKind.Property, SymbolKind.Event, SymbolKind.Field);

        protected override IEnumerable<Diagnostic> AnalyzeName(INamedTypeSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IPropertySymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IEventSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override IEnumerable<Diagnostic> AnalyzeName(IFieldSymbol symbol, Compilation compilation) => AnalyzeName(symbol);

        protected override bool ShallAnalyze(IMethodSymbol symbol) => base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        private IEnumerable<Diagnostic> AnalyzeName(ISymbol symbol)
        {
            var symbolName = symbol.Name;

            if (symbolName.ContainsAny(WrongPhrases, StringComparison.OrdinalIgnoreCase))
            {
                return new[] { Issue(symbol) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}
