using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1002_EventHandlingMethodParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1002";

        private const string Parameter1 = "sender";
        private const string Parameter2 = "e";

        public MiKo_1002_EventHandlingMethodParametersAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol symbol) => symbol.IsEventHandler();

        protected override bool ShallAnalyzeLocalFunctions(IMethodSymbol symbol) => symbol.IsEventHandler() is false;

        protected override bool ShallAnalyzeLocalFunction(IMethodSymbol symbol) => symbol.IsEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol symbol, Compilation compilation)
        {
            var parameters = symbol.Parameters;
            var sender = parameters[0];

            if (sender.Name != Parameter1)
            {
                yield return Issue(sender, Parameter1, CreateBetterNameProposal(Parameter1));
            }

            var e = parameters[1];

            if (e.Name != Parameter2)
            {
                yield return Issue(e, Parameter2, CreateBetterNameProposal(Parameter2));
            }
        }
    }
}