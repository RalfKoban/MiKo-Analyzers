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

        private const string Name = "cancellationToken";

        public MiKo_1042_CancellationTokenParameterNameAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            foreach (var parameter in method.Parameters)
            {
                if (parameter.Type.IsCancellationToken() && parameter.Name != Name)
                {
                    return new[] { ReportIssue(parameter, Name) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}