using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1028_LocalFunctionNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1028";

        public MiKo_1028_LocalFunctionNameLengthAnalyzer() : base(Id, SymbolKind.Method, Constants.MaxNamingLengths.Methods)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => false;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => Analyze(symbol);
    }
}