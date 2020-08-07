using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1008";

        private const string Parameter1 = "d";
        private const string Parameter2 = "e";

        public MiKo_1008_DependencyPropertyEventHandlingMethodParametersAnalyzer() : base(Id)
        {
        }

        internal static string FindBetterName(IParameterSymbol symbol)
        {
            var isParameter1 = symbol.ContainingSymbol is IMethodSymbol m && symbol.Equals(m.Parameters[0], SymbolEqualityComparer.Default);
            return isParameter1
                       ? Parameter1
                       : Parameter2;
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => method.IsDependencyPropertyEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var parameters = method.Parameters;

            if (parameters[0].Name != Parameter1)
            {
                yield return Issue(parameters[0], Parameter1);
            }

            if (parameters[1].Name != Parameter2)
            {
                yield return Issue(parameters[1], Parameter2);
            }
        }
    }
}