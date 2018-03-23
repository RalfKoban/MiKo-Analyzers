using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1039_ExtensionMethodsParameterAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1039";

        private const string Name1 = "value";
        private const string Name2 = "source";

        public MiKo_1039_ExtensionMethodsParameterAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeMethod(IMethodSymbol method)
        {
            if (method.IsExtensionMethod)
            {
                var parameter = method.Parameters[0];
                if (parameter.Name != Name1 && parameter.Name != Name2)
                    return new[] { ReportIssue(parameter, Name1, Name2) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}