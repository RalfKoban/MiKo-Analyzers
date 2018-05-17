using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1022_ParameterNameLengthAnalyzer : NamingLengthAnalyzer
    {
        public const string Id = "MiKo_1022";

        public MiKo_1022_ParameterNameLengthAnalyzer() : base(Id, SymbolKind.Method, Constants.MaxNamingLengths.Parameters)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => !method.IsOverride;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol) => symbol.Parameters.SelectMany(Analyze).ToList();
    }
}