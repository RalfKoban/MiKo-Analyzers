using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1019_ClearRemoveMethodsAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1019";

        public MiKo_1019_ClearRemoveMethodsAnalyzer() : base(Id)
        {
        }

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var methodName = method.Name;

            const string Remove = "Remove";
            const string Clear = "Clear";
            if (methodName.StartsWith(Clear, StringComparison.OrdinalIgnoreCase))
            {
                return method.Parameters.Any()
                       ? new[] { Issue(method, methodName.Replace(Clear, Remove)) }
                       : Enumerable.Empty<Diagnostic>();
            }

            if (methodName.StartsWith(Remove, StringComparison.OrdinalIgnoreCase))
            {
                return method.Parameters.Any()
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(method, methodName.Replace(Remove, Clear)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}