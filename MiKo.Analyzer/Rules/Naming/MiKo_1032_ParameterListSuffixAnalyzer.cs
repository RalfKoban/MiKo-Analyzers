using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1032_ParameterListSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1032";

        public MiKo_1032_ParameterListSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol)
        {
            List<Diagnostic> list = null;

            foreach (var diagnostic in symbol.Parameters.Select(Analyze).Where(_ => _ != null))
            {
                if (list == null) list = new List<Diagnostic>();
                list.Add(diagnostic);
            }

            return list ?? Enumerable.Empty<Diagnostic>();
        }

        private Diagnostic Analyze(ISymbol symbol) => AnalyzeSuffix(symbol, "List") ?? AnalyzeSuffix(symbol, "Dictionary");
    }
}