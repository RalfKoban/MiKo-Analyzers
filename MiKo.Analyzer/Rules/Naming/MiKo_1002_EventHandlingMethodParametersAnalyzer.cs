using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace MiKoSolutions.Analyzers.Rules.Naming
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class MiKo_1002_EventHandlingMethodParametersAnalyzer : NamingAnalyzer
    {
        public const string Id = "MiKo_1002";

        public const string Sender = "sender";
        public const string EventArgs = "e";

        public MiKo_1002_EventHandlingMethodParametersAnalyzer() : base(Id)
        {
        }

        protected override bool ShallAnalyze(IMethodSymbol method) => method.IsEventHandler();

        protected override IEnumerable<Diagnostic> AnalyzeName(IMethodSymbol method)
        {
            var parameters = method.Parameters;

            if (parameters[0].Name != Sender)
            {
                yield return Issue(parameters[0], Sender);
            }

            if (parameters[1].Name != EventArgs)
            {
                yield return Issue(parameters[1], EventArgs);
            }
        }
    }
}