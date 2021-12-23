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
            var method = symbol.GetEnclosingMethod();

            return symbol.Equals(method.Parameters[0], SymbolEqualityComparer.Default)
                       ? Parameter1
                       : Parameter2;
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsDependencyObjectEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var parameters = symbol.Parameters;

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