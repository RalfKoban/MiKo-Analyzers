using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Maintainability
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_3024_RefParameterAnalyzer : MaintainabilityAnalyzer
    {
        public const string Id = "MiKo_3024";

        public MiKo_3024_RefParameterAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> Analyze(IMethodSymbol method)
        {
            foreach (var parameter in method.Parameters)
            {
                if (parameter.RefKind == RefKind.Ref && parameter.Type.TypeKind != TypeKind.Struct)
                {
                    return new[] { ReportIssue(parameter) };
                }
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}