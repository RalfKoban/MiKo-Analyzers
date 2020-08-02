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

        private const string Remove = "Remove";
        private const string Clear = "Clear";

        public MiKo_1019_ClearRemoveMethodsAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IMethodSymbol method) => method.Parameters.Any()
                                                                           ? method.Name.Replace(Clear, Remove)
                                                                           : method.Name.Replace(Remove, Clear);

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var methodName = method.Name;

            const StringComparison Comparison = StringComparison.Ordinal;

            if (methodName.StartsWith(Clear, Comparison) && !methodName.StartsWith(Clear + "s", Comparison))
            {
                return method.Parameters.Any()
                       ? new[] { Issue(method, methodName.Replace(Clear, Remove)) }
                       : Enumerable.Empty<Diagnostic>();
            }

            if (methodName.StartsWith(Remove, Comparison) && !methodName.StartsWith(Remove + "s", Comparison))
            {
                return method.Parameters.Any()
                       ? Enumerable.Empty<Diagnostic>()
                       : new[] { Issue(method, methodName.Replace(Remove, Clear)) };
            }

            return Enumerable.Empty<Diagnostic>();
        }
    }
}