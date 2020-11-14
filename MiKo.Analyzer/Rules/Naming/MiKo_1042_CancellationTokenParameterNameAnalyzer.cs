using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1042_CancellationTokenParameterNameAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1042";

        public const string ExpectedName = "cancellationToken";

        public MiKo_1042_CancellationTokenParameterNameAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.Parameters.Length > 0;

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol)
        {
            foreach (var parameter in symbol.Parameters)
            {
                if (parameter.Type.IsCancellationToken() && parameter.Name != ExpectedName)
                {
                    return new[] { Issue(parameter, ExpectedName) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}