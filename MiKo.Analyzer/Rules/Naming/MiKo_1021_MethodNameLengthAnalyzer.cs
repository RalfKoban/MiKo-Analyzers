using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1021_MethodNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1021";

        public MiKo_1021_MethodNameLengthAnalyzer() : base(Id, SymbolKind.Method, Constants.MaxNamingLengths.Methods)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsSpecialAccessor() is false && base.ShallAnalyze(symbol) && symbol.IsTestMethod() is false;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => false;

        protected override IEnumerable<Diagnostic> AnalyzeLocalFunctions(IMethodSymbol symbol, Compilation compilation) => Enumerable.Empty<Diagnostic>(); // don't consider local functions at all

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => Analyze(symbol);
    }
}