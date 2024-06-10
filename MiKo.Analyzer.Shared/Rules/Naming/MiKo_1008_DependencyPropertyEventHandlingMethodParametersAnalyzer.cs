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

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsDependencyObjectEventHandler();

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => symbol.IsDependencyObjectEventHandler() is false;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.IsDependencyObjectEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var parameters = symbol.Parameters;

            var d = parameters[0];

            if (d.Name != Parameter1)
            {
                yield return Issue(d, Parameter1, CreateBetterNameProposal(Parameter1));
            }

            var e = parameters[1];

            if (e.Name != Parameter2)
            {
                yield return Issue(e, Parameter2, CreateBetterNameProposal(Parameter2));
            }
        }
    }
}