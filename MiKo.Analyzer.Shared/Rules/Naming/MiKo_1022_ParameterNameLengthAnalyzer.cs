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

        public MiKo_1022_ParameterNameLengthAnalyzer() : base(Id, SymbolKind.Method, Constants.MaxNamingLengths.Parameters) // we need to use methods here as - unfortunately - parameters on local functions are not considered
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsOverride is false;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => true;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation) => symbol.Parameters.SelectMany(Analyze);
    }
}