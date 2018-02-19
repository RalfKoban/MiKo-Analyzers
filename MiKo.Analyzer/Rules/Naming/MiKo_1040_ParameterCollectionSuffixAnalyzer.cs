using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1040_ParameterCollectionSuffixAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1040";

        public MiKo_1040_ParameterCollectionSuffixAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol symbol)
        {
            List<Diagnostic> list = null;

            foreach (var diagnostic in symbol.Parameters.Select(AnalyzeCollectionSuffix).Where(_ => _ != null))
            {
                if (list == null) list = new List<Diagnostic>();
                list.Add(diagnostic);
            }

            return list ?? Enumerable.Empty<Diagnostic>();
        }
    }
}